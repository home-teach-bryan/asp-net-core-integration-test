using System.Net;
using System.Net.Http.Json;
using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using AspNetCoreIntegrationTest.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreIntegrationTests.Tests;

[TestFixture]
public class OrderControllerTests
{
    private string _orderEndpoint = "api/Order";
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public async Task Order_AddOrderWithOutToken_ReturnFail()
    {
        // arrange
        _client = TestSetup.Factory.CreateClient();
        var addOrderRequest = new List<AddOrderRequest>
        {
            new AddOrderRequest
            {
                ProductId = Guid.Parse(TestSetup.ExistProductId3),
                Quantity = 1,
            }
        };
        // act
        var response = await _client.PostAsJsonAsync(_orderEndpoint, addOrderRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status401Unauthorized);
    }
    
    [TestCase("Admin", "Admin")]
    public async Task Order_AdminRole_AddOrder_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addOrderRequest = new List<AddOrderRequest>
        {
            new AddOrderRequest
            {
                ProductId = Guid.Parse(TestSetup.ExistProductId3),
                Quantity = 1,
            },
            new AddOrderRequest
            {
                ProductId = Guid.Parse(TestSetup.ExistProductId4),
                Quantity = 1,
            },
        };
        // act
        var response = await _client.PostAsJsonAsync(_orderEndpoint, addOrderRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status403Forbidden);
    }

    [TestCase("User", "User")]
    [TestCase("SuperAdmin", "SuperAdmin")]
    public async Task Order_UserRole_AddOrder_ReturnSuccess(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addOrderRequest = new List<AddOrderRequest>
        {
            new AddOrderRequest
            {
                ProductId = Guid.Parse(TestSetup.ExistProductId3),
                Quantity = 1,
            }
        };
        // act
        var response = await _client.PostAsJsonAsync(_orderEndpoint, addOrderRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Status.Should().Be(ApiResponseStatus.Success);
    }

    [TestCase("User", "User")]
    public async Task Order_UserRole_AddOrder_OverQuantity_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addOrderRequest = new List<AddOrderRequest>
        {
            new AddOrderRequest
            {
                ProductId = Guid.Parse(TestSetup.ExistProductId4),
                Quantity = 500,
            },
        };
        // act
        var response = await _client.PostAsJsonAsync(_orderEndpoint, addOrderRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Status.Should().Be(ApiResponseStatus.AddOrderFail);
    }

    [TestCase("User", "User")]
    public async Task Order_UserRole_GetOrderDetails_ReturnSuccess(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.GetAsync($"{_orderEndpoint}/OrderDetails");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<GetOrderDetailsResponse>>>();
        result.Status.Should().Be(ApiResponseStatus.Success);
        result.Data.Should().NotBeEmpty();
        result.Data.Count().Should().Be(2);
    }

    [TestCase("User1", "User1")]
    public async Task Order_UserRole_GetOrderDetails_ReturnEmpty(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.GetAsync($"{_orderEndpoint}/OrderDetails");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<GetOrderDetailsResponse>>>();
        result.Status.Should().Be(ApiResponseStatus.Success);
        result.Data.Count().Should().Be(0);
    }

    [TestCase("Admin", "Admin")]
    public async Task Order_AdminRole_GetOrderDetails_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.GetAsync($"{_orderEndpoint}/OrderDetails");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status403Forbidden);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}