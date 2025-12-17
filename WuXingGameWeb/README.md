# WuXing Game (Unity + ASP.NET Core 复刻)

## 项目概述
将网页版五行财富之海游戏完整复刻为  
- **前端**：Unity 6.2 + UGUI + Text（非 TMP）  
- **后端**：ASP.NET Core 10 + Swagger + WebSocket  
- **数据库**：MongoDB（端口 27018）  

## 快速开始

### 1. 启动 MongoDB
```bash
mongod --port 27018 --dbpath ./db
```

### 2. 启动后端
```bash
cd WuXingGameBackend
dotnet run
```
浏览器访问：  
- Swagger UI：https://localhost:5001/swagger  
- WebSocket 地址：wss://localhost:5001/api/UnityWebSocketAdapter/ws  

### 3. 运行 Unity
- 打开 Unity 2023 LTS（或 6.2）  
- 菜单栏 Tools → Create Battle UI → Generate Full UI  
- 运行场景，自动连接后端进行测试

## 核心接口
| 方法 | 路径 | 说明 |
|---|---|---|
| POST | /api/Player/init | 初始化玩家 |
| PUT  | /api/Player/element | 消耗元素 |
| POST | /api/UnityWebSocketAdapter/element/select | Unity 元素选择 |

## WebSocket 事件
- `ReceiveBattleResult` 战斗结果推送  
- `ElementSelected` 元素选择广播  

## 项目结构
```
WuXingGameBackend/
 ├─ Program.cs                 // net10 + Swagger + SignalR
 ├─ Hubs/GameHub.cs            // WebSocket 逻辑
 ├─ Models/Player.cs           // MongoDB 实体
 ├─ Controllers/
 │   ├─ PlayerController.cs    // REST API
 │   └─ UnityWebSocketAdapter.cs // Unity 适配
UnityEditorScripts/             // 自动生成 UGUI 脚本
UnityScripts/BattleAreaController.cs // 前端核心逻辑
docs/                           // 设计文档
```

## 下一步
- 完善战斗结算逻辑  
- 接入 Unity 真实网络模块  
- 增加自动化测试脚本  

Enjoy hacking！
