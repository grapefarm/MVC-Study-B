# ASP.NET Core MVC 核心架構演進與技術實作展示

本專案旨在展示 **ASP.NET Core MVC** 開發中的架構演進、代碼重構（Refactoring）以及 Git 分支控管實務。
專案基於微軟官方工具反向工程（Reverse Engineering）生成的 CRUD 基礎，透過漸進式重構（Progressive Refactoring），逐步演進為符合單一職責原則、前後端解耦且具備依賴注入的企業級分層架構。

---

## 🌿 Git 分支規劃與架構演進說明

本倉庫透過 Git Flow 概念切分為四個核心階段（Branches），每個分支代表不同的技術實作焦點與架構維度：

### 1. 基礎關鍵字檢索
* **分支名稱：** `feature/basic-search`
* **實作內容：**
  * 配置資料庫連線字串（`DefaultConnection`）並打通資料存取層。
  * 利用 Entity Framework Core 建立資料上下文（DbContext）與 Data Models。
  * 於 `ProductsController/Index` 實作基礎的單一關鍵字條件篩選，完成資料流驗證。
* **技術焦點：** 基礎 LINQ 查詢與 Controller 路由配置。

### 2. 多條件複合篩選與狀態保留排序
* **分支名稱：** `feature/advanced-filter-sorting`
* **實作內容：**
  * **複合篩選：** 擴充 UI 表單為「關鍵字輸入框 + 分類下拉選單」之網格佈局，支援多重條件併發查詢。
  * **狀態保留：** 運用 `ViewBag` 與 `SelectList` 控制器，確保表單送出（Request 週期重置）後，前端下拉選單仍能精準保留上一次的選取狀態（Selected Item）。
  * **動態排序：** 於網頁標頭導入 `sortBy` 參數，利用 C# `switch 運算式` 進行單價（UnitPrice）的正倒序切換，並整合 `Context.Request.Query` 確保排序與篩選條件跨 Request 交互保留。
* **技術焦點：** 狀態保留（State Retention）、多條件動態 LINQ 組裝、模型屬性命名規範。

### 3. Fetch API 非同步無刷新刪除
* **分支名稱：** `feature/fetch-delete-api`
* **實作內容：**
  * **行內事件移除：** 依循現代前端開發規範，全面移除 HTML 行內 `onclick` 屬性，改以 `addEventListener` 進行事件監聽與行為綁定。
  * **非同步資料交換：** 控制器端建立回傳 `JsonResult` 的 API 端點（Endpoint），前端採用原生 JavaScript **Fetch API** 發送非同步 `POST` 請求。
  * **DOM 節點異步操作：** 捨棄非必要之動畫特效，追求操作容錯率與高回應性。於確認後端刪除成功後，直接透過 `button.closest('tr').remove()` 進行 DOM 節點移除，實現零刷新（Zero-Refresh）的使用者體驗。
* **技術焦點：** 前後端解耦、非同步編程（Async/Await）、Web API 設計原則、DOM 操縱。

### 4. 服務層抽離與依賴注入重構
* **分支名稱：** `refactor/product-service-di`
* **實作內容：**
  * **關注點分離（SoC）：** 建立獨立的商業邏輯層（Business Logic Layer）。定義 `IProductService` 介面作為系統契約，並由 `ProductService` 承接原配置於 Controller 中的動態查詢與排序邏輯。
  * **依賴注入（DI）：** 於 `Program.cs` 容器中註冊 `builder.Services.AddScoped<IProductService, ProductService>()`，落實依賴反轉原則（DIP）。
  * **輕量化控制器（Thin Controller）：** 重構後的 Controller 僅負責請求調度與視圖渲染，不直接參與 SQL 邏輯拼湊，大幅提升代碼的可讀性與單元測試可行性（Unit Testability）。
  * **介面抽象與唯讀控制：** Service 接口明確回傳 **`IEnumerable<Product>`**。此舉旨在阻斷 Controller 層進一步追加 LINQ 查詢以致污染 SQL 生成的風險，並利用其唯讀特性提供最小權限控制，同時抹平底層集合（如 List 或 Array）的實作差異。
  * **漸進式重構（Progressive Refactoring）：** 於 Controller 建構子中同時保留 `DbContext` 與新抽離的 `IProductService`，確保其餘未調整之 CRUD 功能不受影響，展示在既有系統中進行安全重構的時程掌控力。
* **技術焦點：** 軟體架構設計、低耦合（Loose Coupling）、單一職責原則（SRP）、介面導向程式設計。

---

## 🛠️ 環境與工具配置
* **開發環境：** .NET 8.0 / ASP.NET Core MVC
* **資料庫：** Microsoft SQL Server (LocalDB) - Northwind Database
* **ORM 工具：** Entity Framework Core / EF Core Power Tools
* **前端框架：** Bootstrap 5 / 原生 JavaScript (ES6+)
