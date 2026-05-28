# ET8 Newcomer Tutorials Design

## Goal

Rework the existing single `Docs/开发指南.md` into a numbered tutorial set for newcomers who are learning ET8 for the first time. The repository should be used as a concrete example, not as the only project the tutorials apply to.

## Audience

The primary reader is a developer who can read C# and Unity project structure, but has not built an ET8 client/server project before.

The tutorials should assume the reader does not yet understand ET-specific terms such as `Scene`, `Fiber`, `Session`, `Actor`, generated message classes, hotfix assemblies, or start config tables.

## Documentation Structure

Create a new tutorial root directory with two independently numbered series:

```text
Docs/教程/
Docs/教程/新手教程/
Docs/教程/深入ET原理/
```

The newcomer tutorial series should contain these numbered documents:

```text
Docs/教程/新手教程/00-教程索引.md
Docs/教程/新手教程/01-ET项目整体结构.md
Docs/教程/新手教程/02-客户端如何连接服务端.md
Docs/教程/新手教程/03-一次请求的完整链路.md
Docs/教程/新手教程/04-新增接口和DTO.md
Docs/教程/新手教程/05-服务端推送与广播.md
Docs/教程/新手教程/06-客户端开发入门.md
Docs/教程/新手教程/07-服务端运行模型.md
Docs/教程/新手教程/08-部署打包与集群运维.md
Docs/教程/新手教程/09-客户端如何热更.md
Docs/教程/新手教程/10-服务端如何热更.md
```

The advanced ET internals series should contain its own index and numbering:

```text
Docs/教程/深入ET原理/00-深入ET原理索引.md
Docs/教程/深入ET原理/01-ETTask与Task.md
Docs/教程/深入ET原理/02-热更.md
```

Future advanced internals tutorials should be added under `Docs/教程/深入ET原理/` and continue that series' own numbering.

`Docs/开发指南.md` should stop being the main long-form tutorial. It should become a short entry page that links to `Docs/教程/新手教程/00-教程索引.md`, or remain as a legacy guide with a clear note pointing new readers to the tutorial set.

## Shared Writing Pattern

Each tutorial should follow this structure:

```text
这篇解决什么问题
先建立通用概念
再解释 ET8 中的关键对象和调用链
然后用本仓库路径举例
最后给新人排查清单
```

The writing should be tutorial-oriented rather than API-reference-oriented. Prefer short conceptual explanations, concrete call chains, and file path examples.

## Topic Coverage

`00-教程索引.md`

Explain the intended reading order, list each document, and map common questions to the right tutorial.

`01-ET项目整体结构.md`

Explain the difference between Unity client, dotnet server, shared code, generated code, config files, hotfix assemblies, and framework reference docs. Use `README.md`, `Unity/`, `DotNet/`, `Share/`, `Config/`, `Book/`, and `Tools/` as examples.

`02-客户端如何连接服务端.md`

Explain the connection path from client startup to router/realm/gate connection. Cover `Session`, router discovery, login to Realm, login to Gate, and how the project stores the usable client session. Use files under `Unity/Assets/Scripts/Hotfix/Client/Agar/NetClient/` and server handlers under `Unity/Assets/Scripts/Hotfix/Server/Agar/Realm/` and `Gate/`.

`03-一次请求的完整链路.md`

Explain how a request leaves the client, is serialized, reaches a server message handler, optionally crosses server scenes with inner messages, then returns a response or notification. Use login, match, player stats, or movement as examples.

`04-新增接口和DTO.md`

Explain where protocol definitions live, how generated message classes are produced, how request/response/message types differ, where handlers go, and what must be recompiled. Use `Unity/Assets/Config/Proto/*.proto`, generated files under `Unity/Assets/Scripts/Model/Generate/`, and custom Agar messages under `Unity/Assets/Scripts/Model/Share/Agar/Message/AgarMessage.cs`.

`05-服务端推送与广播.md`

Explain one-to-one notification, room-level broadcast, and cross-scene delivery. Use `Match2G_AgarMatchSuccess`, `Match2G_AgarBattleState`, `Match2G_AgarBattleResult`, `MessageLocationSenderComponent`, and the Agar room/match systems as examples.

`06-客户端开发入门.md`

Explain creating a new UI window, connecting prefab and ET UI entity, binding `ReferenceCollector`, switching views, scene switching at a conceptual level, and why `ET/Compile` matters. Reuse and reshape the useful content from the existing `Docs/开发指南.md`.

`07-服务端运行模型.md`

Explain that the service runtime is a dotnet process, not Unity Editor. Cover `DotNet/App/Program.cs`, `DotNet/Loader/`, `AppType.Server`, `StartConfig`, process id, server scenes, Fiber creation, and what "Client Server" generated code means.

`08-部署打包与集群运维.md`

Explain how ET start configs define machines, processes, zones, and scenes; how packaging currently works; how startup and shutdown are expected to work; what the watcher process is for; how crashes should be handled; and what can be monitored. Use `Config/Json/s/StartConfig/*`, `Publish-linux-x64.ps1`, and watcher-related server modules as examples.

