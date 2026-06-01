# Fiber

这篇解释 ET 里的 `Fiber` 到底是什么：它和线程、Scene、Actor、ETTask 分别是什么关系，为什么 ET 要把业务逻辑放进 Fiber 里调度。

先说结论：`Fiber` 是 ET 的逻辑执行单元。它不是 C# 语言层面的协程，也不等于一个线程。你可以把它理解成“有自己 Root Scene、EntitySystem、消息邮箱和同步上下文的一套逻辑环境”。

## Fiber 解决什么问题

游戏服务端和客户端都容易遇到一个问题：逻辑对象很多，但又不能随便多线程乱改状态。

例如：

```text
玩家登录
匹配请求
房间战斗
客户端网络收包
UI 状态更新
服务器内部消息转发
```

这些流程经常异步发生。如果所有逻辑都直接用全局单例和线程池处理，很快会遇到：

```text
谁拥有这个 Entity？
这个 Handler 应该在哪个逻辑环境执行？
await 之后还能不能安全访问当前对象？
跨 Scene 调用会不会直接改到别人的状态？
同一个进程里多个服务如何隔离？
```

ET 用 Fiber 给这些问题划边界。

## Fiber 不是线程

很多新手看到 Fiber 会先把它理解成“轻量线程”。这个说法只能帮助入门，不能当成精确定义。

本仓库里 `FiberManager` 支持三种调度方式：

| SchedulerType | 含义 |
| --- | --- |
| `Main` | 在主线程 Update/LateUpdate 中调度 |
| `Thread` | 一个 Fiber 一个固定线程 |
| `ThreadPool` | 多个 Fiber 分配到线程池式调度循环 |

所以 Fiber 和线程不是一一绑定关系。

更准确地说：

```text
线程是操作系统执行资源
Fiber 是 ET 框架里的逻辑执行上下文
Scheduler 决定 Fiber 在哪个线程上被驱动
```

在 Unity Editor 的某些模式下，`Thread` 和 `ThreadPool` 还会被替换成主线程调度，避免编辑器环境下的线程问题。这也说明 Fiber 的核心不是“开线程”，而是“隔离逻辑上下文”。

## 一个 Fiber 里有什么

`Fiber` 创建时会初始化这些关键对象：

```text
Id
Zone
Root Scene
EntitySystem
Mailboxes
ThreadSynchronizationContext
Log
```

其中最重要的是：

- `Root Scene`：这个 Fiber 的根 Scene。
- `EntitySystem`：驱动这个 Fiber 内 Entity 的 Awake、Update、LateUpdate 等系统。
- `Mailboxes`：接收发给本 Fiber 内 Actor 的消息。
- `ThreadSynchronizationContext`：把回调投递回这个 Fiber 的执行队列。

所以当你说“某个 Entity 属于哪个 Fiber”，实际通常是：

```text
Entity -> IScene -> Scene.Fiber
```

本仓库里也有扩展方法：

```csharp
entity.Fiber()
entity.Root()
entity.Zone()
```

这些方法都依赖 Entity 所在 Scene 的 Fiber。

## Fiber 和 Scene 的关系

`Scene` 是业务语义，`Fiber` 是执行边界。

服务端运行模型通常是：

```text
Process
 -> Fiber
 -> Root Scene
 -> Entity / Component / System
```

`SceneType` 决定这个 Scene 承担什么职责：

```text
Main
Realm
Gate
Location
Match
Room
NetInner
NetClient
Agar
```

消息 Handler、事件 Handler、FiberInit 都会用 `SceneType` 绑定运行位置。例如：

```csharp
[MessageSessionHandler(SceneType.Gate)]
[MessageHandler(SceneType.Match)]
[Invoke((long)SceneType.NetClient)]
```

这意味着 Handler 是否触发，不是由文件夹决定的，而是由当前消息所在 Scene/Fiber 的 `SceneType` 决定的。

## Fiber 是怎么创建并启动的

服务端和客户端入口都会先创建 `FiberManager`。

服务端入口在 `DotNet/Loader/Init.cs`：

```text
添加 TimeInfo
添加 FiberManager
添加 CodeLoader
```

客户端入口在 `Unity/Assets/Scripts/Loader/MonoBehaviour/Init.cs`：

```text
添加 TimeInfo
添加 FiberManager
加载资源包
启动 CodeLoader
```

随后 `Entry.Start` 会创建主 Fiber：

