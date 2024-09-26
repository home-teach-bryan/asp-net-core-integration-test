using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Jwt;
using AspNetCoreIntegration.Models;
using AspNetCoreIntegration.ServiceCollection;
using AspNetCoreIntegration.Services;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCustomSwaggerGen();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<JwtTokenGenerator>();
        builder.Services.AddCustomJwtAuthentication(builder.Configuration);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        
        builder.Services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("ProductConnectionString"));
        });

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
        app.Run();
    }
}