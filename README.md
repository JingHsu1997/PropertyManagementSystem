# 房屋物件管理系統

一個基於 ASP.NET Core MVC 的房屋物件管理系統，提供完整的房屋物件CRUD功能。

## 技術規格

- **框架**: ASP.NET Core 8.0 MVC
- **語言**: C#
- **資料庫**: SQL Server (Docker)
- **ORM**: Dapper
- **測試**: xUnit + Moq
- **架構**: Clean Architecture (分層架構)

## 專案結構

```
PropertyManagementSystem/
├── PropertyManagementSystem.Core/     # 業務邏輯層
│   ├── Models/                        # 實體模型 (Property, PropertyImage)
│   ├── Interfaces/                    # 介面定義
│   └── Services/                      # 業務服務
├── PropertyManagementSystem.Data/     # 資料存取層
│   ├── Repositories/                  # Repository 實作
│   ├── PropertyDbContext.cs          # Dapper 資料存取
│   └── SqlConnectionFactory.cs       # 資料庫連線工廠
├── PropertyManagementSystem.Web/      # 展示層 (MVC)
│   ├── Controllers/                   # MVC 控制器
│   ├── Views/                         # Razor 視圖
│   └── wwwroot/                       # 靜態資源
├── PropertyManagementSystem.Tests/    # 單元測試
├── docker-compose.yml                # Docker 容器配置
└── CreatePropertyDb.sql               # 資料庫初始化腳本
```

## 功能特色

### 房屋物件管理
- ✅ 新增、編輯、刪除房屋物件
- ✅ 房屋物件搜尋與篩選 (城市、區域、類型、狀態、價格範圍)
- ✅ 支援多種物件類型：公寓、透天厝、套房、別墅、辦公室、店面
- ✅ 物件狀態管理：待售、待租、已售、已租、待處理
- ✅ 圖片上傳功能 (URL方式)
- ✅ Modal確認刪除 (無需跳轉頁面)

### 技術特點
- Repository Pattern
- Dependency Injection
- Clean Architecture
- 軟刪除 (Soft Delete)
- 資料庫交易處理
- 單元測試覆蓋

## 環境需求

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022 或 VS Code

## 快速開始

### 1. 啟動系統

```bash
# 啟動 Docker 容器 (資料庫 + Web 應用)
docker-compose up -d --build
```

### 2. 初始化資料庫

```bash
# 複製SQL腳本到容器並執行
docker cp CreatePropertyDb.sql property_management_db:/var/opt/mssql/
docker exec property_management_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Property123!" -C -i /var/opt/mssql/CreatePropertyDb.sql
```

### 3. 訪問應用

開啟瀏覽器，訪問 `http://localhost:5213`

### 4. 執行測試

```bash
# 執行所有單元測試
dotnet test
```

## 使用說明

### 基本操作
1. **首頁** - 查看物件統計和最新物件
2. **物件列表** - 瀏覽和搜尋所有物件
3. **新增物件** - 建立新的房屋物件 (包含圖片URL)
4. **編輯物件** - 修改物件資訊
5. **刪除物件** - 點擊刪除按鈕會彈出確認對話框

### 搜尋功能
- 按城市、區域篩選
- 按物件類型篩選
- 按狀態篩選  
- 設定價格範圍

## 資料庫設定

系統使用 SQL Server 作為資料庫，Docker配置：

```yaml
# docker-compose.yml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: property_management_db
    ports:
      - "1433:1433"
    environment:
      - SA_PASSWORD=Property123!
```

連線字串在 `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PropertyManagement;User=sa;Password=Property123!;TrustServerCertificate=true;"
  }
}
```

## 主要功能路由

- `GET /` - 首頁
- `GET /Properties` - 物件列表 (支援搜尋參數)
- `GET /Properties/Details/{id}` - 物件詳情
- `GET /Properties/Create` - 新增物件表單
- `POST /Properties/Create` - 建立新物件
- `GET /Properties/Edit/{id}` - 編輯物件表單  
- `POST /Properties/Edit/{id}` - 更新物件
- `POST /Properties/Delete` - 刪除物件 (Modal方式)

## 開發指南

### 新增功能

1. 在 `Core/Interfaces` 定義新介面
2. 在 `Core/Services` 實作業務邏輯
3. 在 `Data/Repositories` 實作資料存取
4. 在 `Web/Controllers` 建立控制器
5. 在 `Web/Views` 建立視圖
6. 在 `Tests` 撰寫單元測試

### 資料庫變更

1. 修改 `CreatePropertyDb.sql`
2. 重新執行資料庫初始化腳本

## 測試策略

- **單元測試**: 使用 xUnit 框架測試業務邏輯
- **Mock 框架**: 使用 Moq 進行依賴注入模擬
- **測試範圍**: 涵蓋 PropertyService 的主要功能
- **測試執行**: `dotnet test` 執行所有測試

## 系統狀態

- ✅ **專案已完成建立**
- ✅ **資料庫**: SQL Server Docker容器運行正常
- ✅ **Web應用**: 運行在 http://localhost:5213
- ✅ **單元測試**: 所有測試通過
- ✅ **CRUD功能**: 完整實作包含圖片上傳
- ✅ **UI優化**: 簡化介面，移除教學內容

## 故障排除

### 常見問題

1. **資料庫連線失敗**
   ```bash
   # 檢查Docker容器狀態
   docker ps
   
   # 重啟容器
   docker-compose restart
   ```

2. **編譯錯誤**
   ```bash
   # 停止所有dotnet進程
   taskkill /F /IM dotnet.exe
   
   # 重新建置
   dotnet build
   ```

3. **Port占用**
   ```bash
   # 檢查Port使用狀況
   netstat -ano | findstr :5213
   ```

## 授權

此專案採用 MIT 授權條款
