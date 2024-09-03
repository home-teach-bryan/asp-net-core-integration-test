using AspNetCoreIntegration;
using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegrationTests.Factory;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace AspNetCoreIntegrationTests;

[SetUpFixture]
public class TestSetup
{
    public static MsSqlContainer DbContainer;
    public static AspNetCoreIntegrationWebApplicationFactory<Program> Factory;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        DbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SA_PASSWORD","Aa123456")
            .WithPortBinding("1433")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
        await DbContainer.StartAsync();
        Factory = new AspNetCoreIntegrationWebApplicationFactory<Program>(DbContainer);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        await dbContext.Database.MigrateAsync();
        await SeedData();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await DbContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }

    private async Task SeedData()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

        var admin = new User
        {
            Name = "Admin",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin"),
            Roles = new string[] { "Admin" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        var user = new User
        {
            Name = "User",
            Password = BCrypt.Net.BCrypt.HashPassword("User"),
            Roles = new string[] { "User" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        dbContext.Users.Add(admin);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

    }

}