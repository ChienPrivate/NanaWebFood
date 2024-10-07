
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Utility;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Đường dẫn để chuyển hướng khi chưa đăng nhập
        options.LogoutPath = "/Auth/Logout"; // Đường dẫn khi đăng xuất
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn của cookie
        options.SlidingExpiration = true; // Kéo dài thời gian hết hạn nếu người dùng tiếp tục hoạt động
        options.Cookie.HttpOnly = true; // Chỉ truy cập cookie qua HTTP (bảo mật hơn)
        options.Cookie.IsEssential = true; // Cookie này là cần thiết cho chức năng ứng dụng
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddRazorPages();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();


StaticDetails.APIBase = builder.Configuration["ServiceUrls:APIBase"];

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

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
