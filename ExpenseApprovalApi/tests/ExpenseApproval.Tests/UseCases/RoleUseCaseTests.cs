using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.UseCases;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.UseCases;

public class RoleCreateUseCaseTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleCreateUseCase _useCase;

    public RoleCreateUseCaseTests()
    {
        _useCase = new RoleCreateUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRole_ReturnsDto()
    {
        _repoMock.Setup(r => r.AddAsync(It.IsAny<AppRole>()))
            .ReturnsAsync((AppRole r) =>
            {
                r.Claims ??= new List<AppRoleClaim>();
                return r;
            });

        var result = await _useCase.ExecuteAsync(new CreateRoleDto("TestRole"));

        result.Should().NotBeNull();
        result.Name.Should().Be("TestRole");
    }
}

public class RoleGetAllUseCaseTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleGetAllUseCase _useCase;

    public RoleGetAllUseCaseTests()
    {
        _useCase = new RoleGetAllUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedDtos()
    {
        var roles = new List<AppRole>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin", Claims = new List<AppRoleClaim>() },
            new() { Id = Guid.NewGuid(), Name = "User", Claims = new List<AppRoleClaim>() }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(roles);

        var result = await _useCase.ExecuteAsync();

        result.Should().HaveCount(2);
    }
}

public class RoleAddClaimUseCaseTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleAddClaimUseCase _useCase;

    public RoleAddClaimUseCaseTests()
    {
        _useCase = new RoleAddClaimUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DelegatesToRepository()
    {
        _repoMock.Setup(r => r.AddClaimAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var roleId = Guid.NewGuid();
        await _useCase.ExecuteAsync(roleId, "expenses:list");

        _repoMock.Verify(r => r.AddClaimAsync(roleId, "expenses:list"), Times.Once);
    }
}

public class RoleRemoveClaimUseCaseTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleRemoveClaimUseCase _useCase;

    public RoleRemoveClaimUseCaseTests()
    {
        _useCase = new RoleRemoveClaimUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DelegatesToRepository()
    {
        _repoMock.Setup(r => r.RemoveClaimAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var roleId = Guid.NewGuid();
        await _useCase.ExecuteAsync(roleId, "expenses:list");

        _repoMock.Verify(r => r.RemoveClaimAsync(roleId, "expenses:list"), Times.Once);
    }
}

public class RoleGetByIdUseCaseTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleGetByIdUseCase _useCase;

    public RoleGetByIdUseCaseTests()
    {
        _useCase = new RoleGetByIdUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Found_ReturnsDto()
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Approver",
            Claims = new List<AppRoleClaim>
            {
                new() { Id = Guid.NewGuid(), ClaimValue = "expenses:approve" }
            }
        };

        _repoMock.Setup(r => r.GetByIdAsync(role.Id)).ReturnsAsync(role);

        var result = await _useCase.ExecuteAsync(role.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Approver");
        result.Claims.Should().Contain("expenses:approve");
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AppRole?)null);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_RoleWithNoClaims_ReturnsEmptyClaimsList()
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Basic",
            Claims = new List<AppRoleClaim>()
        };

        _repoMock.Setup(r => r.GetByIdAsync(role.Id)).ReturnsAsync(role);

        var result = await _useCase.ExecuteAsync(role.Id);

        result.Should().NotBeNull();
        result!.Claims.Should().BeEmpty();
    }
}
