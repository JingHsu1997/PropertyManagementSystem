# 房屋物件管理系統

一個基於 ASP.NET Core MVC 的房屋物件管理系統，參考永慶房屋網站功能設計。

## 技術規格

- **框架**: ASP.NET Core 8.0 MVC
- **語言**: C#
- **資料庫**: SQL Server (Docker)
- **ORM**: Entity Framework Core
- **測試**: xUnit + Moq
- **架構**: Clean Architecture (分層架構)

## 專案結構

```
PropertyManagementSystem/
├── PropertyManagementSystem.Core/     # 業務邏輯層
│   ├── Models/                        # 實體模型
│   ├── Interfaces/                    # 介面定義
│   └── Services/                      # 業務服務
├── PropertyManagementSystem.Data/     # 資料存取層
│   ├── Repositories/                  # Repository 實作
│   └── PropertyDbContext.cs          # EF Core DbContext
├── PropertyManagementSystem.Web/      # 展示層 (MVC)
│   ├── Controllers/                   # MVC 控制器
│   ├── Views/                         # Razor 視圖
│   └── Models/                        # 視圖模型
└── PropertyManagementSystem.Tests/    # 單元測試
```

## 功能特色

### 房屋物件管理
- 新增、編輯、刪除房屋物件
- 房屋物件搜尋與篩選
- 支援多種物件類型：公寓、透天厝、套房、別墅、辦公室、店面
- 物件狀態管理：待售、待租、已售、已租、待處理

### 搜尋功能
- 按城市搜尋
- 按區域搜尋
- 按物件類型篩選
- 按狀態篩選
- 價格範圍篩選

### 技術特點
- Repository Pattern
- Dependency Injection
- Clean Architecture
- 軟刪除 (Soft Delete)
- 單元測試覆蓋

## 環境需求

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022 或 VS Code

## 快速開始

### 1. 啟動資料庫

```bash
# 啟動 SQL Server Docker 容器
docker-compose up -d
```

### 2. 建立資料庫

```bash
# 進入 Web 專案目錄
cd PropertyManagementSystem.Web

# 建立 Migration
dotnet ef migrations add InitialCreate --project ../PropertyManagementSystem.Data

# 更新資料庫
dotnet ef database update --project ../PropertyManagementSystem.Data
```

### 3. 執行應用程式

```bash
# 執行 Web 應用程式
dotnet run --project PropertyManagementSystem.Web
```

### 4. 執行測試

```bash
# 執行所有單元測試
dotnet test
```

## 使用說明

1. 開啟瀏覽器，訪問 `https://localhost:5001`
2. 點擊導航欄中的「房屋物件」進入物件管理頁面
3. 使用搜尋表單篩選物件
4. 點擊「新增物件」建立新的房屋物件
5. 點擊「詳細資料」查看物件詳情
6. 點擊「編輯」修改物件資訊
7. 點擊「刪除」移除物件

## 資料庫設定

系統使用 SQL Server 作為資料庫，連線字串設定在 `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PropertyManagementDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  }
}
```

## API 文件

系統提供以下主要功能：

### 房屋物件管理
- `GET /Properties` - 取得房屋物件列表
- `GET /Properties/Details/{id}` - 取得特定物件詳情
- `GET /Properties/Create` - 顯示新增物件表單
- `POST /Properties/Create` - 建立新物件
- `GET /Properties/Edit/{id}` - 顯示編輯物件表單
- `POST /Properties/Edit/{id}` - 更新物件
- `GET /Properties/Delete/{id}` - 顯示刪除確認頁面
- `POST /Properties/Delete/{id}` - 刪除物件

## 開發指南

### 新增功能

1. 在 Core 層定義新的介面和服務
2. 在 Data 層實作 Repository
3. 在 Web 層建立控制器和視圖
4. 撰寫對應的單元測試

### 資料庫變更

```bash
# 建立新的 Migration
dotnet ef migrations add <MigrationName> --project PropertyManagementSystem.Data

# 套用 Migration
dotnet ef database update --project PropertyManagementSystem.Data
```

## 測試策略

- 單元測試使用 xUnit 框架
- 使用 InMemory 資料庫進行測試
- 使用 Moq 框架進行依賴注入模擬
- 測試覆蓋率包含服務層和資料存取層

## 部署注意事項

### 生產環境設定
1. 更新連線字串指向正式資料庫
2. 設定適當的日誌等級
3. 啟用 HTTPS
4. 設定適當的快取策略

### Docker 部署
```bash
# 建置 Docker 映像
docker build -t property-management-system .

# 執行容器
docker run -p 5000:80 property-management-system
```

## 貢獻指南

1. Fork 此專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

## 授權

此專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 聯絡資訊

如有任何問題或建議，請聯絡：
- Email: [your-email@example.com]
- GitHub: [your-github-username]
