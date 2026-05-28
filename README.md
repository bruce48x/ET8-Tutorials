# ET8.1 教学文档

本仓库定位为 **ET8.1 新手教学文档**。

仓库中保留了一套基于 ET8.1 的 Unity 客户端、dotnet 服务端、热更新、协议、配置和部署示例代码。代码的主要作用是给教程提供可落地的例子，读者不需要先关心示例游戏本身的玩法目标。

## 适合谁阅读

这套文档面向第一次接触 ET8.1 的开发者。默认你能阅读 C# 和 Unity 工程结构，但还不熟悉 ET 的这些概念：

```text
Entity / Component / System
Scene / Fiber / Actor
Session / Message / RPC
Model / Hotfix / ModelView / HotfixView
协议生成
客户端和服务端热更
StartConfig 集群配置
```

## 推荐阅读入口

新手请从这里开始：

[Docs/教程/新手教程/00-教程索引.md](Docs/教程/新手教程/00-教程索引.md)

当你已经能看懂基本开发流程后，再阅读深入原理系列：

[Docs/教程/深入ET原理/00-深入ET原理索引.md](Docs/教程/深入ET原理/00-深入ET原理索引.md)

运行工程的基础步骤仍可参考 ET 原框架文档：

[Book/1.1运行指南.md](Book/1.1运行指南.md)

## 文档结构

```text
Docs/教程/新手教程/        面向新人，按开发流程解释 ET8.1 项目
Docs/教程/深入ET原理/      解释 ETTask、热更等机制和设计取舍
Docs/示例项目/             示例代码背景说明，不是新人必读主线
Book/                      ET 原框架参考资料
```

新手教程当前覆盖：

```text
项目整体结构
客户端如何连接服务端
一次请求的完整链路
新增接口和 DTO
服务端推送与广播
客户端开发入门
服务端运行模型
部署打包与集群运维
客户端如何热更
服务端如何热更
```

深入原理当前覆盖：

```text
ETTask 与 Task
热更机制
```

## 示例代码说明

仓库里的 `Agar` 模块是教学示例，用来演示登录、匹配、房间、状态同步、UI、服务端推送和热更等 ET8.1 常见开发链路。

示例玩法背景放在：

[Docs/示例项目/Agar玩法说明.md](Docs/示例项目/Agar玩法说明.md)

阅读教程时，请优先理解 ET 的项目组织和调用链路，不要把 `Agar` 的命名当作框架要求。

## 技术栈

- Unity 客户端。
- ET8.1 框架。
- dotnet 服务端。
- HybridCLR 客户端 C# 热更新。
- YooAsset 资源加载和 AssetBundle 管理。
- MemoryPack 协议和配置序列化。
- KCP、TCP、WebSocket 等网络传输能力由 ET 框架提供。

## 目录概览

```text
Unity/                 Unity 客户端工程
Unity/Assets/Scripts   客户端、服务端和共享 C# 源码
DotNet/                服务端和工具侧 .NET 工程
Share/                 共享工具、分析器、协议和导表相关代码
Config/                配置源数据和导出结果
Docs/                  本仓库整理的教学文档
Book/                  ET 原框架文档
Tools/                 构建、导表、发布等辅助工具
```

## 文档维护原则

- `README.md` 只作为教学文档入口，不承载具体教程正文。
- 新增教程优先放入 `Docs/教程/新手教程/` 或 `Docs/教程/深入ET原理/`。
- 示例玩法、示例业务背景放入 `Docs/示例项目/`。
- `Book/` 保留为 ET 原框架参考资料，不作为本仓库教程主线的替代。