`Docs/教程/新手教程/09-客户端如何热更.md`

Explain the practical client hot update workflow for newcomers: which assemblies are hotfix assemblies, how `ET/Compile` produces `.dll.bytes`, how the client loads code from `Assets/Bundles/Code`, what HybridCLR contributes, and what changes require rebuilding the app instead of only updating hotfix code. Use `Unity/Assets/Scripts/Loader/CodeLoader.cs`, `Unity/Assets/Scripts/Editor/Assembly/AssemblyTool.cs`, `Unity/Assets/Scripts/Editor/Plugins/HybridCLR/HybridCLREditor.cs`, `Unity/Assets/Bundles/Code/`, and `Unity/Assets/Bundles/AotDlls/` as examples.

`Docs/教程/新手教程/10-服务端如何热更.md`

Explain the practical server hot update workflow: server runtime loads `Hotfix.dll`, reload uses `CodeLoader.Reload()`, `CodeTypes` is rebuilt, and existing long-lived state must be treated carefully. Use `DotNet/Loader/CodeLoader.cs`, `DotNet/Hotfix/DotNet.Hotfix.csproj`, `Unity/Assets/Scripts/Hotfix/Share/Module/Console/ReloadDllConsoleHandler.cs`, `M2A_Reload`/`A2M_Reload`, and server startup files as examples.

`Docs/教程/深入ET原理/00-深入ET原理索引.md`

Explain the purpose of the advanced internals series, list the available advanced topics, and remind readers that these documents explain mechanisms and tradeoffs rather than day-to-day development steps.

`Docs/教程/深入ET原理/01-ETTask与Task.md`

Explain why ET uses `ETTask`, how it fits single-threaded fiber-style async code, what is better than plain `Task`, and what tradeoffs or pitfalls newcomers should know. Use examples from project code where methods return `ETTask`.

`Docs/教程/深入ET原理/02-热更.md`

Explain the underlying hot update model across client and server: stable assemblies vs hotfix assemblies, why ET splits `Model`/`Hotfix` and `ModelView`/`HotfixView`, how reflection-based entry and `CodeTypes` discovery work, why client and server hot update mechanisms are different, and the limits of reloading code when object instances already exist.

## Question Mapping

The user-provided questions should be covered as follows:

```text
客户端与服务端连接如何建立 -> 02
请求从客户端出发、到达服务端、又回到客户端的过程 -> 03
创建新接口、新 DTO -> 04
服务端如何推送通知 -> 05
服务端如何广播 -> 05
ETTask 相比 Task 的优缺点 -> 深入ET原理/01
客户端如何热更 -> 新手教程/09
服务端如何热更 -> 新手教程/10
深入ET原理：热更 -> 深入ET原理/02
如何创建新窗口 -> 06
如何切换场景 -> 06
代码如何生成 -> 04 and 06
"Client Server" 模式如何运行服务端、运行时是否是 Unity -> 07
如何定义集群、打包、部署 -> 08
如何启动、关闭 -> 08
进程崩溃如何处理 -> 08
如何监控集群状态 -> 08
```

## Source Material

Use these files as the main source material:

```text
README.md
Docs/开发指南.md
Unity/Assets/Config/Proto/*.proto
Unity/Assets/Scripts/Model/Share/Agar/Message/AgarMessage.cs
Unity/Assets/Scripts/Model/Generate/
Unity/Assets/Scripts/Hotfix/Client/Agar/NetClient/
Unity/Assets/Scripts/HotfixView/Client/Agar/
Unity/Assets/Scripts/ModelView/Client/Agar/
Unity/Assets/Scripts/Hotfix/Server/Agar/
Unity/Assets/Scripts/Loader/CodeLoader.cs
Unity/Assets/Scripts/Editor/Assembly/AssemblyTool.cs
Unity/Assets/Scripts/Editor/Plugins/HybridCLR/HybridCLREditor.cs
Unity/Assets/Scripts/Hotfix/Share/Module/Console/
Unity/Assets/Scripts/Core/World/Module/Options/Options.cs
Config/Json/s/StartConfig/
DotNet/App/Program.cs
DotNet/Loader/
DotNet/Hotfix/DotNet.Hotfix.csproj
Publish-linux-x64.ps1
Book/
```

`Book/` can be referenced as framework background, but the new tutorials should not simply copy the book. They should explain how to understand and use this type of ET8 project.

## Non-Goals

Do not rewrite the whole ET8 framework manual.

Do not introduce new runtime behavior, new code, or new deployment automation as part of this documentation work.

Do not make the tutorials specific only to Ball Battle. The repository is the running example; the explanation should remain useful to a newcomer working on another ET8 game.

Do not remove existing documentation unless the implementation plan explicitly calls for replacing `Docs/开发指南.md` with a short entry page.

## Verification

After writing the tutorials:

1. Check that every user-provided question maps to at least one tutorial.
2. Check that every tutorial links to the next relevant tutorial or the index.
3. Check that referenced repository paths exist.
4. Run a Markdown/path sanity check with shell commands.
5. Review the text for placeholder words such as `TODO`, `TBD`, and unfinished sections.
