using System.Net.Http.Json;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using FluentAssertions;

namespace AspNetCoreIntegrationTests.Tests;

[TestFixture]
public class UserControllerTests
{
    private HttpClient _client;
    private string _userEndpoint = "api/user";

    [SetUp]
    public void SetUp()
    {
        _client = TestSetup.Factory.CreateClient();
    }

    [Test]
    public async Task User_AddNonExistUser_ReturnSuccess()
    {
        // arrange
        var addUserRequest = new AddUserRequest
        {
            Name = "test",
            Password = "test",
            Roles = new string[] { "Admin" }
        };
        // act
        var response = await _client.PostAsJsonAsync(this._userEndpoint, addUserRequest);
        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
    }

    [Test]
    public async Task User_AddExistUser_ReturnFail()
    {
        // arrange
        var addUserRequest = new AddUserRequest
        {
            Name = "Admin",
            Password = "Admin",
            Roles = new string[] { "Admin" }
        };
        // act
        var response = await _client.PostAsJsonAsync(this._userEndpoint, addUserRequest);
        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Status.Should().Be(ApiResponseStatus.AddUserFail);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}