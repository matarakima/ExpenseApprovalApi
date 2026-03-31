using ExpenseApproval.Application.UseCases;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
using FluentAssertions;

namespace ExpenseApproval.Tests.Mappers;

public class ExpenseRequestMapperTests
{
    [Fact]
    public void MapToDto_FullEntity_MapsAllFields()
    {
        var catId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var decisionById = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = catId,
            Category = new Category { Id = catId, Name = "Travel" },
            Description = "Flight to Madrid",
            Amount = 450.50m,
            ExpenseDate = now.AddDays(-5),
            RequestedById = userId,
            RequestedBy = new AppUser { Id = userId, FullName = "John Doe" },
            Status = ExpenseStatus.Approved,
            CreatedAt = now.AddDays(-3),
            DecisionDate = now,
            DecisionById = decisionById,
            DecisionBy = new AppUser { Id = decisionById, FullName = "Jane Admin" }
        };

        var dto = ExpenseRequestMapper.MapToDto(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Category.Should().Be("Travel");
        dto.Description.Should().Be("Flight to Madrid");
        dto.Amount.Should().Be(450.50m);
        dto.ExpenseDate.Should().Be(entity.ExpenseDate);
        dto.RequestedBy.Should().Be("John Doe");
        dto.Status.Should().Be("Approved");
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.DecisionDate.Should().Be(now);
        dto.DecisionBy.Should().Be("Jane Admin");
    }

    [Fact]
    public void MapToDto_NullNavigationProperties_DefaultsToEmpty()
    {
        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Category = null!,
            Description = "Test",
            Amount = 100m,
            ExpenseDate = DateTime.UtcNow,
            RequestedById = Guid.NewGuid(),
            RequestedBy = null!,
            Status = ExpenseStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            DecisionDate = null,
            DecisionBy = null
        };

        var dto = ExpenseRequestMapper.MapToDto(entity);

        dto.Category.Should().BeEmpty();
        dto.RequestedBy.Should().BeEmpty();
        dto.DecisionDate.Should().BeNull();
        dto.DecisionBy.Should().BeNull();
    }

    [Fact]
    public void MapToDto_PendingStatus_MapsCorrectStatusString()
    {
        var entity = CreateEntity(ExpenseStatus.Pending);
        var dto = ExpenseRequestMapper.MapToDto(entity);
        dto.Status.Should().Be("Pending");
    }

    [Fact]
    public void MapToDto_RejectedStatus_MapsCorrectStatusString()
    {
        var entity = CreateEntity(ExpenseStatus.Rejected);
        var dto = ExpenseRequestMapper.MapToDto(entity);
        dto.Status.Should().Be("Rejected");
    }

    private static ExpenseRequest CreateEntity(ExpenseStatus status) => new()
    {
        Id = Guid.NewGuid(),
        CategoryId = Guid.NewGuid(),
        Category = new Category { Id = Guid.NewGuid(), Name = "X" },
        Description = "D",
        Amount = 10m,
        ExpenseDate = DateTime.UtcNow,
        RequestedById = Guid.NewGuid(),
        RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "U" },
        Status = status
    };
}

public class RoleMapperTests
{
    [Fact]
    public void MapToDto_WithClaims_MapsAllFields()
    {
        var roleId = Guid.NewGuid();
        var role = new AppRole
        {
            Id = roleId,
            Name = "Admin",
            Claims = new List<AppRoleClaim>
            {
                new() { Id = Guid.NewGuid(), RoleId = roleId, ClaimValue = "expenses:list" },
                new() { Id = Guid.NewGuid(), RoleId = roleId, ClaimValue = "expenses:approve" }
            }
        };

        var dto = RoleMapper.MapToDto(role);

        dto.Id.Should().Be(roleId);
        dto.Name.Should().Be("Admin");
        dto.Claims.Should().HaveCount(2);
        dto.Claims.Should().Contain("expenses:list");
        dto.Claims.Should().Contain("expenses:approve");
    }

    [Fact]
    public void MapToDto_NullClaims_ReturnsEmptyClaims()
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Basic",
            Claims = null!
        };

        var dto = RoleMapper.MapToDto(role);

        dto.Name.Should().Be("Basic");
        dto.Claims.Should().BeEmpty();
    }

    [Fact]
    public void MapToDto_EmptyClaims_ReturnsEmptyClaims()
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Viewer",
            Claims = new List<AppRoleClaim>()
        };

        var dto = RoleMapper.MapToDto(role);

        dto.Claims.Should().BeEmpty();
    }
}

public class UserMapperTests
{
    [Fact]
    public void MapToDto_WithRole_MapsAllFields()
    {
        var roleId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = "auth0|abc123",
            Email = "admin@company.com",
            FullName = "Admin User",
            RoleId = roleId,
            Role = new AppRole
            {
                Id = roleId,
                Name = "Administrator",
                Claims = new List<AppRoleClaim>
                {
                    new() { Id = Guid.NewGuid(), ClaimValue = "expenses:approve" },
                    new() { Id = Guid.NewGuid(), ClaimValue = "expenses:reject" }
                }
            }
        };

        var dto = UserMapper.MapToDto(user);

        dto.Id.Should().Be(user.Id);
        dto.Auth0Id.Should().Be("auth0|abc123");
        dto.Email.Should().Be("admin@company.com");
        dto.FullName.Should().Be("Admin User");
        dto.RoleName.Should().Be("Administrator");
        dto.Claims.Should().HaveCount(2);
        dto.Claims.Should().Contain("expenses:approve");
    }

    [Fact]
    public void MapToDto_NullRole_DefaultsToEmpty()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = "auth0|xyz",
            Email = "user@test.com",
            FullName = "Test",
            Role = null!
        };

        var dto = UserMapper.MapToDto(user);

        dto.RoleName.Should().BeEmpty();
        dto.Claims.Should().BeEmpty();
    }

    [Fact]
    public void MapToDto_RoleWithNullClaims_ReturnsEmptyClaims()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = "auth0|123",
            Email = "user@test.com",
            FullName = "Test",
            Role = new AppRole { Id = Guid.NewGuid(), Name = "Basic", Claims = null! }
        };

        var dto = UserMapper.MapToDto(user);

        dto.RoleName.Should().Be("Basic");
        dto.Claims.Should().BeEmpty();
    }
}
