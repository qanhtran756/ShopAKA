-Razor Pages
Xây dựng website theo từng trang (.cshtml).

- `Program.cs`: `AddRazorPages()`, `MapRazorPages()`
- Thư mục `Pages/`: mỗi trang gồm `.cshtml` + `.cshtml.cs`
- Khai báo trang: `@page` ở đầu file, ví dụ `Pages/Index.cshtml`, `Pages/Login.cshtml`, `Pages/Cart.cshtml`, `Pages/ProductDetail.cshtml`, `Pages/Checkout.cshtml`
- Trang admin: `Pages/Admin/Categories/Index.cshtml`, `Pages/Admin/Products/Index.cshtml`, …

# View Components

Tạo thành phần giao diện có thể tái sử dụng.

- Class: `ViewComponents/ShopViewComponents.cs`
  - `CategoryMenuViewComponent`
  - `CartBadgeViewComponent`
  - `AdminSidebarViewComponent`
  - `FeaturedProductsViewComponent`
- View: `Pages/Shared/Components/CategoryMenu/Default.cshtml`, `CartBadge/Default.cshtml`, `AdminSidebar/Default.cshtml`, `FeaturedProducts/Default.cshtml`
- Gọi trong layout/trang:
  - `Pages/Shared/_Layout.cshtml`: `@await Component.InvokeAsync("CategoryMenu")`
  - `Pages/Index.cshtml`: `@await Component.InvokeAsync("FeaturedProducts", new { count = 4 })`
  - `Pages/Shared/_AdminLayout.cshtml`: `@await Component.InvokeAsync("AdminSidebar", new { activePage = ViewData["ActivePage"] })`

# Blazor

Xây dựng giao diện web bằng C#.

- `Program.cs`: `AddServerSideBlazor()`, `MapBlazorHub()`
- Component: `Components/ProductSearch.razor` (logic C# trong `@code`, bind `@bind`, sự kiện `@onclick`)
- Nhúng vào Razor Page: `Pages/Shared/_Layout.cshtml` — `Html.RenderComponentAsync<ProductSearch>(RenderMode.Server)` trên navbar

# Tag Helpers

Hỗ trợ tạo HTML động bằng các thuộc tính `asp-*`.

- Bật Tag Helpers: `Pages/_ViewImports.cshtml` — `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`
- `asp-page`: `Pages/Shared/_Layout.cshtml` — `asp-page="/Index"`, `asp-page="/Cart"`, `asp-page="/Login"`
- `asp-route`: `Pages/Shared/_ProductCard.cshtml` — `asp-page="/ProductDetail" asp-route-id="@Model.ProductID"`
- `asp-for`: `Pages/Login.cshtml` — `asp-for="LoginInput.Username"`, `asp-for="LoginInput.Password"`
- `asp-page-handler`: `Pages/Login.cshtml` — `asp-page-handler="Login"`, `asp-page-handler="Register"`
- `asp-items`: `Pages/Admin/Products/Create.cshtml` — `asp-for="Product.CategoryID" asp-items="Model.CategoryList"`
