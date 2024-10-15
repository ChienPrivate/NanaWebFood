using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NanaFoodDAL.Context;
using NanaFoodDAL.Helper;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.IRepository.Repository;
using NanaFoodDAL.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using static System.Environment;

// Load các biến nằm trong file .env và lấy giá trị của các biến đó (tăng tính bảo mật cho ứng dụng tránh lộ các thông tin quan trọng)
Env.Load();
string apikey = GetEnvironmentVariable("NANA_API_KEY");


var builder = WebApplication.CreateBuilder(args);

IMapper mapper = MapConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CloudConnection"), b => b.MigrationsAssembly("NanaFoodApi"));
});
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // Scheme mặc định khi thách thức (Challenge) là JWT
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    // Scheme mặc định cho việc đăng nhập (SignIn) là Cookie
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"],
        ValidAudience = builder.Configuration["ApiSettings:JwtOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:JwtOptions:SigningKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true
        
    };
}).AddOAuth("github", o =>
{
    o.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
    o.ClientId = builder.Configuration["Authentication:Github:ClientId"];
    o.ClientSecret = builder.Configuration["Authentication:Github:ClientSecret"];

    o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    o.TokenEndpoint = "https://github.com/login/oauth/access_token";

    o.CallbackPath = "/Auth/ExternalLoginCallBack";
    /*o.SaveTokens = true;*/
    o.UserInformationEndpoint = "https://api.github.com/user";

    o.ClaimActions.MapJsonKey("sub", "id");
    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

    o.Events = new OAuthEvents
    {
        OnCreatingTicket = async ctx =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            using var result = await ctx.Backchannel.SendAsync(request);
            result.EnsureSuccessStatusCode();
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();

            // Áp dụng claims từ user nhận được
            ctx.RunClaimActions(user);

            // Lấy thông tin từ claims
            var username = ctx.Principal.FindFirstValue("sub"); // Lấy username từ claim "sub"
            var fullName = ctx.Principal.FindFirstValue(ClaimTypes.Name); // Lấy fullname từ claim "login"
            var imageUrl = user.GetProperty("avatar_url").GetString();

            // Sử dụng ApplicationDbContext để tìm kiếm hoặc tạo user
            using var scope = ctx.HttpContext.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            // Kiểm tra xem user đã tồn tại hay chưa
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "G" + username);
            string token;

            if (existingUser != null)
            {
                // Lấy các vai trò (roles) của user đã tồn tại
                var roles = await userManager.GetRolesAsync(existingUser);

                // Tạo JWT token cho user đã tồn tại
                token = tokenService.CreateToken(existingUser, roles);
            }
            else
            {
                // Tạo user mới
                var newUser = new User
                {
                    EmailConfirmed = true,
                    Email = "githubexample@github.com",  // Có thể thay đổi email khi cần
                    UserName = "G"+username,
                    FullName = fullName,
                    AvatarUrl = imageUrl
                };
                var createUserResult = await userManager.CreateAsync(newUser);

                // Kiểm tra và tạo role nếu chưa tồn tại
                string roleName = "customer";
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Gán role cho user mới
                await userManager.AddToRoleAsync(newUser, roleName);

                // Tạo Cart cho user mới
                var cart = new Cart
                {
                    UserId = newUser.Id,
                };
                await dbContext.Carts.AddAsync(cart);
                await dbContext.SaveChangesAsync();

                // Tạo JWT token cho user mới
                var roles = new List<string> { roleName };
                token = tokenService.CreateToken(newUser, roles);
            }

            // Mã hóa JWT token thành Base64
            var base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            // RedirectUri với token mã hóa Base64
            var redirectUri = $"{ctx.Properties.RedirectUri}?data={base64Token}";

            // Chuyển hướng đến client với token
            ctx.Response.Redirect(redirectUri);
        },

        OnTicketReceived = ctx =>
        {
            // Điều này sẽ ngăn việc tự động đăng nhập qua SignInAsync 
            ctx.HandleResponse(); // Chặn xử lý đăng nhập dưới API
            return Task.CompletedTask;
        }
    };
}).AddGoogle(GoogleDefaults.AuthenticationScheme, o =>
{
    o.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
    o.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    o.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
    o.TokenEndpoint = "https://oauth2.googleapis.com/token";
    o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

    o.CallbackPath = "/Auth/google";
    /*o.SaveTokens = true;*/
    o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

    o.Events = new OAuthEvents
    {
        OnCreatingTicket = async ctx =>
        {
            // Gửi yêu cầu đến UserInformationEndpoint
            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            using var result = await ctx.Backchannel.SendAsync(request);
            result.EnsureSuccessStatusCode();
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();

            // Áp dụng claims từ thông tin người dùng nhận được từ Google
            ctx.RunClaimActions(user);

            // Lấy thông tin từ claims
            var userId = user.GetProperty("sub").GetString(); // Lấy ID người dùng
            var fullName = user.GetProperty("name").GetString(); // Lấy tên đầy đủ
            var pictureUrl = user.GetProperty("picture").GetString();
            var email = user.GetProperty("email").GetString();
            var emailVerified = user.GetProperty("email_verified").GetBoolean();

            // Sử dụng ApplicationDbContext để tìm kiếm hoặc tạo user mới
            using var scope = ctx.HttpContext.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            // Kiểm tra xem user đã tồn tại hay chưa
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "GG" + userId);
            string token;

            if (existingUser != null)
            {
                // Lấy các vai trò (roles) của user đã tồn tại
                var roles = await userManager.GetRolesAsync(existingUser);

                // Tạo JWT token cho user đã tồn tại
                token = tokenService.CreateToken(existingUser, roles);
            }
            else
            {
                // Tạo user mới
                var newUser = new User
                {
                    EmailConfirmed = emailVerified,
                    Email = email,  // Email từ Google
                    UserName = "GG"+userId,
                    FullName = fullName,
                    AvatarUrl = pictureUrl,
                };
                var createUserResult = await userManager.CreateAsync(newUser);

                // Kiểm tra và tạo role nếu chưa tồn tại
                string roleName = "customer";
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Gán role cho user mới
                await userManager.AddToRoleAsync(newUser, roleName);

                // Tạo Cart cho user mới (nếu cần)
                var cart = new Cart
                {
                    UserId = newUser.Id,
                };
                await dbContext.Carts.AddAsync(cart);
                await dbContext.SaveChangesAsync();

                // Tạo JWT token cho user mới
                var roles = new List<string> { roleName };
                token = tokenService.CreateToken(newUser, roles);
            }

            // Mã hóa JWT token thành Base64
            var base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            // RedirectUri với token mã hóa Base64
            var redirectUri = $"{ctx.Properties.RedirectUri}?data={base64Token}";

            // Chuyển hướng đến client với token
            ctx.Response.Redirect(redirectUri);
        },

        OnTicketReceived = ctx =>
        {
            // Ngăn việc tự động đăng nhập
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    };
}).AddFacebook(FacebookDefaults.AuthenticationScheme, o =>
{
    o.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
    o.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
    o.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
    o.AuthorizationEndpoint = FacebookDefaults.AuthorizationEndpoint;
    o.TokenEndpoint = FacebookDefaults.TokenEndpoint;
    o.UserInformationEndpoint = FacebookDefaults.UserInformationEndpoint;

    o.Scope.Add("public_profile");
    o.Scope.Add("email");

    o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

    o.Events = new OAuthEvents
    {
        OnCreatingTicket = async ctx =>
        {
            // Gửi yêu cầu đến UserInformationEndpoint
            /*var requestUrl = $"https://graph.facebook.com/melfields-first_name.last_name.picture.email&accesstoken={ctx.AccessToken}";*/

            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            using var result = await ctx.Backchannel.SendAsync(request);
            result.EnsureSuccessStatusCode();
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();
            
            // Áp dụng claims từ thông tin người dùng nhận được từ Google
            ctx.RunClaimActions(user);

            // Lấy thông tin từ claims
            var userId = user.GetProperty("id").GetString(); // Lấy username từ claim "sub"
            var email = ctx.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = ctx.Principal.FindFirstValue(ClaimTypes.Name); // Lấy fullname từ claim Name

            // Sử dụng ApplicationDbContext để tìm kiếm hoặc tạo user mới
            using var scope = ctx.HttpContext.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            // Kiểm tra xem user đã tồn tại hay chưa
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "FB" + userId);
            string token;

            if (existingUser != null)
            {
                // Lấy các vai trò (roles) của user đã tồn tại
                var roles = await userManager.GetRolesAsync(existingUser);

                // Tạo JWT token cho user đã tồn tại
                token = tokenService.CreateToken(existingUser, roles);
            }
            else
            {
                // Tạo user mới
                var newUser = new User
                {
                    EmailConfirmed = true,
                    Email = email,  // Email từ Google
                    UserName = "FB" + userId,
                    FullName = fullName,
                };
                var createUserResult = await userManager.CreateAsync(newUser);

                // Kiểm tra và tạo role nếu chưa tồn tại
                string roleName = "customer";
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Gán role cho user mới
                await userManager.AddToRoleAsync(newUser, roleName);

                // Tạo Cart cho user mới (nếu cần)
                var cart = new Cart
                {
                    UserId = newUser.Id,
                };
                await dbContext.Carts.AddAsync(cart);
                await dbContext.SaveChangesAsync();

                // Tạo JWT token cho user mới
                var roles = new List<string> { roleName };
                token = tokenService.CreateToken(newUser, roles);
            }

            // Mã hóa JWT token thành Base64
            var base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            // RedirectUri với token mã hóa Base64
            var redirectUri = $"{ctx.Properties.RedirectUri}?data={base64Token}";

            // Chuyển hướng đến client với token
            ctx.Response.Redirect(redirectUri);
        },

        OnTicketReceived = ctx =>
        {
            // Ngăn việc tự động đăng nhập
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer "
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nana Food API 1.0",
        Version = "v1.0",
        Description = "An API to perform fast food delivery operations"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
    

// Config các IService và Service ở chỗ này ↓
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthenRepo, AuthenRepo>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<EmailPoster>();
builder.Services.AddScoped<CloudinaryService>();

// Config các IService và Service ở chỗ này ↑


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

/*Phương thức ApplyMigration() trong đoạn mã của bạn có nhiệm vụ áp dụng các migrations 
 * (di chuyển cơ sở dữ liệu) trong ứng dụng sử dụng Entity Framework (EF) Core.
 * Cụ thể nó kiểm tra xem có các migrations nào chưa được áp dụng và tự động cập nhật cơ sở dữ liệu nếu cần.*/
void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}

