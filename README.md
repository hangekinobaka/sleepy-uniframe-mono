# Sleepy Uniframe Mono

> A monorepo of Unity features and utilities for personal use across projects.

这个仓库是我个人的 Unity 功能模块集合，主要用于在多个项目中通过 UPM 引用和复用。  
This repository is a personal monorepo of reusable Unity modules, designed for cross-project integration via Unity Package Manager.

本仓库是公开的，欢迎浏览与引用，但**不接受任何形式的贡献（包括 Pull Request / Issue）**。  
The repository is **publicly visible and open for reference**, but **does not accept any form of contribution (Pull Requests or Issues are disabled)**.

若某个模块未来适合社区共建，我将会**单独拆出仓库公开维护**。  
If any specific package becomes suitable for collaborative development, it will **be migrated to a standalone public repo**.

## 📦 使用方法 | How to Use

可以直接通过 `manifest.json` 添加引用，例如：  
Install a package directly via `manifest.json`, like this:
```json
"com.sleepy.feature": "https://github.com/yourname/sleepy-uniframe-mono.git?path=sleepy-uniframe-mono_unity/Assets/[feature_path]"   
```

## 📚 当前提供的包列表 | Available Packages

以下是目前可用的模块包路径（持续更新）：
The following packages are available in this repository (keep updated):


| 模块名 / Module        | 功能简介 / Description                                                                                                       | 包名 / Package Name        | 依赖 / Dependencies | Git 地址 / Git URL                                                                                                                                                                                                                |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------- | -------------------------- | ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `sleepy-unicore`       | 核心通用模块，通常无需单独引用。<br>Universal core utilities for the Sleepy Unity packages.                                  | `com.sleepy.unicore`       | None           | https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=packages/Sleepy_Unicore             |
| `sleepy-ui`        | Sleepy UI 模块<br>The sleepy UI module.                                            | `com.sleepy.ui`        | UniRx, UniTask, sleepy-unicore             | https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=packages/Sleepy_UI               |
| `sleepy-loading`        | Asset 和 Scene 的异步读取模块<br>Tools for async asset and scene loading.                                            | `com.sleepy.loading`        | Addressable, UniRx, UniTask, sleepy-unicore           | https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=packages/Sleepy_Loading               |
| `sleepy-routing`        | 一个支持类似网页导航的 Unity UI 路由机制<br>A webpage-like routing system for the Unity UI.                                           | `com.sleepy.routing`        | UniTask, sleepy-unicore, sleepy-loading             | https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=packages/Sleepy_Routing               |
| `sleepy-timer` | 计时器功能模块 <br>Timer Functionality Module | `com.sleepy.timer` | UniRx, UniTask, sleepy-unicore      | https://github.com/hangekinobaka/sleepy-uniframe-mono.git?path=packages/Sleepy_Timer |


## Other Dependencies


这些是开发时用到的版本，你可以自行使用更新版本，不过可能会有不知名风险      
These are the versions used during development.   
You may use newer versions at your own discretion, but doing so may introduce unknown risks.   

- UniRx Ver 7.1.0 - https://github.com/neuecc/UniRx/releases/tag/7.1.0
- UniTask Ver.2.5.0 - https://github.com/Cysharp/UniTask/releases/tag/2.5.0
- DOTween v1.2.765(February 4, 2024) - https://dotween.demigiant.com/download.php