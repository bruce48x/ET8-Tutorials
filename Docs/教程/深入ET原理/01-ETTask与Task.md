# ETTask与Task

这篇解释为什么 ET 使用 `ETTask`，它和 .NET 标准 `Task` 相比有什么优点和缺点。

先说结论：`ETTask` 是为了适配 ET 的 Fiber、单线程逻辑调度、对象池和框架内异步模型。它不是 `Task` 的全面替代品，也不应该随便和多线程并行模型混用。

## Task 解决什么问题

.NET 的 `Task` 是通用异步抽象，适合：

```text
IO 异步
线程池工作
并行任务
标准库和第三方库互操作
async/await
```

它的优势是生态完整，几乎所有 .NET 库都认识它。

但它是通用抽象，不知道 ET 的 `Fiber`、`Scene`、`Entity` 生命周期，也不会自动遵守 ET 的单线程逻辑约束。

## ETTask 想解决什么问题

ET 的很多逻辑运行在 Fiber 中。一个 Fiber 内的业务代码通常希望：

```text
同一段逻辑按顺序执行
避免随意切到别的线程修改 Entity
减少高频异步带来的分配
和 ET 的 Timer、Message、Event、Coroutine 风格统一
异常进入 ET 日志系统
```

所以 ET 提供 `ETTask`。

你在本仓库里会看到大量方法返回：

```csharp
async ETTask
async ETTask<T>
```

例如：

```text
LoginHelper.Login
ClientSenderComponentSystem.LoginAsync
C2R_LoginHandler.Run
AgarRoomSystem.FinishBattle
```

## 和 Fiber 的关系

ET 的 `FiberManager` 在每帧 `Update` 和 `LateUpdate` 中驱动 Fiber。

服务端入口里能看到：

```text
TimeInfo.Instance.Update()
FiberManager.Instance.Update()
FiberManager.Instance.LateUpdate()
```

这说明 ET 不是把所有业务都扔到随机线程池里跑。很多业务代码依赖“当前 Fiber 的上下文”。

`ETTask` 更适合表达：

```text
在 ET 调度模型里等待一个消息
等待一个 Timer
等待一个事件流程
等待一个内部 Fiber 调用
```

## 优点

`ETTask` 的主要优点：

1. 更贴合 ET 的调度模型。
2. 可以减少一些通用 `Task` 带来的额外分配。
3. 和 ET 的 `Coroutine()`、Timer、MessageSender 等工具统一。
4. 异常可以接入 `ETTask.ExceptionHandler`。
5. 读代码时能明确知道这是 ET 框架内异步，而不是普通 .NET 后台任务。

例如服务端初始化里有：

```text
ETTask.ExceptionHandler += Log.Error
```

这样未处理异常可以进入 ET 日志体系。

## 缺点

`ETTask` 也有明显限制：

1. 它不是 .NET 生态的通用接口。
2. 第三方库通常返回 `Task`，需要边界转换或单独处理。
3. 新手容易误以为 `async ETTask` 就是多线程。
4. 如果在错误 Fiber 或线程里修改 Entity，仍然可能出问题。
5. 调试时要理解 ET 的调度和消息链路，不能只按普通 `Task` 思维看。

一句话：

```text
ETTask 让 ET 内部异步更顺，但不会替你解决所有并发问题。
```

## ETTask 不是并行工具

看到 `async` 和 `await`，不要立刻理解成“开线程”。

在 ET 里，很多 `await` 只是让出当前流程，等消息、定时器或框架调度回来继续。

例如：

```text
await session.Call(request)
await timerComponent.WaitAsync(1000)
await FiberManager.Instance.Remove(fiberId)
```

这些表达的是异步等待，不等于 CPU 并行。

## 什么时候用 ETTask

建议：

```text
ET Entity/System/Event/MessageHandler 里的异步逻辑 -> ETTask
ET Timer、MessageSender、Fiber 调用 -> ETTask
需要接入 ET 异常和调度模型 -> ETTask
调用标准 .NET IO 或第三方库 -> 根据库返回 Task，再在边界处理
```

在 ET 业务层，不要随意 `Task.Run` 后修改 Entity。那很容易绕过 Fiber 线程模型。

## 常见误区

误区一：`ETTask` 比 `Task` 全面更好。

不是。`ETTask` 是 ET 框架内更合适，`Task` 是 .NET 生态更通用。

误区二：`await ETTask` 一定切线程。

不是。它通常只是异步恢复流程。

误区三：用了 `ETTask` 就不用管异常。

不是。你仍然要在业务边界处理错误，只是未处理异常可以进 ET 的异常处理器。

误区四：可以在任何线程修改 Entity。

不是。Entity 生命周期仍然要遵守 ET 的 Fiber 和 Scene 归属。

## 新人排查清单

1. Handler 里异步方法优先用 `ETTask`。
2. 不要用 `Task.Run` 直接操作 ET Entity。
3. 调用第三方 `Task` API 时，明确边界在哪里。
4. 异常没日志时，检查 `ETTask.ExceptionHandler`。
5. 卡住或超时，沿着消息和 Fiber 调用链排查，不要只看单个 await。
6. 高频逻辑里注意分配和生命周期。

下一篇：[02-热更.md](02-热更.md)
