
//using FoodOrdering.API.Extensions;
//using FoodOrdering.API.Middleware;
//using FoodOrdering.Application;
//using FoodOrdering.Infrastructure;
//using TaskManagement.API.Extensions;

//namespace FoodOrdering.API
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("AllowAll", policy =>
//                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//            });
//            builder.Services.AddControllers();
//            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//            builder.Services.AddSwaggerGen();

//            builder.Services.AddAutoMapper(map => map.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));

//            builder.Services.AddOpenApi();
//            builder.Services.AddInfrastructure(builder.Configuration);
//            builder.Services.AddApplication();
//            builder.Services.AddSwaggerWithVersioning();
//            var app = builder.Build();
//            await app.SeedDatabaseAsync();
//            //using (var scope = app.Services.CreateScope())
//            //{
//            //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//            //    db.Database.Migrate();
//            //     DatabaseSeeder.SeedAsync(app.Services);
//            //}
//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//            {
//                //app.MapOpenApi();
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }
//            app.UseMiddleware<ExceptionMiddleware>();
//            app.UseMiddleware<RequestLoggingMiddleware>();
//            app.UseMiddleware<LanguageMiddleware>();


//            app.UseHttpsRedirection();
//            app.UseAuthorization();


//            app.MapControllers();

//            app.Run();
//        }
//    }
//}
using FoodOrdering.API.Extensions;
using FoodOrdering.API.Middleware;
using FoodOrdering.Application;
using FoodOrdering.Infrastructure;
using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4000"
              
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSwaggerWithVersioning();

var app = builder.Build();

//await app.SeedDatabaseAsync();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<LanguageMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();