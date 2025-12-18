# WuXing Game Unity版本

这是基于Web版本开发的Unity网络游戏版本，实现了五行探险记的核心游戏机制。

## 项目结构

```
Assets/
├── UnityScripts/           # 核心游戏脚本
│   ├── Enums.cs           # 枚举定义
│   ├── Models.cs          # 数据模型
│   ├── GameUtils.cs       # 游戏工具函数
│   ├── GameManager.cs     # 游戏管理器
│   ├── BattleSystem.cs    # 战斗系统
│   ├── UIManager.cs       # UI管理系统
│   ├── UIController.cs    # UI控制器
│   ├── WebSocketClient.cs # WebSocket客户端
│   ├── JsonHelper.cs     # JSON助手类
│   └── GameTester.cs     # 测试脚本
├── UnityEditorScripts/    # 编辑器脚本
│   ├── SetupGameUI.cs    # UI设置工具
│   └── ...               # 其他编辑器脚本
└── Main.unity            # 主场景
```

## 核心功能

### 1. 五行元素系统
- 金、木、水、火、土五种元素
- 元素之间存在相克关系：金克木、木克土、土克水、水克火、火克金
- 战斗中应用克制关系影响对手元素数量

### 2. 战斗系统
- 5回合制战斗
- 玩家选择元素 vs 对手元素
- 应用五行相克规则
- 需要3胜以上才能获胜

### 3. 货币系统
- 阴阳两种货币
- 冒险成功后随机获得
- 失败时可用于扭转战局

### 4. 移动机制
- 随机四个对角方向移动
- 基于曼哈顿距离计算难度
- 距离越远对手越强奖励越丰厚

## 设置步骤

### 1. 创建Canvas和基本UI
1. 在Unity编辑器中，点击菜单 `Tools` → `WuXing Game` → `Setup Game UI`
2. 选择 `Setup All` 按钮创建主Canvas和游戏组件

### 2. 配置游戏组件
1. 确保场景中有GameManager对象
2. 为GameManager对象添加以下组件：
   - GameManager
   - BattleSystem
   - WebSocketClient
   - UIController
   - UIManager

### 3. 运行游戏
1. 点击Play按钮运行游戏
2. 使用以下按键进行测试：
   - `B` 键：开始战斗
   - `R` 键：补充元素
   - `Space` 键：连接WebSocket服务器

## 网络功能

### WebSocket连接
- 连接到后端SignalR Hub
- 支持实时多人游戏功能
- 实现玩家加入、元素选择等操作

### 后端集成
- 与WuXingGameBackend项目集成
- 使用SignalR实现实时通信
- 支持MongoDB数据存储

## 测试说明

### GameTester组件
- 自动测试游戏工具函数
- 验证游戏状态管理
- 测试元素选择功能
- 键盘快捷键测试

### 手动测试流程
1. 启动游戏
2. 点击"开始探险"按钮
3. 选择玩家元素
4. 揭示对手元素
5. 完成5回合战斗
6. 查看战斗结果
7. 测试货币系统
8. 测试返回出生点功能

## 开发注意事项

### 代码规范
- 使用C#编写所有游戏逻辑
- 遵循Unity组件设计模式
- 保持代码结构清晰易维护

### 扩展性考虑
- 易于添加新的元素类型
- 支持更多战斗机制
- 可扩展技能系统
- 支持成就和排行榜功能

## 故障排除

### 常见问题
1. **WebSocket连接失败**
   - 检查后端服务是否运行
   - 确认服务器地址配置正确
   - 检查防火墙设置

2. **UI显示异常**
   - 确保Canvas设置正确
   - 检查UI组件引用
   - 验证RectTransform设置

3. **游戏逻辑错误**
   - 检查GameState初始化
   - 验证元素数量计算
   - 确认战斗结果判定

## 版本信息

当前版本：v1.0.0
基于Web版本：v1.3.0

## 许可证

本项目仅供学习和参考使用。