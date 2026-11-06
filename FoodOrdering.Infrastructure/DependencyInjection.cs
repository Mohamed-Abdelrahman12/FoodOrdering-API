using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Caching;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.seeding;
using FoodOrdering.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace FoodOrdering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddSingleton<IImageManagementService, ImageManagementService>();

           
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(wwwrootPath);   

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            //services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = ConfigurationOptions.Parse(
                    config.GetConnectionString("Redis") ?? "localhost:6379");
                options.AbortOnConnectFail = false;
                options.ConnectRetry = 5;
                options.ConnectTimeout = 5000;
                return ConnectionMultiplexer.Connect(options);
            });

            services.AddScoped<IRedisCartRepository, RedisCartRepository>();

            services.AddInfrastructureAuth(config);

            return services;
        }

        private static IServiceCollection AddInfrastructureAuth(
            this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookie =>
                {
                    cookie.Cookie.Name = "accessToken";
                    cookie.Cookie.HttpOnly = true;
                    cookie.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    cookie.Cookie.SameSite = SameSiteMode.Strict;
                    cookie.Events.OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    cookie.Events.OnRedirectToAccessDenied = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.RequireHttpsMetadata = false;
                    jwt.SaveToken = true;

                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                        ValidateIssuer = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = config["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    jwt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var token = ctx.Request.Cookies["accessToken"];
                            if (!string.IsNullOrEmpty(token))
                                ctx.Token = token;
                            return Task.CompletedTask;
                        },
                        OnChallenge = ctx =>
                        {
                            ctx.HandleResponse();
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            ctx.Response.ContentType = "application/json";
                            return ctx.Response.WriteAsync(
                                "{\"success\":false,\"message\":\"Unauthorized.\",\"statusCode\":401}");
                        },
                        OnForbidden = ctx =>
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            ctx.Response.ContentType = "application/json";
                            return ctx.Response.WriteAsync(
                                "{\"success\":false,\"message\":\"Forbidden.\",\"statusCode\":403}");
                        }
                    };
                });

            return services;
        }
    }
}
//using FoodOrdering.Domain.Entities;
//using FoodOrdering.Domain.Interfaces;
//using FoodOrdering.Infrastructure.Caching;
//using FoodOrdering.Infrastructure.Data;
//using FoodOrdering.Infrastructure.Repositories;
//using FoodOrdering.Infrastructure.seeding;
//using FoodOrdering.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.FileProviders;
//using Microsoft.IdentityModel.Tokens;
//using StackExchange.Redis;
//using System.Text;


//namespace FoodOrdering.Infrastructure
//{
//    public static class DependencyInjection
//    {
//        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
//        {
//            services.AddDbContext<AppDbContext>(options =>
//                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

//            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//            {
//                options.Password.RequireDigit = true;
//                options.Password.RequiredLength = 8;
//                options.Password.RequireUppercase = true;
//                options.Password.RequireNonAlphanumeric = false;
//                options.User.RequireUniqueEmail = true;
//                options.SignIn.RequireConfirmedEmail = false;
//            })
//            .AddEntityFrameworkStores<AppDbContext>()
//            .AddDefaultTokenProviders();
//            services.AddSingleton<IImageManagementService, ImageManagementService>();
//            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
//                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
//            services.AddScoped<IUnitOfWork, UnitOfWork>();
//            services.AddScoped<ITokenService, TokenService>();
//            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

//            services.AddMemoryCache();
//            services.AddSingleton<ICacheService, CacheService>();

//            services.AddSingleton<IConnectionMultiplexer>(sp =>
//            {
//                var options = ConfigurationOptions.Parse(
//                    config.GetConnectionString("Redis") ?? "localhost:6379");
//                options.AbortOnConnectFail = false;
//                options.ConnectRetry = 5;
//                options.ConnectTimeout = 5000;
//                return ConnectionMultiplexer.Connect(options);
//            });

//            services.AddScoped<IRedisCartRepository, RedisCartRepository>();

//            services.AddInfrastructureAuth(config);

//            return services;
//        }

//        private static IServiceCollection AddInfrastructureAuth(
//            this IServiceCollection services, IConfiguration config)
//        {
//            services
//                .AddAuthentication(options =>
//                {
//                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                })
//                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookie =>
//                {
//                    cookie.Cookie.Name = "accessToken";
//                    cookie.Cookie.HttpOnly = true;
//                    cookie.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//                    cookie.Cookie.SameSite = SameSiteMode.Strict;
//                    cookie.Events.OnRedirectToLogin = ctx =>
//                    {
//                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                        return Task.CompletedTask;
//                    };
//                    cookie.Events.OnRedirectToAccessDenied = ctx =>
//                    {
//                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
//                        return Task.CompletedTask;
//                    };
//                })
//                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
//                {
//                    jwt.RequireHttpsMetadata = false;
//                    jwt.SaveToken = true;

//                    jwt.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,
//                        IssuerSigningKey = new SymmetricSecurityKey(
//                            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
//                        ValidateIssuer = true,
//                        ValidIssuer = config["Jwt:Issuer"],
//                        ValidateAudience = true,
//                        ValidAudience = config["Jwt:Audience"],
//                        ValidateLifetime = true,
//                        ClockSkew = TimeSpan.Zero
//                    };

