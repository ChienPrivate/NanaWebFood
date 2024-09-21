using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Helper;
using NanaFoodDAL.IRepository;
using NanaFoodDAL.IRepository.Repository;
using NanaFoodDAL.Model;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("NanaFoodApi"));
});
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// Config các IService và Service ở chỗ này ↓
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Config các IService và Service ở chỗ này ↑


// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
