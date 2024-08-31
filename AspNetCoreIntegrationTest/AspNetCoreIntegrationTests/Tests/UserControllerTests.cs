using System.Net.Http.Json;
using AspNetCoreIntegration;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegrationTests.Factory;
using FluentAssertions;

namespace AspNetCoreIntegrationTests.Tests;

[TestFixture]
public class UserControllerTests
{
    private HttpClient _client;
    private string _baseUrl = "api/user";

    [SetUp]
    public void SetUp()
    {
        _client = TestSetup.Factory.CreateClient();
    }

    [Test]
    public async Task User_AddUser_ReturnSuccess()
    {
        // arrange
        var addUserRequest = new AddUserRequest
        {
            Name = "test",
            Password = "test",
            Roles = new string[] { "Admin" }
        };
        // act
        var response = await _client.PostAsJsonAsync(this._baseUrl, addUserRequest);
        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}