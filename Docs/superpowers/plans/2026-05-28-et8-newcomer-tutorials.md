# ET8 Newcomer Tutorials Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the approved ET8 newcomer tutorial set and the separately numbered advanced ET internals tutorial set under `Docs/教程/`.

**Architecture:** Documentation is split into two series: `Docs/教程/新手教程/` for practical newcomer workflows and `Docs/教程/深入ET原理/` for mechanism-level explanations. `Docs/开发指南.md` becomes a short entry page pointing readers to the new index instead of remaining the primary long-form tutorial.

**Tech Stack:** Markdown documentation, existing ET8 C# source files as examples, PowerShell verification commands.

---

### Task 1: Create Tutorial Indexes And Entry Page

**Files:**
- Create: `Docs/教程/新手教程/00-教程索引.md`
- Create: `Docs/教程/深入ET原理/00-深入ET原理索引.md`
- Modify: `Docs/开发指南.md`

- [ ] **Step 1: Create the newcomer tutorial index**

Create `Docs/教程/新手教程/00-教程索引.md` with the reading order, the two-series distinction, and the user question mapping from the approved design.

- [ ] **Step 2: Create the advanced internals index**

Create `Docs/教程/深入ET原理/00-深入ET原理索引.md` explaining that these documents cover mechanisms and tradeoffs rather than day-to-day steps.

- [ ] **Step 3: Replace `Docs/开发指南.md` with an entry page**

Keep it short and point new readers to `Docs/教程/新手教程/00-教程索引.md`; preserve a note that the old UI material has been reorganized into the client development tutorial.

- [ ] **Step 4: Verify index links**

Run: `Select-String -Path 'Docs\教程\**\*.md','Docs\开发指南.md' -Pattern '\]\(([^)]+)\)'`

Expected: All local Markdown links point to files planned in this document.

### Task 2: Write Core Newcomer Tutorials 01-05

**Files:**
- Create: `Docs/教程/新手教程/01-ET项目整体结构.md`
- Create: `Docs/教程/新手教程/02-客户端如何连接服务端.md`
- Create: `Docs/教程/新手教程/03-一次请求的完整链路.md`
- Create: `Docs/教程/新手教程/04-新增接口和DTO.md`
- Create: `Docs/教程/新手教程/05-服务端推送与广播.md`

- [ ] **Step 1: Write project structure tutorial**

Explain `Unity/`, `DotNet/`, `Share/`, `Config/`, `Book/`, `Tools/`, generated code, hotfix assemblies, and why the repository is only the example.

- [ ] **Step 2: Write client connection tutorial**

Explain Router, Realm, Gate, Session, login key, and usable session storage using `Unity/Assets/Scripts/Hotfix/Client/Agar/NetClient/` and server Realm/Gate handlers as examples.

- [ ] **Step 3: Write request lifecycle tutorial**

Explain request serialization, `Session.Call`, server handler dispatch, inner messages, response return, and notification handling using login, match, stats, and movement examples.

- [ ] **Step 4: Write new interface and DTO tutorial**

Explain proto files, generated files, custom message files, request/response/message interfaces, handler placement, and required compile/regeneration steps.

- [ ] **Step 5: Write push and broadcast tutorial**

Explain one-to-one push, room broadcast, cross-scene delivery, and where Agar sends match success, battle state, and result messages.

- [ ] **Step 6: Verify coverage for core questions**

Run: `Select-String -Path 'Docs\教程\新手教程\0[1-5]-*.md' -Pattern 'Session|Router|Realm|Gate|proto|DTO|推送|广播|MessageLocationSender|Agar'`

Expected: Each term appears in the relevant tutorial.

### Task 3: Write Practical Newcomer Tutorials 06-10

**Files:**
- Create: `Docs/教程/新手教程/06-客户端开发入门.md`
- Create: `Docs/教程/新手教程/07-服务端运行模型.md`
- Create: `Docs/教程/新手教程/08-部署打包与集群运维.md`
- Create: `Docs/教程/新手教程/09-客户端如何热更.md`
- Create: `Docs/教程/新手教程/10-服务端如何热更.md`

