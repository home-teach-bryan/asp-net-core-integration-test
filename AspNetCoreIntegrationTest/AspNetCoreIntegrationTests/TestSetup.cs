using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using AspNetCoreIntegration;
using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using AspNetCoreIntegrationTests.Factory;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace AspNetCoreIntegrationTests;

[SetUpFixture]
public class TestSetup
{
    private static string _tokenEndpoint = "api/Token";
    
    public static MsSqlContainer DbContainer;
    public static AspNetCoreIntegrationWebApplicationFactory<Program> Factory;
    public static string AdminUserId = "fcfa1500-b231-4161-8089-be5cd0159bd5";
    public static string UserId = "1fd043eb-4f55-45fc-82fb-f8b0018bacf1";
    public static string SuperAdminUserId = "5b83c860-d298-4919-bb1d-842f637fb90c";
    public static string ExistProductId1 = "855295d1-242b-4d7b-aa0b-08b8a84706bf";
    public static string ExistProductId2 = "72193f41-abfc-4545-832c-66ab136c2948";
    public static string ExistProductId3 = "654d48e6-4bc0-4461-ba77-72408619836d";
    public static string ExistProductId4 = "e5f9f9b7-8f7f-4c4e-b7f7-7a8c8c8c8c8c";
    public static string DeleteProductId1 = "cac02031-2d37-4444-a925-c5fa6b660075";
    public static string DeleteProductId2 = "2f610747-17f6-463c-89c4-8b47c3103493";

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

    public static async Task<HttpClient> GenerateClientWithTokenAsync(string name, string password)
    {
        var getTokenClient = TestSetup.Factory.CreateClient();
        var getTokenRequest = new GetTokenRequest
        {
            Name = name,
            Password = password
        };
        var response = await getTokenClient.PostAsJsonAsync(_tokenEndpoint, getTokenRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        if (result.Status != ApiResponseStatus.Success)
        {
            throw new Exception("get token error");
        }

        var token = result.Data;
        var clientWithToken = TestSetup.Factory.CreateClient();
        clientWithToken.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        return clientWithToken;
    }
    
    private async Task SeedData()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

        var admin = new User
        {
            Id = Guid.Parse(AdminUserId),
            Name = "Admin",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin"),
            Roles = new string[] { "Admin" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        var user = new User
        {
            Id = Guid.Parse(UserId),
            Name = "User",
            Password = BCrypt.Net.BCrypt.HashPassword("User"),
            Roles = new string[] { "User" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };

        var superAdmin = new User
        {
            Id = Guid.Parse(SuperAdminUserId),
            Name = "SuperAdmin",
            Password = BCrypt.Net.BCrypt.HashPassword("SuperAdmin"),
            Roles = new string[] { "Admin", "User" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User1",
            Password = BCrypt.Net.BCrypt.HashPassword("User1"),
            Roles = new string[] { "User" },
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        dbContext.Users.Add(admin);
        dbContext.Users.Add(user);
        dbContext.Users.Add(superAdmin);
        dbContext.Users.Add(user1);

        var existProduct = new Product
        {
            Id = Guid.Parse(ExistProductId1),
            Name = "產品1",
            Price = 100,
            Quantity = 10,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        var existProduct2 = new Product
        {
            Id = Guid.Parse(ExistProductId2),
            Name = "產品2",
            Price = 100,
            Quantity = 10,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        var existProduct3 = new Product
        {
            Id = Guid.Parse(ExistProductId3),
            Name = "產品3",
            Price = 100,
            Quantity = 10,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        var existProduct4 = new Product
        {
            Id = Guid.Parse(ExistProductId4),
            Name = "產品4",
            Price = 100,
            Quantity = 5,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        
        var deleteProduct = new Product
        {
            Id = Guid.Parse(DeleteProductId1),
            Name = "刪除產品1",
            Price = 100,
            Quantity = 10,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
        var deleteProduct2 = new Product
        {
            Id = Guid.Parse(DeleteProductId2),
            Name = "刪除產品2",
            Price = 100,
            Quantity = 10,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderUser = user,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    Product = existProduct3,
                    OrderPrice = 100,
                    OrderQuantity = 1
                },
                new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    Product = existProduct4,
                    OrderPrice = 100,
                    OrderQuantity = 1
                },
            },
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        
        dbContext.Products.Add(existProduct);
        dbContext.Products.Add(existProduct2);
        dbContext.Products.Add(existProduct3);
        dbContext.Products.Add(existProduct4);
        dbContext.Products.Add(deleteProduct);
        dbContext.Products.Add(deleteProduct2);
        dbContext.Orders.AddRange(order);
        await dbContext.SaveChangesAsync();

    }

    
}