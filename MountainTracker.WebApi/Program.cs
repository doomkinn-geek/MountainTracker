using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MountainTracker.Infrastructure.Data;
using MountainTracker.Infrastructure.Repositories.Implementations;
using MountainTracker.Infrastructure.Repositories.Interfaces;
using MountainTracker.Infrastructure.Services;
using MountainTracker.Core.Services;
using System.Text;
using MountainTracker.WebApi.Filters;
using Microsoft.AspNetCore.Identity;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Читаем строку подключения
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2) Подключаем DbContext
            builder.Services.AddDbContext<MountainTrackerDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // или UseSqlServer(connectionString)
            });

            // 3) Регистрируем репозитории            
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IReminderRepository, ReminderRepository>();

            // 4) Регистрируем сервисы (из Core -> реализация в Infrastructure)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IReminderService, ReminderService>();            

            // 5) Контроллеры + Swagger            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MountainTracker API",
                    Version = "v1",
                    Description = "API для горнолыжного приложения (локации, чат, напоминания и т.д.)"
                });

                // Для JWT авторизации в Swagger (опционально):
                
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        new string[] {}
                    }
                });                
            });

            // 6) Аутентификация (JWT) - опционально
            //   (Если вы хотите защищать некоторые эндпоинты)
            var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "default_secret_key";
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "mountain_app";
            var audience = builder.Configuration["Jwt:Audience"] ?? "mountain_users";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Настройка требований к паролю
                options.Password.RequireDigit = false;                   // не требовать обязательно цифры
                options.Password.RequireLowercase = false;               // не требовать строчные
                options.Password.RequireUppercase = false;               // не требовать заглавные
                options.Password.RequireNonAlphanumeric = false;         // не требовать символы (например, !@#)
                options.Password.RequiredLength = 6;                     // минимальная длина пароля
                options.Password.RequiredUniqueChars = 1;                // количество уникальных символов
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MountainTrackerDbContext>()
                .AddDefaultTokenProviders();           


            // (Опционально) Глобальный фильтр для обработки исключений
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new ApiExceptionFilter());
            });

            var app = builder.Build();

            // 7) Применяем миграции при старте (опционально)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MountainTrackerDbContext>();
                dbContext.Database.Migrate();
            }

            // 8) Подключаем Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // если JWT
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