```csharp
await FiberManager.Instance.Create(SchedulerType.Main, ConstFiberId.Main, 0, SceneType.Main, "");
```

创建过程可以简化成：

```text
new Fiber(...)
加入 FiberManager 字典
交给对应 Scheduler 调度
把 FiberInit 投递到该 Fiber 的 ThreadSynchronizationContext
在 Fiber 自己的上下文里执行对应 SceneType 的初始化逻辑
```

源码里有一句注释很关键：

```text
根据Fiber的SceneType分发Init,必须在Fiber线程中执行
```

这就是为什么 Fiber 初始化不是简单直接调用，而是先 `Post` 到自己的同步上下文。

## Main Fiber 做什么

主 Fiber 是一个进程的起点，`SceneType.Main`。

主 Fiber 初始化时会发布三个入口事件：

```text
EntryEvent1
EntryEvent2
EntryEvent3
```

服务端的 `EntryEvent2_InitServer` 会在这里根据 `StartSceneConfig` 创建真正的业务 Fiber：

```text
如果当前进程有内部通信端口 -> 创建 NetInner Fiber
遍历当前 Process 的 StartSceneConfig -> 创建 Realm/Gate/Location/Match 等 Fiber
```

所以服务端不是启动后立刻有所有逻辑服务，而是：

```text
进程启动
 -> Main Fiber
 -> 读取配置
 -> 创建各个 SceneType 对应的 Fiber
```

这也是部署和排查时必须看 `StartConfig` 的原因。

## Fiber 如何被调度

客户端和服务端每帧或每轮循环都会调用：

```text
TimeInfo.Instance.Update()
FiberManager.Instance.Update()
FiberManager.Instance.LateUpdate()
```

主线程调度器会依次取出 FiberId：

```text
设置 Fiber.Instance
设置 SynchronizationContext 为当前 Fiber 的 ThreadSynchronizationContext
执行 fiber.Update()
执行 fiber.LateUpdate()
恢复默认 SynchronizationContext
```

`fiber.Update()` 会驱动这个 Fiber 内的 `EntitySystem.Update()`。

`fiber.LateUpdate()` 会做三件事：

```text
EntitySystem.LateUpdate()
处理 WaitFrameFinish()
执行 ThreadSynchronizationContext 队列里的回调
```

这解释了两个常见现象：

1. `await` 之后逻辑通常还能回到 ET 的上下文继续执行。
2. 某些跨线程或网络回调不是立即改 Entity，而是投递到 Fiber 队列里等调度。

## ThreadSynchronizationContext 的作用

`ThreadSynchronizationContext` 很简单，核心就是一个线程安全队列：

```text
Post(action) -> 入队
Update() -> 逐个执行 action
```

它解决的是“回到正确 Fiber 执行”的问题。

例如创建 Fiber 时，初始化逻辑会被投递进去：

```text
fiber.ThreadSynchronizationContext.Post(async () => { ... FiberInit ... })
```

这样做的意义是：

```text
不要在创建者的上下文里初始化目标 Fiber
目标 Fiber 的 Root Scene、EntitySystem、组件 Awake 应该在目标 Fiber 上执行
```

如果你从普通线程、网络回调或第三方库回调里直接修改 Entity，就绕过了这个保护。

## Fiber 之间怎么通信

Fiber 之间不应该随便拿对方引用直接改状态。

`FiberManager.Get` 是 `internal`，源码注释也写得很直白：

```text
不允许外部调用，容易出现多线程问题，只能通过消息通信，不允许直接获取其它Fiber引用
```

同进程内 Fiber 通信用 `ProcessInnerSender`。

发送方会把消息放入 `MessageQueue`，目标 Fiber 的 `ProcessInnerSender.Update()` 再拉取属于自己的消息：

```text
MessageQueue.Send(from, actorId, message)
目标 Fiber Fetch 自己的队列
根据 ActorId.InstanceId 找 MailBoxComponent
投递到目标 Actor 的 MailBox
```

所以 ActorId 里不仅有进程信息，也有 Fiber 信息：

```text
Process
Fiber
InstanceId
```

这就是 ET Actor 模型和 Fiber 模型连在一起的地方。

## 客户端为什么有 NetClient Fiber

新手教程里讲过，客户端主逻辑不会直接操作 Gate Session，而是创建一个 `SceneType.NetClient` 的 Fiber。

登录时大致是：

