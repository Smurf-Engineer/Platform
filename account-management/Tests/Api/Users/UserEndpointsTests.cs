using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PlatformPlatform.AccountManagement.Application.Users;
using PlatformPlatform.AccountManagement.Domain.Users;
using PlatformPlatform.AccountManagement.Infrastructure;
using Xunit;

namespace PlatformPlatform.AccountManagement.Tests.Api.Users;

public sealed class UserEndpointsTests : BaseApiTests<AccountManagementDbContext>
{
    [Fact]
    public async Task CreateUser_WhenValid_ShouldCreateUser()
    {
        // Act
        var command = new CreateUser.Command(DatabaseSeeder.Tenant1Id, "test@test.com", UserRole.TenantUser);
        var response = await TestHttpClient.PostAsJsonAsync("/api/users", command);

        // Assert
        await EnsureSuccessPostRequest(response, startsWith: "/api/users/");
        response.Headers.Location!.ToString().Length.Should().Be($"/api/users/{UserId.NewId()}".Length);
    }

    [Fact]
    public async Task CreateUser_WhenInvalid_ShouldReturnBadRequest()
    {
        // Act
        var command = new CreateUser.Command(DatabaseSeeder.Tenant1Id, "a", UserRole.TenantOwner);
        var response = await TestHttpClient.PostAsJsonAsync("/api/users", command);

        // Assert
        const string expectedBody =
            """{"type":"https://httpstatuses.com/400","title":"Bad Request","status":400,"Errors":[{"code":"Email","message":"'Email' is not a valid email address."}]}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.BadRequest, expectedBody);
    }

    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = DatabaseSeeder.User1Id;

        // Act
        var response = await TestHttpClient.GetAsync($"/api/users/{userId}");

        // Assert
        EnsureSuccessGetRequest(response);

        var userDto = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        const string userEmail = DatabaseSeeder.User1Email;
        var createdAt = userDto?.CreatedAt.ToString(Iso8601TimeFormat);
        var expectedBody =
            $$"""{"id":"{{userId}}","createdAt":"{{createdAt}}","modifiedAt":null,"email":"{{userEmail}}","userRole":0}""";
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Be(expectedBody);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await TestHttpClient.GetAsync("/api/users/999");

        // Assert
        const string expectedBody =
            """{"type":"https://httpstatuses.com/404","title":"Not Found","status":404,"detail":"User with id '999' not found."}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.NotFound, expectedBody);
    }

    [Fact]
    public async Task UpdateUser_WhenValid_ShouldUpdateUser()
    {
        // Act
        var command = new UpdateUser.Command {Email = "updated@test.com", UserRole = UserRole.TenantOwner};
        var response = await TestHttpClient.PutAsJsonAsync($"/api/users/{DatabaseSeeder.User1Id}", command);

        // Assert
        EnsureSuccessPutRequest(response);
    }

    [Fact]
    public async Task UpdateUser_WhenInvalid_ShouldReturnBadRequest()
    {
        // Act
        var command = new UpdateUser.Command {Email = "Invalid Email", UserRole = UserRole.TenantAdmin};
        var response = await TestHttpClient.PutAsJsonAsync($"/api/users/{DatabaseSeeder.User1Id}", command);

        // Assert
        const string expectedBody =
            """{"type":"https://httpstatuses.com/400","title":"Bad Request","status":400,"Errors":[{"code":"Email","message":"'Email' is not a valid email address."}]}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.BadRequest, expectedBody);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExists_ShouldReturnNotFound()
    {
        // Act
        var command = new UpdateUser.Command {Email = "updated@test.com", UserRole = UserRole.TenantAdmin};
        var response = await TestHttpClient.PutAsJsonAsync("/api/users/999", command);

        //Assert
        const string expectedBody =
            """{"type":"https://httpstatuses.com/404","title":"Not Found","status":404,"detail":"User with id '999' not found."}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.NotFound, expectedBody);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExists_ShouldReturnNotFound()
    {
        // Act
        var response = await TestHttpClient.DeleteAsync("/api/users/999");

        //Assert
        const string expectedBody =
            """{"type":"https://httpstatuses.com/404","title":"Not Found","status":404,"detail":"User with id '999' not found."}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.NotFound, expectedBody);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldDeleteUser()
    {
        // Act
        var response = await TestHttpClient.DeleteAsync($"/api/users/{DatabaseSeeder.User1Id}");

        // Assert
        EnsureSuccessDeleteRequest(response);

        // Verify that User is deleted
        response = await TestHttpClient.GetAsync($"/api/users/{DatabaseSeeder.User1Id}");
        var expectedBody =
            $$"""{"type":"https://httpstatuses.com/404","title":"Not Found","status":404,"detail":"User with id '{{DatabaseSeeder.User1Id}}' not found."}""";
        await EnsureErrorStatusCode(response, HttpStatusCode.NotFound, expectedBody);
    }
}