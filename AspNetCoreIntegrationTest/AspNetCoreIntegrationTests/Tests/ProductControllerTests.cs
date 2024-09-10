using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework.Internal;

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

    [TestCase("Admin", "Admin", "測試案例產品1", StatusCodes.Status200OK, ApiResponseStatus.Success)]
    [TestCase("SuperAdmin", "SuperAdmin", "測試案例產品2", StatusCodes.Status200OK, ApiResponseStatus.Success)]
    public async Task Product_AdminRole_AddNonExistProduct_ReturnSuccess(string name, string password,
        string productName, int httpStatusCode, ApiResponseStatus apiResponseStatus)
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
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
        result.Status.Should().Be(apiResponseStatus);
    }

    [TestCase("User", "User", StatusCodes.Status403Forbidden)]
    public async Task Product_UserRole_AddNonExistProduct_ReturnFail(string name, string password, int httpStatusCode)
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
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", StatusCodes.Status400BadRequest)]
    public async Task Product_AddNotExistProduct_WithFieldValidation_ReturnBadRequest(string name, string password,
        int httpStatusCode)
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
        result.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", StatusCodes.Status400BadRequest)]
    public async Task Product_AddExistProduct_ReturnFail(string name, string password, int httpStatusCode)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var addProductRequest = new AddProductRequest
        {
            Name = "更新產品1",
            Price = 100,
            Quantity = 1
        };
        // act
        var result = await _client.PostAsJsonAsync(_productEndpoint, addProductRequest);
        // assert
        result.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", "產品1", StatusCodes.Status200OK, ApiResponseStatus.Success)]
    [TestCase("SuperAdmin", "SuperAdmin", "產品1", StatusCodes.Status200OK, ApiResponseStatus.Success)]
    public async Task Product_AdminRole_UpdateExistProduct_ReturnSuccess(string name, string password,
        string productName, int httpStatusCode, ApiResponseStatus apiResponseStatus)
    {
        
        var productIdMap = new Dictionary<string, string>
        {
            ["Admin"] = TestSetup.UpdateProductId1,
            ["SuperAdmin"] = TestSetup.UpdateProductId2,
        };
        
        productIdMap.TryGetValue(name, out var productId);
        
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Name = productName,
            Price = 1,
            Quantity = 1
        };
        // act
        var response =
            await _client.PutAsJsonAsync($"{_productEndpoint}/{productId}", updateProductRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
        result.Status.Should().Be(apiResponseStatus);
    }

    [TestCase("User", "User", "產品1", StatusCodes.Status403Forbidden)]
    public async Task Product_UserRole_UpdateExistProduct_ReturnFail(string name, string password, string productName,
        int httpStatusCode)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        var updateProductRequest = new UpdateProductRequest
        {
            Name = productName,
            Price = 1,
            Quantity = 1
        };
        // act
        var response =
            await _client.PutAsJsonAsync($"{_productEndpoint}/{TestSetup.UpdateProductId1}", updateProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", StatusCodes.Status400BadRequest, ApiResponseStatus.Fail)]
    public async Task Product_UpdateNotExistProduct_ReturnFail(string name, string password, int httpStatusCode,
        ApiResponseStatus apiResponseStatus)
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
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Status.Should().Be(apiResponseStatus);
    }

    [TestCase("Admin", "Admin", "產品1", StatusCodes.Status400BadRequest, ApiResponseStatus.Fail)]
    public async Task Product_UpdateExistProduct_WithFieldValidation_ReturnFail(string name, string password,
        string productName,
        int httpStatusCode, ApiResponseStatus apiResponseStatus)
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
            await _client.PutAsJsonAsync($"{_productEndpoint}/{TestSetup.UpdateProductId1}", updateProductRequest);
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", StatusCodes.Status200OK)]
    [TestCase("SuperAdmin", "SuperAdmin", StatusCodes.Status200OK)]
    public async Task Product_AdminRole_DeleteExistProduct_ReturnSuccess(string name, string password,
        int httpStatusCode)
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
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("User", "User", StatusCodes.Status403Forbidden)]
    public async Task Product_UserRole_DeleteExistProduct_ReturnFail(string name, string password, int httpStatusCode)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.DeleteAsync($"{_productEndpoint}/{TestSetup.DeleteProductId1}");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }

    [TestCase("Admin", "Admin", StatusCodes.Status400BadRequest)]
    public async Task Product_DeleteNotExistProduct_ReturnBadRequest(string name, string password,
        int httpStatusCode)
    {
        // arrange
        _client = await TestSetup.GenerateClientWithTokenAsync(name, password);
        // act
        var response = await _client.DeleteAsync($"{_productEndpoint}/{Guid.NewGuid()}");
        // assert
        response.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
    }
}
   