using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.DBContext;
using Microsoft.IdentityModel.Tokens;
using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;
using PetShop.Services.Services;
using PetShop.Repositories.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PetShop.Services.Mapper;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // Loại bỏ độ trễ thời gian (nếu có)
        };
        // 🔥 Thêm phần kiểm tra blacklist
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

                if (authService.IsTokenBlacklisted(token))
                {
                    context.Fail("Token is blacklisted");
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dorfo API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,   // ✅ dùng Http
        Scheme = "bearer",                                         // ✅ phải có bearer
        BearerFormat = "JWT",                                      // ✅ format JWT
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token with **Bearer** prefix. Example: `Bearer {your token}`"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"   // phải trùng với tên ở trên
                }
            },
            new string[] {}
        }
    });

    c.MapType<TimeSpan>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Example = new Microsoft.OpenApi.Any.OpenApiString("08:00:00"),
        Format = "time"
    });

    c.MapType<TimeSpan?>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Example = new Microsoft.OpenApi.Any.OpenApiString("08:00:00"),
        Format = "time"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Cho phép tất cả domain
                  .AllowAnyMethod()   // Cho phép tất cả method (GET, POST, PUT, DELETE...)
                  .AllowAnyHeader();  // Cho phép tất cả header
        });
});


// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PetShop");

// Đăng ký DbContext
builder.Services.AddDbContext<PetShopDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpClient();

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


//mapper
builder.Services.AddAutoMapper(typeof(ProductMapper));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
