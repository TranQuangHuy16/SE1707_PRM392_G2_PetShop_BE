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
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


// ============ JWT AUTHENTICATION ============
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
            ClockSkew = TimeSpan.Zero
        };

        // Kiểm tra blacklist token
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

// ============ CONTROLLERS ============
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ============ SWAGGER ============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PetShop API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
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
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ============ CORS ============
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ============ CONNECTION STRING ============
var connectionString =
    builder.Configuration.GetConnectionString("PetShop") ??
    builder.Configuration["ConnectionStrings__PetShop"] ??
    builder.Configuration["CONNECTION_STRING"] ??
    Environment.GetEnvironmentVariable("ConnectionStrings__PetShop") ??
    Environment.GetEnvironmentVariable("CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ ERROR: Connection string is NULL or EMPTY!");
}
else
{
    Console.WriteLine($"✅ Connection string loaded: {connectionString}");
}

// Bắt lỗi SSL Mode nếu viết sai (nhiều bạn gõ "SSL Mode" thay vì "Ssl Mode")
connectionString = connectionString.Replace("SSL Mode", "Ssl Mode");

// ============ DB CONTEXT ============
builder.Services.AddDbContext<PetShopDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============ REPOSITORIES & SERVICES ============
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMapboxService, MapboxService>();
builder.Services.AddScoped<IUserAddressService, UserAddressService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IZaloPayService, ZaloPayService>();

builder.Services.AddAutoMapper(typeof(ProductMapper));
builder.Services.AddAutoMapper(typeof(CartMapper));
builder.Services.AddAutoMapper(typeof(PaymentMapper));

var app = builder.Build();

// ============ DATABASE MIGRATION (có retry + delay) ============
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PetShopDbContext>();
    var connectionStr = dbContext.Database.GetConnectionString();

    Console.WriteLine("⏳ Waiting for PostgreSQL to be ready...");
    Thread.Sleep(8000); // chờ DB Render khởi động

    try
    {
        Console.WriteLine("🔍 Testing connection...");
        using var conn = new NpgsqlConnection(connectionStr);
        conn.Open();
        Console.WriteLine("✅ PostgreSQL connection successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ PostgreSQL connection failed: {ex.Message}");
    }

    int retryCount = 0;
    const int maxRetries = 5;

    while (true)
    {
        try
        {
            Console.WriteLine("🔄 Attempting database migration...");
            dbContext.Database.Migrate();
            Console.WriteLine("✅ Database migration successful!");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            Console.WriteLine($"⚠️ Migration attempt {retryCount} failed: {ex.Message}");
            if (retryCount >= maxRetries)
                throw;
            Thread.Sleep(5000);
        }
    }
}

// ============ APP PIPELINE ============
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
