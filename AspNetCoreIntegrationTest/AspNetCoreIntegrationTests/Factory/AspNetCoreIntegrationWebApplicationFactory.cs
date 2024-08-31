using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Jwt;
using AspNetCoreIntegration.ServiceCollection;
using AspNetCoreIntegration.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace AspNetCoreIntegrationTests.Factory;

public class AspNetCoreIntegrationWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly MsSqlContainer _dbContainer;

    public AspNetCoreIntegrationWebApplicationFactory(MsSqlContainer dbContainer)
    {
        _dbContainer = dbContainer;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(service =>
        {
            service.AddDbContext<ProductDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });
        builder.UseEnvironment("Development");
        base.ConfigureWebHost(builder);
    }
}