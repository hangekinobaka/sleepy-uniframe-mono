# Sleepy Uniframe Mono

> A monorepo of Unity features and utilities for personal use across projects.

这个仓库是我个人的 Unity 功能模块集合，主要用于在多个项目中通过 UPM 引用和复用。  
This repository is a personal monorepo of reusable Unity modules, designed for cross-project integration via Unity Package Manager.

本仓库是公开的，欢迎浏览与引用，但**不接受任何形式的贡献（包括 Pull Request / Issue）**。  
The repository is **publicly visible and open for reference**, but **does not accept any form of contribution (Pull Requests or Issues are disabled)**.

若某个模块未来适合社区共建，我将会**单独拆出仓库公开维护**。  
If any specific package becomes suitable for collaborative development, it will **be migrated to a standalone public repo**.

## 📦 使用方法 | How to Use

直接通过 `manifest.json` 添加引用，例如：  
Install a package directly via `manifest.json`, like this:
```json
"com.sleepy.feature": "https://github.com/yourname/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono_unity/Assets/[feature_path]"   
```

**About Demo:**    
我不喜欢 Unity 的脑残 Samples~ 系统。
想要试用 demo 场景？请直接把 Package 中的 Demo 复制到你项目的 `Assets` 目录里。

I don’t like Unity’s stupid Samples~ system — it’s clunky and annoying to work with.
If you want to check out the demo scene, just copy the Demo from the Package into your project’s `Assets` directory.

## 📚 当前提供的包列表 | Available Packages

以下是目前可用的模块包路径（持续更新）：
The following packages are available in this repository (keep updated):


| 模块名 / Module        | 功能简介 / Description                                                                                                       | 包名 / Package Name        | 依赖 / Dependencies | Git 地址 / Git URL                                                                                                                                                                                                                |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------- | -------------------------- | ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `sleepy-unicore`       | 核心通用模块，通常无需单独引用。<br>Universal core utilities for the Sleepy Unity packages.                                  | `com.sleepy.unicore`       | None           | [https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono\_unity/Assets/unicore](https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono_unity/Assets/unicore)             |
| `sleepy-unicore-async` | 带 Async 功能的核心模块，依赖 UniRx 与 UniTask。<br>Universal core utilities with async support. Requires UniRx and UniTask. | `com.sleepy.unicore-async` | UniRx, UniTask      | [https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono\_unity/Assets/unicore-async](https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono_unity/Assets/unicore-async) |
| `sleepy-ui-tab`        | 快速搭建 Tab UI 的组件模块。<br>A lightweight module for quickly building Tab UI.                                            | `com.sleepy.ui.tab`        | UniRx, UniTask, sleepy-unicore, sleepy-unicore-async             | [https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono\_unity/Assets/ui/tab](https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono_unity/Assets/ui/tab)               |
