---
name: update-vbproj-nuget-metadata
overview: 扫描 mzkit 工作区中所有 .vbproj，筛选 RootNamespace 以 BioNovoGene 开头的算法库项目（42 个），根据各项目的 VB.NET 代码内容，用英文重写每个 vbproj 中的 NuGet 描述性元数据：Title、Description、PackageTags、ReleaseNotes。
todos:
  - id: survey-project-code
    content: 使用 [subagent:code-explorer] 扫描 42 个项目的源代码，产出各项目功能要点与关键词清单
    status: completed
  - id: update-assembly-metadata
    content: 依据功能清单重写 assembly 目录 11 个 vbproj 的 Title/Description/Tags/ReleaseNotes
    status: completed
  - id: update-metadb-metadata
    content: 重写 metadb 与 metadna 目录共 11 个 vbproj 的四项元数据字段
    status: completed
  - id: update-mzmath-metadata
    content: 重写 mzmath 目录 12 个 vbproj 的四项元数据字段
    status: completed
  - id: update-visualize-metadata
    content: 重写 visualize 目录 8 个 vbproj 的四项元数据字段
    status: completed
  - id: validate-vbproj-changes
    content: 校验 42 个 vbproj 的 XML 有效性与四字段完整性，确认未改动其他属性
    status: completed
---

## 产品概述

为 mzkit 代谢组学工具箱的基础算法代码库完善 NuGet 包描述性元数据。扫描工作区 g:\mzkit\src 中全部 113 个 .vbproj，筛选出 RootNamespace 以 BioNovoGene 为前缀的项目（排除 2 个单元测试项目和 mzkit_win32 主程序后共 42 个），深入阅读各项目的 VB.NET 源代码内容，据此用英文重写每个 vbproj 中的四个 NuGet 元数据字段。

## 核心功能

- 扫描并过滤 RootNamespace 以 BioNovoGene 开头的 42 个算法库项目（assembly 11 个、metadb 9 个、metadna 2 个、mzmath 12 个、visualize 8 个）
- 通读各项目源代码，提炼其核心功能、算法特性与适用场景
- 为每个 vbproj 重写 Title（包标题）、Description（功能描述）、PackageTags（关键词标签）、ReleaseNotes（基于代码内容生成的功能简介）
- 全部使用英文编写，遵循 NuGet 国际惯例，与现有元数据风格一致
- 全量重写策略：四个字段均覆盖现有值
- 编辑过程保持 vbproj 原有 XML 结构、缩进风格及 Version、OutputPath、Configuration 等其他属性不变

## 技术方案

### 实现方式

- vbproj 为 SDK 风格 MSBuild XML 文件，元数据位于第一个 PropertyGroup（参考 mzmath/ms2_math-core/mzmath-netcore5.vbproj 第 21-24 行的 Title/Description/PackageTags）
- 每个项目先浏览其目录下的 .vb 源文件（模块/类/命名空间命名、模块注释、核心算法类型），提炼功能摘要，再定位 vbproj 中的既有元数据行进行改写；缺失的字段（如 PackageTags、ReleaseNotes）在第一个 PropertyGroup 内相邻位置新增
- 42 个项目按目录分组批量处理（assembly / metadb+metadna / mzmath / visualize），控制上下文切换
- 借助子代理并行完成"读代码→归纳功能要点"的探索步骤，主流程专注元数据写入

### 执行细节

- 只改 Title、Description、PackageTags、ReleaseNotes 四个字段；不触碰 Version、OutputPath、DefineConstants、Condition PropertyGroup 等任何其他内容
- PackageTags 采用逗号分隔的小写关键词（如 mzkit, mass-spectrometry, lipidomics），沿用现有 "mzkit, mzpack" 风格
- ReleaseNotes 为 2-4 句英文功能简介，概括包的核心算法与特性
- 存在重复 RootNamespace 的项目（如 KNApSAcK 的 .NET5 与旧式两个 vbproj）两个文件都需更新
- 完成后用 XML 解析校验每个被修改文件的格式有效性，并复核 42 个文件四个字段均已填写

### 性能与可靠性

- 逐目录批量处理，避免 113 个文件的冗余扫描；已排除非 BioNovoGene 前缀的约 68 个项目与 3 个明确排除项
- 编辑采用精确文本替换，保持混合缩进（空格/Tab）原样，降低破坏 MSBuild 文件的风险

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose：批量探索 42 个项目的 VB.NET 源代码，按目录分组归纳每个项目的核心功能、主要类/模块与算法特性，为元数据撰写提供依据
- Expected outcome：产出每个项目的功能要点清单（项目路径 → 核心功能描述 → 建议关键词），覆盖 assembly、metadb、metadna、mzmath、visualize 全部待更新项目