# Ball Battle

这是一个基于 [ET8.1](https://github.com/egametang/ET/tree/release8.1) 的 Unity 客户端和 dotnet 服务端一体化游戏项目，目标是实现类似《球球大作战》的实时多人吞噬成长玩法。

当前仓库已经包含 ET 框架的客户端、服务端、热更新、配置、资源和工具链基础。后续开发应围绕本项目的游戏目标组织代码，而不是继续把仓库当作通用 ET 框架示例。

## 项目目标

核心玩法方向：

1. 玩家控制一个球体在开放竞技场中移动。
2. 通过吞噬地图资源或体积更小的玩家成长。
3. 体积越大移动越慢，体积越小越灵活。
4. 支持分裂、合体、喷射、追击、逃跑等实时对抗行为。
5. 服务端负责权威状态、碰撞判定、成长结算和同步。
6. 客户端负责输入、预测表现、相机、UI、动画和资源展示。

优先实现顺序：

1. 单房间多人进入和离开。
2. 玩家移动、视野跟随、地图边界。
3. 食物刷新、吞噬、体积成长。
4. 玩家之间的吞噬判定。
5. 分裂、合体、喷射等进阶操作。
6. 排行榜、结算、匹配和房间生命周期。

## 技术栈

- Unity 客户端。
- ET8 框架，使用 Entity、Component、System、Event、Fiber 和 Actor 消息模型。
- C# 服务端，支持客户端和服务端共享逻辑。
- HybridCLR 客户端 C# 热更新。
- YooAsset 资源加载和 AssetBundle 管理。
- MemoryPack 协议和配置序列化。
- KCP、TCP、WebSocket 等网络传输能力由 ET 框架提供。

## 目录结构

```text
Unity/                 Unity 客户端工程
Unity/Assets/Scripts   客户端和共享代码
DotNet/                服务端和工具侧 .NET 工程
Share/                 共享工具、分析器、协议和导表相关代码
Config/                配置源数据和导出结果
Book/                  ET 原框架文档，作为框架参考资料保留
Tools/                 构建、导表、发布等辅助工具
```

常用代码目录：

```text
Unity/Assets/Scripts/Model
Unity/Assets/Scripts/Hotfix
Unity/Assets/Scripts/ModelView
Unity/Assets/Scripts/HotfixView
```

## 代码分层约定

本项目沿用 ET 的程序集拆分方式：

| 程序集 | 用途 |
| --- | --- |
| `Model` | 纯逻辑层的稳定类型定义，例如实体、组件、协议相关结构、配置结构 |
| `Hotfix` | 纯逻辑层的可热更行为，例如系统逻辑、消息处理、游戏规则 |
| `ModelView` | 客户端表现层的稳定类型定义，例如 UI 组件、GameObject 引用组件、表现层实体组件 |
| `HotfixView` | 客户端表现层的可热更行为，例如 UI 创建、按钮响应、资源加载、动画和相机逻辑 |

简单规则：

- 新增稳定的数据结构或组件类型，优先放 `Model` 或 `ModelView`。
- 修改玩法规则、消息处理、UI 行为、资源加载流程，优先放 `Hotfix` 或 `HotfixView`。
- 不依赖 `UnityEngine` 的逻辑放非 `View` 层。
- 依赖 UI、Prefab、GameObject、Transform、相机、动画等 Unity 表现能力的代码放 `View` 层。

## 热更新说明

客户端热更新基于 HybridCLR。热更程序集会被编译为 dll，并复制为 `.dll.bytes` 资源放入：

```text
Unity/Assets/Bundles/Code
```

运行时由 `CodeLoader` 加载：

```text
Unity/Assets/Scripts/Loader/CodeLoader.cs
```

这些 `.bytes` 文件不是普通文本资源，而是客户端启动和热更新加载链路的一部分，不要随意删除。

## 游戏开发方向

建议以“服务端权威，客户端表现”的方式实现类球球玩法：

- 服务端维护房间、玩家球体、食物、质量、位置、速度和碰撞结果。
- 客户端只上传输入意图，例如移动方向、分裂、喷射。
- 客户端可以做平滑插值和预测表现，但最终状态以服务端同步为准。
- 吞噬判定、质量变化、死亡和结算必须在服务端完成。
- 地图资源刷新和房间生命周期也应由服务端统一驱动。

推荐核心模块：

```text
Room              房间生命周期、玩家进入离开
Ball              玩家球体、质量、半径、速度、分裂体
Food              地图食物刷新和回收
Input             客户端输入上报
Sync              服务端状态广播和客户端插值表现
Collision         球体和食物、球体和球体的吞噬判定
Ranking           房间内排行榜和结算
```

## 开发入口

基础运行方式仍参考 ET 原框架文档：[Book/1.1运行指南.md](Book\1.1运行指南.md)

ET 框架开发方法和本项目示例参考：[Docs/开发指南.md](Docs/开发指南.md)


常见工作流：

1. 使用 Unity 打开 `Unity` 工程。
2. 修改客户端或共享代码。
3. 编译热更程序集。
4. 运行客户端和服务端。
5. 通过日志、断点和 ET Entity 可视化能力调试。

具体菜单、脚本和发布流程以后应在本 README 中补充为项目自己的说明。

## 文档维护原则

- `README.md` 是本游戏项目的入口文档。
- `Book/` 目录是 ET 框架参考资料，不作为本项目 README 的替代。
- 新增玩法系统、运行步骤、协议约定或部署方式后，应同步更新本文档。
- 面向项目开发者描述“怎么开发这个游戏”，不要继续堆放 ET 框架宣传内容。
