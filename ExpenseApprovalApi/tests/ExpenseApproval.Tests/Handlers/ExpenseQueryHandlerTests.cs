using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Queries;
using ExpenseApproval.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseApproval.Tests.Handlers;

public class GetAllExpensesQueryHandlerTests
{
    private readonly Mock<IExpenseRequestGetAllUseCase> _useCaseMock = new();
    private readonly GetAllExpensesQueryHandler _handler;

    public GetAllExpensesQueryHandlerTests()
    {
        _handler = new GetAllExpensesQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsList()
    {
        var expenses = new List<ExpenseRequestDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Travel", "Flight", 500m, DateTime.UtcNow, "User", Guid.NewGuid(), "Pending", DateTime.UtcNow, null, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "Office", "Supplies", 100m, DateTime.UtcNow, "User", Guid.NewGuid(), "Approved", DateTime.UtcNow, DateTime.UtcNow, "Admin", Guid.NewGuid())
        };

        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(expenses);

        var result = await _handler.Handle(new GetAllExpensesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        _useCaseMock.Verify(u => u.ExecuteAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmpty()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(Enumerable.Empty<ExpenseRequestDto>());

        var result = await _handler.Handle(new GetAllExpensesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

public class GetExpenseByIdQueryHandlerTests
{
    private readonly Mock<IExpenseRequestGetByIdUseCase> _useCaseMock = new();
    private readonly GetExpenseByIdQueryHandler _handler;

    public GetExpenseByIdQueryHandlerTests()
    {
        _handler = new GetExpenseByIdQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_Found_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var dto = new ExpenseRequestDto(id, Guid.NewGuid(), "Travel", "Flight", 500m, DateTime.UtcNow, "User", Guid.NewGuid(), "Pending", DateTime.UtcNow, null, null, null);
        _useCaseMock.Setup(u => u.ExecuteAsync(id)).ReturnsAsync(dto);

        var result = await _handler.Handle(new GetExpenseByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNull()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseRequestDto?)null);

        var result = await _handler.Handle(new GetExpenseByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}

public class FilterExpensesQueryHandlerTests
{
    private readonly Mock<IExpenseRequestFilterUseCase> _useCaseMock = new();
    private readonly FilterExpensesQueryHandler _handler;

    public FilterExpensesQueryHandlerTests()
    {
        _handler = new FilterExpensesQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_WithCorrectDto()
    {
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;

        _useCaseMock.Setup(u => u.ExecuteAsync(
            It.Is<FilterExpenseRequestDto>(f => f.Status == "Pending" && f.Category == "Travel" && f.FromDate == from && f.ToDate == to)))
            .ReturnsAsync(new List<ExpenseRequestDto>());

        var query = new FilterExpensesQuery("Pending", "Travel", from, to);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
        _useCaseMock.Verify(u => u.ExecuteAsync(It.IsAny<FilterExpenseRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFilters_PassesNulls()
    {
        _useCaseMock.Setup(u => u.ExecuteAsync(
            It.Is<FilterExpenseRequestDto>(f => f.Status == null && f.Category == null)))
            .ReturnsAsync(new List<ExpenseRequestDto>());

        var query = new FilterExpensesQuery(null, null, null, null);
        await _handler.Handle(query, CancellationToken.None);

        _useCaseMock.Verify(u => u.ExecuteAsync(
            It.Is<FilterExpenseRequestDto>(f => f.Status == null && f.Category == null)), Times.Once);
    }
}

public class GetExpenseMetricsQueryHandlerTests
{
    private readonly Mock<IExpenseRequestGetMetricsUseCase> _useCaseMock = new();
    private readonly GetExpenseMetricsQueryHandler _handler;

    public GetExpenseMetricsQueryHandlerTests()
    {
        _handler = new GetExpenseMetricsQueryHandler(_useCaseMock.Object);
    }

    [Fact]
    public async Task Handle_DelegatesToUseCase_ReturnsMetrics()
    {
        var metrics = new ExpenseMetricsDto(10, 5, 2, 3, 2500m);
        _useCaseMock.Setup(u => u.ExecuteAsync()).ReturnsAsync(metrics);

        var result = await _handler.Handle(new GetExpenseMetricsQuery(), CancellationToken.None);

        result.TotalRequests.Should().Be(10);
        result.ApprovedCount.Should().Be(5);
        result.TotalApprovedAmount.Should().Be(2500m);
        _useCaseMock.Verify(u => u.ExecuteAsync(), Times.Once);
    }
}
