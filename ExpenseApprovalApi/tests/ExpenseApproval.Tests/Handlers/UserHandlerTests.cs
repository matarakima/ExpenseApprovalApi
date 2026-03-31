using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Users.Commands;
using ExpenseApproval.Application.Features.Users.Queries;
using ExpenseApproval.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.Handlers;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserCreateUseCase> _useCaseMock = new();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _handler = new CreateUserCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var roleId = Guid.NewGuid();
        var expectedDto = new UserDto(Guid.NewGuid(), "auth0|123", "test@test.com", "Test User", "Admin", new[] { "expenses:list" });

        _useCaseMock.Setup(u => u.ExecuteAsync(
            It.Is<CreateUserDto>(d => d.Email == "test@test.com" && d.RoleId == roleId)))
            .ReturnsAsync(expectedDto);

        var command = new CreateUserCommand("auth0|123", "test@test.com", "Test User", roleId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Email.Should().Be("test@test.com");
        result.RoleName.Should().Be("Admin");
        _useCaseMock.Verify(u => u.ExecuteAsync(It.IsAny<CreateUserDto>()), Times.Once);
    }
}

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserGetAllUseCase> _useCaseMock = new();
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _handler = new GetAllUsersQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsList()
    {
        var users = new List<UserDto>
        {
            new(Guid.NewGuid(), "auth0|1", "a@a.com", "A", "Admin", new[] { "expenses:list" })
        };

        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(users);

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmpty()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(Enumerable.Empty<UserDto>());

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserGetByIdUseCase> _useCaseMock = new();
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _handler = new GetUserByIdQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_Found_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var dto = new UserDto(id, "auth0|123", "test@test.com", "Test", "Admin", new[] { "expenses:list" });
        _useCaseMock.Setup(u => u.ExecuteAsync(id)).ReturnsAsync(dto);

        var result = await _handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNull()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>())).ReturnsAsync((UserDto?)null);

        var result = await _handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}
