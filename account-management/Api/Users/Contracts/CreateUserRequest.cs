using PlatformPlatform.AccountManagement.Domain.Users;

namespace PlatformPlatform.AccountManagement.Api.Users.Contracts;

public sealed record CreateUserRequest(string Email, UserRole UserRole);