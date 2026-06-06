USE master;

CREATE DATABASE Northwind;
GO

USE Northwind;
GO

-- 2. 建立關聯表：供應商 (Suppliers)
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName NVARCHAR(40) NOT NULL,
    ContactName NVARCHAR(30) NULL
);

-- 3. 建立關聯表：類別 (Categories)
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(15) NOT NULL,
    [Description] NTEXT NULL
);

-- 4. 建立核心商品表 (Products) - 帶有 FK 關聯，完美模擬真實考情
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(40) NOT NULL,
    SupplierID INT NULL FOREIGN KEY REFERENCES Suppliers(SupplierID),
    CategoryID INT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    QuantityPerUnit NVARCHAR(20) NULL,
    UnitPrice MONEY DEFAULT 0,
    UnitsInStock SMALLINT DEFAULT 0
);
GO

-- 5. 灌入標準測試資料
INSERT INTO Suppliers (CompanyName, ContactName) VALUES 
(N'Exotic Liquids', N'Charlotte Cooper'),
(N'New Orleans Cajun Delights', N'Shelley Burke'),
(N'Grandma Kelly''s Homestead', N'Regina Murphy');

INSERT INTO Categories (CategoryName, [Description]) VALUES 
(N'Beverages', N'Soft drinks, coffees, teas, beers, and ales'),
(N'Condiments', N'Sweet and savory sauces, relishes, spreads, and seasonings'),
(N'Confections', N'Desserts, candies, and sweet breads');

INSERT INTO Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock) VALUES
(N'Chai', 1, 1, N'10 boxes x 20 bags', 18.00, 39),
(N'Chang', 1, 1, N'24 - 12 oz bottles', 19.00, 17),
(N'Aniseed Syrup', 1, 2, N'12 - 550 ml bottles', 10.00, 13),
(N'Chef Anton''s Cajun Seasoning', 2, 2, N'48 - 6 oz jars', 22.00, 53),
(N'Chef Anton''s Gumbo Mix', 2, 2, N'36 boxes', 21.35, 0),
(N'Grandma''s Boysenberry Spread', 3, 2, N'12 - 8 oz jars', 25.00, 120),
(N'Uncle Bob''s Organic Dried Pears', 3, 3, N'12 - 1 lb pkgs', 30.00, 15),
(N'蘋果汁 (Apple Juice)', 1, 1, N'24 - 12 oz cans', 15.50, 100);
GO

-- 驗證成果
SELECT p.ProductID, p.ProductName, c.CategoryName, s.CompanyName, p.UnitPrice
FROM Products p
LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID;