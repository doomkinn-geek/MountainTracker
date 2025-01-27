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

            // 1) ������ ������ �����������
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2) ���������� DbContext
            builder.Services.AddDbContext<MountainTrackerDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // ��� UseSqlServer(connectionString)
            });

            // 3) ������������ �����������            
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IReminderRepository, ReminderRepository>();

            // 4) ������������ ������� (�� Core -> ���������� � Infrastructure)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IReminderService, ReminderService>();            

            // 5) ����������� + Swagger            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MountainTracker API",
                    Version = "v1",
                    Description = "API ��� ������������ ���������� (�������, ���, ����������� � �.�.)"
                });

                // ��� JWT ����������� � Swagger (�����������):
                
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "������� JWT �����.",
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

            // 6) �������������� (JWT) - �����������
            //   (���� �� ������ �������� ��������� ���������)
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
                // ��������� ���������� � ������
                options.Password.RequireDigit = false;                   // �� ��������� ����������� �����
                options.Password.RequireLowercase = false;               // �� ��������� ��������
                options.Password.RequireUppercase = false;               // �� ��������� ���������
                options.Password.RequireNonAlphanumeric = false;         // �� ��������� ������� (��������, !@#)
                options.Password.RequiredLength = 6;                     // ����������� ����� ������
                options.Password.RequiredUniqueChars = 1;                // ���������� ���������� ��������
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MountainTrackerDbContext>()
                .AddDefaultTokenProviders();           


            // (�����������) ���������� ������ ��� ��������� ����������
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new ApiExceptionFilter());
            });

            var app = builder.Build();

            // 7) ��������� �������� ��� ������ (�����������)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MountainTrackerDbContext>();
                dbContext.Database.Migrate();
            }

            // 8) ���������� Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // ���� JWT
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
