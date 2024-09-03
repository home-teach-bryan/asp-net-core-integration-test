using System.Net.Http.Json;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using Azure.Core;
using FluentAssertions;

namespace AspNetCoreIntegrationTests.Tests;

public class TokenControllerTests
{
    private HttpClient _client;
    private string _baseUrl = "api/token";
    
    [SetUp]
    public void SetUp()
    {
        _client = TestSetup.Factory.CreateClient();
    }

    [TestCase("Admin", "Admin")]
    [TestCase("User", "User")]
    public async Task Token_GetToken_ReturnSuccess(string name, string password)
    {
        // arrange
        var getTokenRequest = new GetTokenRequest
        {
            Name = name,
            Password = password
        };
        // act
        var response = await _client.PostAsJsonAsync(_baseUrl, getTokenRequest);
        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        result.Status.Should().Be(ApiResponseStatus.Success);
        result.Data.Should().NotBeEmpty();
    }

    [Test]
    public async Task Token_GetToken_ReturnFail()
    {
        // arrange
        var getTokenRequest = new GetTokenRequest
        {
            Name = "UserA",
            Password = "UserA"
        };
        // act
        var response = await _client.PostAsJsonAsync(_baseUrl, getTokenRequest);
        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        result.Status.Should().Be(ApiResponseStatus.UserNotFound);
        result.Data.Should().NotBeEmpty();
    }

    [TestCase("Admin", "Admin", new[] { "Admin" })]
    [TestCase("User", "User", new[] { "User" })]
    [TestCase("SuperAdmin", "SuperAdmin", new[] { "Admin", "User" })]
    public async Task Token_GetRoles_Return_UserRoles(string name, string password, string[] roles)
    {
        // arrange for token
        var getTokenRequest = new GetTokenRequest
        {
            Name = name,
            Password = password
        };
        
        // act for token
        var response = await _client.PostAsJsonAsync(_baseUrl, getTokenRequest);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var getTokenResult = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        
        // assert for token
        getTokenResult.Status.Should().Be(ApiResponseStatus.Success);
        getTokenResult.Data.Should().NotBeEmpty();
        
        // arrange for get roles
        var token = getTokenResult.Data;
        _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        // act for get roles
        var getRolesResult = await _client.GetFromJsonAsync<ApiResponse<string[]>>($"{this._baseUrl}/Roles");
        
        // assert for get roles
        getRolesResult.Status.Should().Be(ApiResponseStatus.Success);
        getRolesResult.Data.Should().BeEquivalentTo(roles);
        

    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}