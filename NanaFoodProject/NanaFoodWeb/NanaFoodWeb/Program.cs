using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using NanaFoodWeb.Extensions;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Utility;
using QuestPDF.Infrastructure;
using static System.Environment;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
string lisense = builder.Configuration["Key:SyncFunsion"];
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(lisense);
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Sử dụng HTTPS cho cookie
        options.Cookie.SameSite = SameSiteMode.None; // Để cookie có thể được gửi trong yêu cầu cross-origin
        options.LoginPath = "/Auth/Login"; // Đường dẫn khi chưa đăng nhập
        options.LogoutPath = "/Auth/Logout"; // Đường dẫn khi đăng xuất
        /*options.AccessDeniedPath = "/Home/Forbidden";*/ // Đường dẫn khi từ chối truy cập
        options.AccessDeniedPath = "/Auth/AccessDenied";
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
builder.Services.AddScoped<IDashBoardRepository, DashBoardRepository>();
builder.Services.AddScoped<EmailConfirmed>();
builder.Services.AddScoped<CheckUserStatus>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("https://nanafoodweb20241114171424.azurewebsites.net", "https://nanafoodapi20241110164928.azurewebsites.net")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Quan trọng để cho phép cookie cross-origin
        });
});


var app = builder.Build();


//StaticDetails.APIBase = builder.Configuration["ServiceUrls:APIBase"];
StaticDetails.APIBase = builder.Configuration["ServiceUrls:PublicAPIUrl"];
/*StaticDetails.GHNApiKey = GetEnvironmentVariable("GHN_API_KEY");*/
StaticDetails.GHNApiKey = builder.Configuration["Key:GHN_API_KEY"];


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

app.UseCors("AllowSpecificOrigins"); // Áp dụng CORS policy

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/NotFoundPage");
    }
    if (context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/Auth/Login");
    }
    if(context.HttpContext.Response.StatusCode == 403)
    {
        context.HttpContext.Response.Redirect("/Home/Forbiden"); 
    }
});

// Cấu hình NotFound với StatusCodePages
/*app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/NotFoundPage");
    }
});*/
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
