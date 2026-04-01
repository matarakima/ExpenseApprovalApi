using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Commands;
using ExpenseApproval.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.Handlers;

public class CreateExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRequestCreateUseCase> _useCaseMock = new();
    private readonly CreateExpenseCommandHandler _handler;

    public CreateExpenseCommandHandlerTests()
    {
        _handler = new CreateExpenseCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var categoryId = Guid.NewGuid();
        var requestedById = Guid.NewGuid();
        var expectedDto = new ExpenseRequestDto(
            Guid.NewGuid(), categoryId, "Travel", "Flight", 500m, DateTime.UtcNow.AddDays(-1),
            "User", requestedById, "Pending", DateTime.UtcNow, null, null, null);

        _useCaseMock.Setup(u => u.ExecuteAsync(
            It.Is<CreateExpenseRequestDto>(d => d.CategoryId == categoryId && d.Amount == 500m && d.RequestedById == requestedById)))
            .ReturnsAsync(expectedDto);

        var command = new CreateExpenseCommand(categoryId, "Flight", 500m, DateTime.UtcNow.AddDays(-1), requestedById);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedDto);
        _useCaseMock.Verify(u => u.ExecuteAsync(It.IsAny<CreateExpenseRequestDto>()), Times.Once);
    }
}

public class UpdateExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRequestUpdateUseCase> _useCaseMock = new();
    private readonly UpdateExpenseCommandHandler _handler;

    public UpdateExpenseCommandHandlerTests()
    {
        _handler = new UpdateExpenseCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var expectedDto = new ExpenseRequestDto(
            id, categoryId, "Office", "Supplies", 200m, DateTime.UtcNow.AddDays(-1),
            "User", Guid.NewGuid(), "Pending", DateTime.UtcNow, null, null, null);

        _useCaseMock.Setup(u => u.ExecuteAsync(
            id,
            It.Is<UpdateExpenseRequestDto>(d => d.CategoryId == categoryId && d.Amount == 200m)))
            .ReturnsAsync(expectedDto);

        var command = new UpdateExpenseCommand(id, categoryId, "Supplies", 200m, DateTime.UtcNow.AddDays(-1));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedDto);
        _useCaseMock.Verify(u => u.ExecuteAsync(id, It.IsAny<UpdateExpenseRequestDto>()), Times.Once);
    }
}

public class ApproveExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRequestApproveUseCase> _useCaseMock = new();
    private readonly ApproveExpenseCommandHandler _handler;

    public ApproveExpenseCommandHandlerTests()
    {
        _handler = new ApproveExpenseCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var decisionById = Guid.NewGuid();
        var expectedDto = new ExpenseRequestDto(
            id, Guid.NewGuid(), "Travel", "Flight", 500m, DateTime.UtcNow.AddDays(-1),
            "User", Guid.NewGuid(), "Approved", DateTime.UtcNow, DateTime.UtcNow, "Approver", decisionById);

        _useCaseMock.Setup(u => u.ExecuteAsync(id, decisionById))
            .ReturnsAsync(expectedDto);

        var command = new ApproveExpenseCommand(id, decisionById);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedDto);
        result.Status.Should().Be("Approved");
        _useCaseMock.Verify(u => u.ExecuteAsync(id, decisionById), Times.Once);
    }
}

public class RejectExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRequestRejectUseCase> _useCaseMock = new();
    private readonly RejectExpenseCommandHandler _handler;

    public RejectExpenseCommandHandlerTests()
    {
        _handler = new RejectExpenseCommandHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var decisionById = Guid.NewGuid();
        var expectedDto = new ExpenseRequestDto(
            id, Guid.NewGuid(), "Travel", "Flight", 500m, DateTime.UtcNow.AddDays(-1),
            "User", Guid.NewGuid(), "Rejected", DateTime.UtcNow, DateTime.UtcNow, "Rejector", decisionById);

        _useCaseMock.Setup(u => u.ExecuteAsync(id, decisionById))
            .ReturnsAsync(expectedDto);

        var command = new RejectExpenseCommand(id, decisionById);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedDto);
        result.Status.Should().Be("Rejected");
        _useCaseMock.Verify(u => u.ExecuteAsync(id, decisionById), Times.Once);
    }
}
