using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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

    o.CallbackPath = "/oauth/github-cb";
    o.SaveTokens = true;
    o.UserInformationEndpoint = "https://api.github.com/user";

    o.ClaimActions.MapJsonKey("sub", "id");
    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

    o.Events.OnCreatingTicket = async ctx =>
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
        using var result = await ctx.Backchannel.SendAsync(request);
        var user = await result.Content.ReadFromJsonAsync<JsonElement>();

        ctx.RunClaimActions(user);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, ctx.Principal.FindFirst(ClaimTypes.NameIdentifier).Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, ctx.Principal.FindFirst(ClaimTypes.Name).Value),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:JwtOptions:SigningKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: builder.Configuration["ApiSettings:JwtOptions:Issuer"],
            audience: builder.Configuration["ApiSettings:JwtOptions:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Lưu JWT vào response để trả về cho người dùng
        ctx.Response.Cookies.Append("jwt", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
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
        Scheme = "Bearer"
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

