# OSUI — 操作系统算法模拟平台

一款基于 WPF 的桌面应用程序，用于可视化演示操作系统课程中常见的核心算法，帮助学生直观理解算法的运行过程。

---

## ✨ 功能模块

### 🔀 调度算法（Scheduler）
- 支持 **FCFS**（先来先服务）和 **SJFS**（短作业优先）调度策略
- 支持整数时间 / HH:MM 两种输入格式
- 自动计算周转时间、带权周转时间及其平均值
- 提供示例数据一键加载

### 🏦 银行家算法（Banker's Algorithm）
- 可自定义进程数量与资源种类数
- 动态编辑已分配矩阵、最大需求矩阵和可用资源向量
- 安全性检查及安全序列展示
- 支持模拟资源请求并验证是否会导致不安全状态

### 💽 磁盘寻道算法（Disk Seek）
- 支持 **SSTF**、**SCAN**、**C-SCAN** 三种寻道算法
- 可设置起始磁道位置与请求队列
- Canvas 绘制寻道路径图，含方向箭头和访问顺序标注
- 实时统计总寻道长度与平均寻道长度

### 📄 页面置换算法（Page Replacement）
- 支持 **FIFO**、**LRU**、**OPT** 三种置换策略
- 自定义页面访问序列与物理块数
- 逐步演示：上一步 / 下一步 / 自动播放
- 每个步骤卡片高亮显示命中（绿色）或缺页（红色），当前步骤橙色边框
- 统计总页数、缺页次数与缺页率

---

## 🛠️ 技术栈

| 技术 | 用途 |
|------|------|
| **.NET 8 (WPF)** | 桌面应用框架 |
| **C# 14** | 编程语言 |
| **CommunityToolkit.Mvvm 8.4** | MVVM 架构支持（`[ObservableProperty]`、`[RelayCommand]`） |
| **MaterialDesignInXaml 5.3** | UI 主题与组件库 |
| **HandyControl 3.5** | 辅助 UI 组件 |
| **Microsoft.Extensions.DependencyInjection** | 依赖注入容器 |

---

## 📁 项目结构

```
OSUI/
├── Views/
│   ├── Windows/        # LoginWindow、MainWindow、RegisterWindow 等
│   └── Pages/          # 各算法功能页面
├── ViewModels/         # MVVM ViewModel（CommunityToolkit.Mvvm）
├── Models/             # 数据模型（Process、StepRecord、DiskSeekVector 等）
├── Services/           # 业务服务（Auth、Navigation、Localization、算法引擎等）
├── Resources/
│   └── Localization/   # Strings.zh-CN.xaml / Strings.en-US.xaml
├── Extensions/         # DI 扩展、向量扩展
├── Converters/         # 值转换器
├── Messages/           # MVVM 消息
└── Data/               # 常量与配置
```

---

## 🎨 界面特性

- **明暗主题切换** — 一键切换 Light / Dark 主题，演示区颜色同步适配
- **中英文双语** — 全界面本地化，语言切换后即时生效
- **自定义字体** — 支持系统字体自由切换
- **用户系统** — 登录 / 注册 / 修改密码，支持访客模式浏览
- **偏好持久化** — 主题、语言、字体等设置通过 JSON 文件持久存储

---

## 🚀 快速开始

### 环境要求
- Windows 10 / 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 编译运行

```powershell
# 克隆项目
git clone https://github.com/TickZhong/OSUI.git

cd OSUI

# 还原依赖并编译
dotnet restore
dotnet build

# 运行
dotnet run --project OSUI
```

### 发布

```powershell
dotnet publish OSUI -c Release -o ./publish
```

---

## 🤝 贡献

欢迎提交 Issue 或 Pull Request，共同完善本项目。

---

## 📄 License

© 2024 TickZhong. 本项目仅供学习与教学使用。
