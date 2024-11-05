
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Utility;
using static System.Environment;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
string lisense = GetEnvironmentVariable("SyncFusionLisense");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(lisense);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sử dụng HTTPS cho cookie
        options.Cookie.SameSite = SameSiteMode.None; // Để cookie có thể được gửi trong yêu cầu cross-origin
        options.LoginPath = "/Auth/Login"; // Đường dẫn khi chưa đăng nhập
        options.LogoutPath = "/Auth/Logout"; // Đường dẫn khi đăng xuất
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Đường dẫn khi từ chối truy cập
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
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
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IHelperRepository, HelperRepository>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepo, CartRepo>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICouponRepo, CouponRepo>();



var app = builder.Build();


StaticDetails.APIBase = builder.Configuration["ServiceUrls:APIBase"];
StaticDetails.GHNApiKey = GetEnvironmentVariable("GHN_API_KEY");

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

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/NotFoundPage");
    }
});
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