- [ ] **Step 1: Write client development tutorial**

Reuse useful material from the old `Docs/开发指南.md`: UI creation, prefab binding, `ReferenceCollector`, UI lifecycle, scene switching concept, code generation and `ET/Compile`.

- [ ] **Step 2: Write server runtime tutorial**

Explain dotnet runtime, `DotNet/App/Program.cs`, `DotNet/Loader/`, `AppType.Server`, `StartConfig`, processes, scenes, fibers, and what `ClientServer` generated code means.

- [ ] **Step 3: Write deployment and cluster tutorial**

Explain start config tables, machines, processes, zones, scenes, publish script, startup, shutdown, watcher, crash handling, and monitoring from a newcomer perspective.

- [ ] **Step 4: Write client hot update tutorial**

Explain HybridCLR, hotfix assemblies, `.dll.bytes`, `CodeLoader`, AOT metadata, `ET/Compile`, and rebuild boundaries.

- [ ] **Step 5: Write server hot update tutorial**

Explain `Hotfix.dll`, `AssemblyLoadContext`, `CodeLoader.Reload()`, `CodeTypes`, console reload handler, and state caveats.

- [ ] **Step 6: Verify practical tutorial terms**

Run: `Select-String -Path 'Docs\教程\新手教程\0[6-9]-*.md','Docs\教程\新手教程\10-*.md' -Pattern 'ReferenceCollector|dotnet|StartConfig|Watcher|HybridCLR|CodeLoader|Reload|ET/Compile'`

Expected: Each practical topic appears in the relevant tutorial.

### Task 4: Write Advanced Internals Tutorials

**Files:**
- Create: `Docs/教程/深入ET原理/01-ETTask与Task.md`
- Create: `Docs/教程/深入ET原理/02-热更.md`

- [ ] **Step 1: Write ETTask vs Task tutorial**

Explain ETTask's fit with ET fibers, allocation and scheduling goals, advantages, tradeoffs, and newcomer pitfalls.

- [ ] **Step 2: Write hot update internals tutorial**

Explain stable vs hotfix assemblies, `Model`/`Hotfix` and `ModelView`/`HotfixView`, client vs server hot update mechanism differences, `CodeTypes`, reflection entry, and reload limits.

- [ ] **Step 3: Verify advanced terms**

Run: `Select-String -Path 'Docs\教程\深入ET原理\*.md' -Pattern 'ETTask|Task|Fiber|CodeTypes|AssemblyLoadContext|HybridCLR|Model|Hotfix'`

Expected: The mechanism terms appear in the relevant advanced tutorials.

### Task 5: Final Documentation Verification

**Files:**
- Inspect: `Docs/教程/**/*.md`
- Inspect: `Docs/开发指南.md`

- [ ] **Step 1: Check all planned files exist**

Run: `Test-Path` for every planned Markdown file.

Expected: every command prints `True`.

- [ ] **Step 2: Check no placeholder markers remain**

Run: `Select-String -Path 'Docs\教程\**\*.md','Docs\开发指南.md' -Pattern 'TODO|TBD|待补|占位'`

Expected: no output.

- [ ] **Step 3: Check user question coverage**

Run: `Select-String -Path 'Docs\教程\**\*.md' -Pattern '客户端如何连接服务端|一次请求|新增接口|DTO|推送|广播|创建新窗口|切换场景|服务端运行|集群|崩溃|监控|客户端如何热更|服务端如何热更|ETTask'`

Expected: all requested topic groups have at least one matching document.

- [ ] **Step 4: Review git diff scope**

Run: `git diff -- Docs/开发指南.md Docs/教程 Docs/superpowers/plans/2026-05-28-et8-newcomer-tutorials.md`

Expected: only documentation files from this plan are changed.
