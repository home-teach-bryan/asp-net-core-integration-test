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
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}