# 新增接口和DTO

这篇解释如何在 ET 项目里创建新接口和新的 DTO。

这里的“接口”通常指一组协议消息和对应处理器。“DTO”通常指消息里携带的数据结构，例如战斗状态里的 `AgarCellInfo`。

## 先判断消息类型

新增协议前，先判断它属于哪一类：

| 类型 | 用途 | 示例 |
| --- | --- | --- |
| `ISessionRequest` | 客户端发给服务端，并等待响应 | `C2G_AgarMatch` |
| `ISessionResponse` | 服务端返回给客户端 | `G2C_AgarMatch` |
| `ISessionMessage` | 客户端发给服务端，不等待响应 | `C2G_AgarMove` |
| `IRequest` | 服务端内部请求，等待响应 | `G2Match_AgarMatch` |
| `IResponse` | 服务端内部响应 | `Match2G_AgarMatch` |
| `IMessage` | 服务端内部或服务端推送消息，不等待响应 | `Match2G_AgarBattleState` |

判断原则：

```text
客户端到服务器长期连接 -> ISession*
服务器内部 Scene 之间 -> I*
需要等待结果 -> Request/Response
不等待结果 -> Message
```

## 协议文件在哪里

框架协议示例在：

```text
Unity/Assets/Config/Proto/OuterMessage_C_10001.proto
Unity/Assets/Config/Proto/InnerMessage_S_20001.proto
Unity/Assets/Config/Proto/ClientMessage_C_1000.proto
```

本仓库的 Agar 消息没有放在 proto 文件里，而是手写在：

```text
Unity/Assets/Scripts/Model/Share/Agar/Message/AgarMessage.cs
```

两种方式都能表达消息结构。新人要先看当前项目模块采用哪种方式，再保持一致。

## 生成代码放在哪里

从 proto 生成的消息类在：

```text
Unity/Assets/Scripts/Model/Generate/Client/Message/
Unity/Assets/Scripts/Model/Generate/Server/Message/
Unity/Assets/Scripts/Model/Generate/ClientServer/Message/
```

生成代码里会包含：

```text
[MemoryPackable]
[Message(...)]
[ResponseType(...)]
字段的 MemoryPackOrder
Create 和 Dispose
消息 id 常量
```

不要手改生成文件。下次生成会覆盖。

## 新增一个客户端请求的步骤

假设要加一个查询排行榜请求。

1. 定义请求和响应消息。
2. 给请求标注响应类型。
3. 选择消息 id。
4. 在服务端目标 Scene 写 Handler。
5. 在客户端调用 `ClientSenderComponent.Call`。
6. 编译或生成代码。

手写消息时结构类似：

```csharp
[MemoryPackable]
[Message(AgarOuter.C2G_AgarRank)]
[ResponseType(nameof(G2C_AgarRank))]
public partial class C2G_AgarRank : MessageObject, ISessionRequest
{
    [MemoryPackOrder(0)]
    public int RpcId { get; set; }
}
```

响应要包含：

```text
RpcId
Error
Message
业务字段
```

这是 ET 请求响应能正确配对和表达错误的基础。

## Handler 放在哪里

客户端到 Gate 的请求处理器示例：

```text
Unity/Assets/Scripts/Hotfix/Server/Agar/Gate/C2G_AgarMatchHandler.cs
```

结构是：

```csharp
[MessageSessionHandler(SceneType.Gate)]
public class C2G_AgarMatchHandler : MessageSessionHandler<C2G_AgarMatch, G2C_AgarMatch>
{
    protected override async ETTask Run(Session session, C2G_AgarMatch request, G2C_AgarMatch response)
    {
        await ETTask.CompletedTask;
    }
}
```

如果是服务端内部消息，比如 Gate 到 Match，Handler 放到对应 Scene 的目录：

```text
Unity/Assets/Scripts/Hotfix/Server/Agar/Match/
```

并使用：

```text
[MessageHandler(SceneType.Match)]
```

## DTO 应该放哪里

DTO 如果是协议的一部分，就跟消息放在一起。

例如：

```text
AgarCellInfo
AgarPlayerScoreInfo
```

它们在 `AgarMessage.cs` 里，被 `Match2G_AgarBattleState` 和 `Match2G_AgarBattleResult` 引用。

DTO 设计建议：

```text
只放传输需要的数据
不要直接传 Entity
字段名表达业务含义
列表项也定义成明确 DTO
```

## 编译和生成

改协议或 DTO 后，通常需要：

```text
重新生成协议代码
执行 ET/Compile
确认 Unity/Assets/Bundles/Code/*.dll.bytes 更新时间变化
```

如果只是改手写 Hotfix 逻辑，也至少需要 `ET/Compile`，否则运行时加载的可能还是旧 dll。

## 新人排查清单

1. 请求没有响应，检查 `ResponseType`。
2. Handler 不触发，检查消息接口类型和 `SceneType`。
3. 客户端编译过但运行没变化，检查是否执行 `ET/Compile`。
4. 生成代码被覆盖，说明你改错了文件。
5. DTO 不要引用服务端 Entity，协议只传输数据。
6. 消息方向命名要清楚，例如 `C2G`、`G2C`、`G2Match`。

下一篇：[05-服务端推送与广播.md](05-服务端推送与广播.md)