//                    jwt.Events = new JwtBearerEvents
//                    {
//                        OnMessageReceived = ctx =>
//                        {
//                            var token = ctx.Request.Cookies["accessToken"];
//                            if (!string.IsNullOrEmpty(token))
//                                ctx.Token = token;
//                            return Task.CompletedTask;
//                        },
//                        OnChallenge = ctx =>
//                        {
//                            ctx.HandleResponse();
//                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                            ctx.Response.ContentType = "application/json";
//                            return ctx.Response.WriteAsync(
//                                "{\"success\":false,\"message\":\"Unauthorized.\",\"statusCode\":401}");
//                        },
//                        OnForbidden = ctx =>
//                        {
//                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
//                            ctx.Response.ContentType = "application/json";
//                            return ctx.Response.WriteAsync(
//                                "{\"success\":false,\"message\":\"Forbidden.\",\"statusCode\":403}");
//                        }
//                    };
//                });

//            return services;
//        }
//    }

    //public static class DependencyInjection
    //{
    //    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    //    {
    //        services.AddDbContext<AppDbContext>(options =>
    //            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

    //        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    //        {
    //            options.Password.RequireDigit = true;
    //            options.Password.RequiredLength = 8;
    //            options.Password.RequireUppercase = true;
    //            options.Password.RequireNonAlphanumeric = false;
    //            options.User.RequireUniqueEmail = true;
    //            options.SignIn.RequireConfirmedEmail = false;
    //        })
    //        .AddEntityFrameworkStores<AppDbContext>()
    //        .AddDefaultTokenProviders();

    //        services.AddScoped<IUnitOfWork, UnitOfWork>();
    //        services.AddScoped<ITokenService, TokenService>();

    //        services.AddMemoryCache();
    //        services.AddSingleton<ICacheService, CacheService>();

    //        services.AddSingleton<IConnectionMultiplexer>(sp =>
    //        {
    //            var options = ConfigurationOptions.Parse(
    //                config.GetConnectionString("Redis") ?? "localhost:6379"
    //            );
    //            options.AbortOnConnectFail = false;
    //            options.ConnectRetry = 5;
    //            options.ConnectTimeout = 5000;
    //            return ConnectionMultiplexer.Connect(options);
    //        });

    //        services.AddScoped<IRedisCartRepository, RedisCartRepository>();
    //        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
    //        services.AddInfrastructureAuthentication(config);

    //        return services;
    //    }

    //    private static IServiceCollection AddInfrastructureAuthentication(
    //        this IServiceCollection services, IConfiguration config)
    //    {
    //        services
    //            .AddAuthentication(options =>
    //            {
    //                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //            })
    //            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
    //            {
    //                cookieOptions.Cookie.Name = "accessToken";
    //                cookieOptions.Cookie.HttpOnly = true;
    //                cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //                cookieOptions.Cookie.SameSite = SameSiteMode.Strict;
    //                cookieOptions.Events.OnRedirectToLogin = context =>
    //                {
    //                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //                    return Task.CompletedTask;
    //                };
    //                cookieOptions.Events.OnRedirectToAccessDenied = context =>
    //                {
    //                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
    //                    return Task.CompletedTask;
    //                };
    //            })
    //            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    //            {
    //                jwtOptions.RequireHttpsMetadata = false;
    //                jwtOptions.SaveToken = true;

    //                jwtOptions.TokenValidationParameters = new TokenValidationParameters
    //                {
    //                    ValidateIssuerSigningKey = true,
    //                    IssuerSigningKey = new SymmetricSecurityKey(
    //                        Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
    //                    ValidateIssuer = true,
    //                    ValidIssuer = config["Jwt:Issuer"],
    //                    ValidateAudience = true,
    //                    ValidAudience = config["Jwt:Audience"],
    //                    ValidateLifetime = true,
    //                    ClockSkew = TimeSpan.Zero
    //                };

    //                jwtOptions.Events = new JwtBearerEvents
    //                {
    //                    OnMessageReceived = context =>
    //                    {
    //                        var bearerToken = context.Request.Cookies["accessToken"];
    //                        if (!string.IsNullOrEmpty(bearerToken))
    //                            context.Token = bearerToken;
    //                        return Task.CompletedTask;
    //                    },
    //                    OnChallenge = context =>
    //                    {
    //                        context.HandleResponse();
    //                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //                        context.Response.ContentType = "application/json";
    //                        return context.Response.WriteAsync(
    //                            "{\"success\":false,\"message\":\"Unauthorized. Token is missing or invalid.\",\"statusCode\":401}");
    //                    },
    //                    OnForbidden = context =>
    //                    {
    //                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    //                        context.Response.ContentType = "application/json";
    //                        return context.Response.WriteAsync(
    //                            "{\"success\":false,\"message\":\"Forbidden. You do not have permission.\",\"statusCode\":403}");
    //                    }
    //                };
    //            });

    //        return services;
    //    }
    //}
//}