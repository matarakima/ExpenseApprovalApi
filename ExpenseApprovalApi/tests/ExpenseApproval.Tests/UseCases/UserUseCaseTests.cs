using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.UseCases;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.UseCases;

public class UserCreateUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly UserCreateUseCase _useCase;

    public UserCreateUseCaseTests()
    {
        _useCase = new UserCreateUseCase(_userRepoMock.Object, _roleRepoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidUser_ReturnsDto()
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Claims = new List<AppRoleClaim>
            {
                new() { Id = Guid.NewGuid(), ClaimValue = "expenses:list" }
            }
        };

        _roleRepoMock.Setup(r => r.GetByIdAsync(role.Id)).ReturnsAsync(role);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<AppUser>()))
            .ReturnsAsync((AppUser u) => u);

        var dto = new CreateUserDto("auth0|123", "test@test.com", "Test User", role.Id);
        var result = await _useCase.ExecuteAsync(dto);

        result.Should().NotBeNull();
        result.Email.Should().Be("test@test.com");
        result.RoleName.Should().Be("Admin");
        result.Claims.Should().Contain("expenses:list");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidRole_ThrowsKeyNotFound()
    {
        _roleRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AppRole?)null);

        var dto = new CreateUserDto("auth0|123", "test@test.com", "Test User", Guid.NewGuid());
        var act = () => _useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

public class UserGetAllUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly UserGetAllUseCase _useCase;

    public UserGetAllUseCaseTests()
    {
        _useCase = new UserGetAllUseCase(_userRepoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUsers()
    {
        var users = new List<AppUser>
        {
            new()
            {
                Id = Guid.NewGuid(), Auth0Id = "auth0|1", Email = "a@a.com", FullName = "A",
                Role = new AppRole { Name = "Admin", Claims = new List<AppRoleClaim>() }
            }
        };

        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _useCase.ExecuteAsync();

        result.Should().HaveCount(1);
    }
}

public class UserGetByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly UserGetByIdUseCase _useCase;

    public UserGetByIdUseCaseTests()
    {
        _useCase = new UserGetByIdUseCase(_userRepoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Found_ReturnsDto()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = "auth0|found",
            Email = "found@test.com",
            FullName = "Found User",
            Role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Editor",
                Claims = new List<AppRoleClaim>
                {
                    new() { Id = Guid.NewGuid(), ClaimValue = "expenses:create" }
                }
            }
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _useCase.ExecuteAsync(user.Id);

        result.Should().NotBeNull();
        result!.Email.Should().Be("found@test.com");
        result.RoleName.Should().Be("Editor");
        result.Claims.Should().Contain("expenses:create");
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AppUser?)null);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_UserWithEmptyClaims_ReturnsEmptyClaims()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = "auth0|noclaims",
            Email = "no@claims.com",
            FullName = "No Claims",
            Role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Viewer",
                Claims = new List<AppRoleClaim>()
            }
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _useCase.ExecuteAsync(user.Id);

        result.Should().NotBeNull();
        result!.Claims.Should().BeEmpty();
    }
}