```text
主 Fiber
 -> 创建 NetClient Fiber
 -> 发送 Main2NetClient_Login
 -> NetClient Fiber 建立网络连接并向服务端请求
 -> 把结果回给主 Fiber
```

本仓库 `ClientSenderComponentSystem.LoginAsync()` 里能看到：

```csharp
self.fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, 0, SceneType.NetClient, "");
self.netClientActorId = new ActorId(self.Fiber().Process, self.fiberId);
```

这样拆开的好处是：

```text
主逻辑关注玩家和 UI 状态
NetClient Fiber 关注网络连接、收包、发包、重连等细节
双方通过内部 Actor 消息通信
```

这不是为了把登录复杂化，而是为了把网络生命周期从主业务生命周期里隔离出来。

## 服务端为什么每个 Scene 一个 Fiber

服务端里 Realm、Gate、Match、Location 不是简单的类名，而是不同职责的逻辑服务。

用 Fiber 隔离后，每个服务有自己的：

```text
Root Scene
组件树
消息处理器
定时器
日志上下文
Actor 邮箱
```

例如：

```text
Gate 处理客户端 Session 和玩家入口
Match 处理匹配队列
Location 管理实体位置映射
NetInner 负责进程间内部通信
```

这些服务可以在同一个 dotnet 进程中运行，也可以按 `StartConfig` 拆到不同进程。业务代码通过消息通信，不需要关心它们最终部署在同进程还是跨进程。

## Fiber 和 ETTask 的关系

`ETTask` 不是 Fiber，但它经常在 Fiber 里运行。

更准确地说：

```text
Fiber 提供执行上下文
ETTask 表达这个上下文里的异步等待
ThreadSynchronizationContext 决定 await 后的恢复位置
```

例如：

```text
等待 Timer
等待内部 Actor Call 返回
等待 Session.Call 返回
等待 Fiber 创建或移除完成
```

这些等待大多不是“开一个线程去跑”，而是当前流程让出，等消息、定时器或队列回调再恢复。

## 为什么不要跨 Fiber 直接改 Entity

Fiber 的价值就在于让一组 Entity 在明确上下文里被驱动。

如果你跨 Fiber 直接拿引用修改对象，会破坏这些假设：

```text
Update/LateUpdate 顺序
组件生命周期
Actor 消息顺序
CoroutineLock
对象销毁时机
异常日志上下文
```

正确做法通常是：

```text
同 Fiber 内：直接操作当前 Scene 的 Entity
跨 Fiber：发 Actor 消息
跨进程：走内部网络消息或框架封装
第三方线程回调：Post 回目标 Fiber 上下文
```

如果你不确定当前代码在哪个 Fiber，可以从当前 Entity 的 `Fiber()`、`Root()`、`SceneType` 和日志里的 FiberId 反查。

## 常见误区

误区一：一个 Fiber 一定对应一个线程。

不是。调度器决定执行线程，Fiber 本身是逻辑上下文。

误区二：用了 Fiber 就没有并发问题。

不是。Fiber 降低共享状态复杂度，但跨 Fiber、跨线程、跨进程通信仍然要遵守边界。

误区三：可以通过 FiberManager 找到目标 Fiber 后直接改对象。

不应该。框架刻意把 `Get` 设为 `internal`，业务应通过消息通信。

误区四：SceneType 只是分类标签。

不是。它决定 Handler、Event、FiberInit 等运行在哪类 Scene/Fiber 上。

误区五：`async ETTask` 会自动保证线程安全。

不是。`ETTask` 只表达异步流程，安全边界仍然来自 Fiber、Scene、消息和生命周期管理。

## 新人排查清单

1. Handler 不触发，先检查当前 Scene 的 `SceneType` 是否匹配。
2. 内部消息发不到，检查 `ActorId` 的 `Process`、`Fiber`、`InstanceId`。
3. 跨 Fiber 调用卡住，检查目标 Fiber 是否创建、Mailbox 是否存在、请求是否 Reply。
4. 状态偶发错乱，检查是否有普通线程或 `Task.Run` 直接修改 Entity。
5. 服务端某个逻辑服务没起来，检查 `StartSceneConfig` 是否给当前 Process 配了对应 Scene。
6. 客户端网络相关问题，区分主 Fiber 和 `NetClient` Fiber 的职责。
7. await 后上下文异常，检查是否绕过了 ET 的 `ThreadSynchronizationContext`。

返回：[00-深入ET原理索引.md](00-深入ET原理索引.md)
