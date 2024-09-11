using System.Net;
using System.Net.Http.Json;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using AspNetCoreIntegrationTest.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreIntegrationTests.Tests;

[TestFixture]
public class ProductControllerTests
{
    private string _productEndpoint = "api/Product";
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {

    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }

    [TestCase("Admin", "Admin", "測試案例產品1")]
    [TestCase("SuperAdmin", "SuperAdmin", "測試案例產品2")]
    public async Task Product_AdminRole_AddNonExistProduct_ReturnSuccess(string name, string password,
        string productName)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addProductRequest = new AddProductRequest
        {
            Name = productName,
            Price = 100,
            Quantity = 10
        };
        // act
        var response = await _client.PostAsJsonAsync(_productEndpoint, addProductRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        result.Status.Should().Be(ApiResponseStatus.Success);
    }

    [TestCase("User", "User")]
    public async Task Product_UserRole_AddNonExistProduct_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addProductRequest = new AddProductRequest
        {
            Name = "產品1",
            Price = 100,
            Quantity = 10
        };
        // act
        var response = await _client.PostAsJsonAsync(_productEndpoint, addProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status403Forbidden);
    }

    [TestCase("Admin", "Admin")]
    public async Task Product_AddNotExistProduct_WithFieldValidation_ReturnBadRequest(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addProductRequest = new AddProductRequest
        {
            Price = 100,
            Quantity = 10
        };
        // act
        var result = await _client.PostAsJsonAsync(_productEndpoint, addProductRequest);
        // assert
        result.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
    }

    [TestCase("Admin", "Admin")]
    public async Task Product_AddExistProduct_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addProductRequest = new AddProductRequest
        {
            Name = "產品1",
            Price = 100,
            Quantity = 1
        };
        // act
        var result = await _client.PostAsJsonAsync(_productEndpoint, addProductRequest);
        // assert
        result.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
    }

    [TestCase("Admin", "Admin")]
    [TestCase("SuperAdmin", "SuperAdmin")]
    public async Task Product_AdminRole_UpdateExistProduct_ReturnSuccess(string name, string password)
    {
        
        var productIdMap = new Dictionary<string, string>
        {
            ["Admin"] = TestSetup.ExistProductId1,
            ["SuperAdmin"] = TestSetup.ExistProductId2,
        };
        
        productIdMap.TryGetValue(name, out var productId);
        
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Name = "更新產品名稱",
            Price = 1,
            Quantity = 1
        };
        // act
        var response =
            await _client.PutAsJsonAsync($"{_productEndpoint}/{productId}", updateProductRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        result.Status.Should().Be(ApiResponseStatus.Success);
    }

    [TestCase("User", "User")]
    public async Task Product_UserRole_UpdateExistProduct_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Name = "更新產品名稱",
            Price = 1,
            Quantity = 1
        };
        // act
        var response =
            await _client.PutAsJsonAsync($"{_productEndpoint}/{TestSetup.ExistProductId1}", updateProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status403Forbidden);
    }

    [TestCase("Admin", "Admin")]
    public async Task Product_UpdateNotExistProduct_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Name = "產品",
            Price = 1,
            Quantity = 1
        };
        // act
        var response = await _client.PutAsJsonAsync($"{_productEndpoint}/{Guid.NewGuid()}", updateProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Status.Should().Be(ApiResponseStatus.Fail);
    }

    [TestCase("Admin", "Admin")]
    public async Task Product_UpdateExistProduct_WithFieldValidation_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Price = 1,
            Quantity = 1
        };
        // act
        var response =
            await _client.PutAsJsonAsync($"{_productEndpoint}/{TestSetup.ExistProductId1}", updateProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
    }

    [TestCase("Admin", "Admin")]
    [TestCase("SuperAdmin", "SuperAdmin")]
    public async Task Product_AdminRole_DeleteExistProduct_ReturnSuccess(string name, string password)
    {
        var productIdMap = new Dictionary<string, string>
        {
            ["Admin"] = TestSetup.DeleteProductId1,
            ["SuperAdmin"] = TestSetup.DeleteProductId2,
        };

        productIdMap.TryGetValue(name, out var productId);

        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.DeleteAsync($"{_productEndpoint}/{productId}");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
    }

    [TestCase("User", "User")]
    public async Task Product_UserRole_DeleteExistProduct_ReturnFail(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.DeleteAsync($"{_productEndpoint}/{TestSetup.DeleteProductId1}");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status403Forbidden);
    }

    [TestCase("Admin", "Admin")]
    public async Task Product_DeleteNotExistProduct_ReturnBadRequest(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.DeleteAsync($"{_productEndpoint}/{Guid.NewGuid()}");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status400BadRequest);
    }
    
    [TestCase("Admin", "Admin")]
    [TestCase("SuperAdmin", "SuperAdmin")]
    [TestCase("User", "User")]
    public async Task Product_GetProductsWithToken_ReturnSuccess(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.GetAsync(_productEndpoint);
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetProductResponse>>>();
        result.Data.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public async Task Product_GetProductsWithoutToken_ReturnFail()
    {
        // arrange
        _client = TestSetup.Factory.CreateClient();
        // act
        var response = await _client.GetAsync(_productEndpoint);
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [TestCase("Admin", "Admin")]
    [TestCase ("SuperAdmin", "SuperAdmin")]
    [TestCase("User", "User")]
    public async Task Product_GetProduct_WithToken_ReturnSuccess(string name, string password)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.GetAsync($"{_productEndpoint}/{TestSetup.ExistProductId1}");
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<GetProductResponse>>();
        result.Data.Should().NotBeNull();
    }
    
    
    [Test]
    public async Task Product_GetProduct_WithoutToken_ReturnFail()
    {
        // arrange
        _client = TestSetup.Factory.CreateClient();
        // act
        var response = await _client.GetAsync($"{_productEndpoint}/{TestSetup.ExistProductId1}");
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
   