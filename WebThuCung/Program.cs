using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian h?t h?n session (30 phút)
    options.Cookie.HttpOnly = true; // Cookie ch? ???c truy c?p qua HTTP
    options.Cookie.IsEssential = true; // Cookie này là c?n thi?t cho ?ng d?ng
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                // Đặt mã trạng thái và trả về thông báo lỗi
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("Bạn không có quyền truy cập vào trang này , mọi thắc mắc xin liên hệ Admin để giải quyết .");
            }
        };
    });


// Add
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PetContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PetContext"))
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireOrderRole", policy => policy.RequireRole("StaffOrder"));
    options.AddPolicy("RequireProductRole", policy => policy.RequireRole("StaffProduct"));
    options.AddPolicy("RequireWarehouseRole", policy => policy.RequireRole("StaffWareHouse"));
    options.AddPolicy("RequireShipperRole", policy => policy.RequireRole("Shipper"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication(); // Đảm bảo middleware xác thực được gọi
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
