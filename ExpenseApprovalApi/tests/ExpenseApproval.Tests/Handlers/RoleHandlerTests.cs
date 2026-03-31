using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Roles.Commands;
using ExpenseApproval.Application.Features.Roles.Queries;
using ExpenseApproval.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.Handlers;

public class CreateRoleCommandHandlerTests
{
    private readonly Mock<IRoleCreateUseCase> _useCaseMock = new();
    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _handler = new CreateRoleCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var expectedDto = new RoleDto(Guid.NewGuid(), "Admin", new[] { "expenses:list" });

        _useCaseMock.Setup(u => u.ExecuteAsync(It.Is<CreateRoleDto>(d => d.Name == "Admin")))
            .ReturnsAsync(expectedDto);

        var command = new CreateRoleCommand("Admin");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Admin");
        _useCaseMock.Verify(u => u.ExecuteAsync(It.IsAny<CreateRoleDto>()), Times.Once);
    }
}

public class GetAllRolesQueryHandlerTests
{
    private readonly Mock<IRoleGetAllUseCase> _useCaseMock = new();
    private readonly GetAllRolesQueryHandler _handler;

    public GetAllRolesQueryHandlerTests()
    {
        _handler = new GetAllRolesQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsList()
    {
        var roles = new List<RoleDto>
        {
            new(Guid.NewGuid(), "Admin", new[] { "expenses:list" }),
            new(Guid.NewGuid(), "User", Enumerable.Empty<string>())
        };

        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(roles);

        var result = await _handler.Handle(new GetAllRolesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}

public class GetRoleByIdQueryHandlerTests
{
    private readonly Mock<IRoleGetByIdUseCase> _useCaseMock = new();
    private readonly GetRoleByIdQueryHandler _handler;

    public GetRoleByIdQueryHandlerTests()
    {
        _handler = new GetRoleByIdQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_Found_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var dto = new RoleDto(id, "Admin", new[] { "expenses:list" });
        _useCaseMock.Setup(u => u.ExecuteAsync(id)).ReturnsAsync(dto);

        var result = await _handler.Handle(new GetRoleByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNull()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>())).ReturnsAsync((RoleDto?)null);

        var result = await _handler.Handle(new GetRoleByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}
