-- 建立資料庫
IF DB_ID('PropertyManagement') IS NULL
BEGIN
    CREATE DATABASE [PropertyManagement];
END
GO

USE [PropertyManagement];
GO

-- Lookup tables
CREATE TABLE dbo.PropertyTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.PropertyStatuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

-- 主檔
CREATE TABLE dbo.Properties (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    City NVARCHAR(100) NULL,
    District NVARCHAR(100) NULL,
    Address NVARCHAR(300) NULL,
    TypeId INT NULL,
    StatusId INT NULL,
    Price DECIMAL(18,2) NULL,
    Area DECIMAL(10,2) NULL,
    Bedrooms TINYINT NULL,
    Bathrooms TINYINT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Properties_PropertyTypes FOREIGN KEY (TypeId) REFERENCES dbo.PropertyTypes(Id),
    CONSTRAINT FK_Properties_PropertyStatuses FOREIGN KEY (StatusId) REFERENCES dbo.PropertyStatuses(Id)
);

-- 圖片
CREATE TABLE dbo.PropertyImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PropertyId INT NOT NULL,
    ImageUrl NVARCHAR(1000) NOT NULL,
    AltText NVARCHAR(200) NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PropertyImages_Properties FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(Id) ON DELETE CASCADE
);

-- 使用者（如需）
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    DisplayName NVARCHAR(200) NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Indexes: 常用查詢索引
CREATE INDEX IX_Properties_City_District ON dbo.Properties (City, District);
CREATE INDEX IX_Properties_Price ON dbo.Properties (Price);
CREATE INDEX IX_Properties_IsDeleted ON dbo.Properties (IsDeleted);

-- 最小種子資料
INSERT INTO dbo.PropertyTypes (Name) VALUES ('公寓'),('透天'),('電梯大樓');
INSERT INTO dbo.PropertyStatuses (Name) VALUES ('待售'),('已售'),('出租');

INSERT INTO dbo.Properties (Title, City, District, Address, TypeId, StatusId, Price, Area, Bedrooms, Bathrooms, Description)
VALUES
('近捷運3房美寓', '台北市','中山區','中山路123號', 1, 1, 15800000.00, 45.5, 3, 2, '屋況佳，近學校和捷運'),
('花園透天', '台中市','西屯區','文心路456號', 2, 1, 38000000.00, 120.0, 4, 3, '附小花園與車庫');

INSERT INTO dbo.PropertyImages (PropertyId, ImageUrl, AltText, SortOrder)
VALUES
(1, '/images/property1-1.jpg', '客廳', 1),
(1, '/images/property1-2.jpg', '廚房', 2),
(2, '/images/property2-1.jpg', '建築外觀', 1);
