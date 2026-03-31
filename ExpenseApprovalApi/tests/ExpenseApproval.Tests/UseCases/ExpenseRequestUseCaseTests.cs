using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.UseCases;
using ExpenseApproval.Application.Validators;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExpenseApproval.Tests.UseCases;

public class ExpenseRequestCreateUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestCreateUseCase _useCase;

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Guid _requestedById = Guid.NewGuid();

    public ExpenseRequestCreateUseCaseTests()
    {
        var validator = new CreateExpenseRequestValidator();
        var loggerMock = new Mock<ILogger<ExpenseRequestCreateUseCase>>();
        _useCase = new ExpenseRequestCreateUseCase(_repoMock.Object, validator, loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsDto()
    {
        var dto = new CreateExpenseRequestDto(_categoryId, "Vuelo tickets", 500m, DateTime.UtcNow.AddDays(-1));
        _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseRequest>()))
            .ReturnsAsync((ExpenseRequest e) =>
            {
                e.Category = new Category { Id = _categoryId, Name = "Viajes" };
                e.RequestedBy = new AppUser { Id = _requestedById, FullName = "Test User" };
                return e;
            });

        var result = await _useCase.ExecuteAsync(dto, _requestedById);

        result.Should().NotBeNull();
        result.Category.Should().Be("Viajes");
        result.Status.Should().Be("Pending");
        result.RequestedBy.Should().Be("Test User");
    }

    [Fact]
    public async Task ExecuteAsync_ZeroAmount_ThrowsValidation()
    {
        var dto = new CreateExpenseRequestDto(_categoryId, "Vuelo", 0m, DateTime.UtcNow.AddDays(-1));

        var act = () => _useCase.ExecuteAsync(dto, _requestedById);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_NegativeAmount_ThrowsValidation()
    {
        var dto = new CreateExpenseRequestDto(_categoryId, "Vuelo", -100m, DateTime.UtcNow.AddDays(-1));

        var act = () => _useCase.ExecuteAsync(dto, _requestedById);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_FutureDate_ThrowsValidation()
    {
        var dto = new CreateExpenseRequestDto(_categoryId, "Vuelo", 500m, DateTime.UtcNow.AddDays(10));

        var act = () => _useCase.ExecuteAsync(dto, _requestedById);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_EmptyCategoryId_ThrowsValidation()
    {
        var dto = new CreateExpenseRequestDto(Guid.Empty, "Vuelo", 500m, DateTime.UtcNow.AddDays(-1));

        var act = () => _useCase.ExecuteAsync(dto, _requestedById);

        await act.Should().ThrowAsync<ValidationException>();
    }
}

public class ExpenseRequestUpdateUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestUpdateUseCase _useCase;

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Guid _newCategoryId = Guid.NewGuid();
    private readonly Guid _requestedById = Guid.NewGuid();

    public ExpenseRequestUpdateUseCaseTests()
    {
        var validator = new UpdateExpenseRequestValidator();
        var loggerMock = new Mock<ILogger<ExpenseRequestUpdateUseCase>>();
        _useCase = new ExpenseRequestUpdateUseCase(_repoMock.Object, validator, loggerMock.Object);
    }

    private ExpenseRequest CreatePendingEntity(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        CategoryId = _categoryId,
        Category = new Category { Id = _categoryId, Name = "Viajes" },
        Description = "Old",
        Amount = 100m,
        ExpenseDate = DateTime.UtcNow.AddDays(-2),
        RequestedById = _requestedById,
        RequestedBy = new AppUser { Id = _requestedById, FullName = "Test User" },
        Status = ExpenseStatus.Pending
    };

    [Fact]
    public async Task ExecuteAsync_PendingRequest_Succeeds()
    {
        var entity = CreatePendingEntity();
        entity.Category = new Category { Id = _newCategoryId, Name = "Office" };

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExpenseRequest>())).Returns(Task.CompletedTask);

        var dto = new UpdateExpenseRequestDto(_newCategoryId, "New description", 200m, DateTime.UtcNow.AddDays(-1));
        var result = await _useCase.ExecuteAsync(entity.Id, dto);

        result.Amount.Should().Be(200m);
    }

    [Fact]
    public async Task ExecuteAsync_ApprovedRequest_ThrowsInvalidOperation()
    {
        var entity = CreatePendingEntity();
        entity.Status = ExpenseStatus.Approved;

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var dto = new UpdateExpenseRequestDto(_newCategoryId, "New", 200m, DateTime.UtcNow.AddDays(-1));
        var act = () => _useCase.ExecuteAsync(entity.Id, dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pending*");
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ThrowsKeyNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseRequest?)null);

        var dto = new UpdateExpenseRequestDto(_newCategoryId, "New", 200m, DateTime.UtcNow.AddDays(-1));
        var act = () => _useCase.ExecuteAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

public class ExpenseRequestApproveUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestApproveUseCase _useCase;

    private readonly Guid _decisionById = Guid.NewGuid();

    public ExpenseRequestApproveUseCaseTests()
    {
        var loggerMock = new Mock<ILogger<ExpenseRequestApproveUseCase>>();
        _useCase = new ExpenseRequestApproveUseCase(_repoMock.Object, loggerMock.Object);
    }

    private ExpenseRequest CreatePendingEntity() => new()
    {
        Id = Guid.NewGuid(),
        CategoryId = Guid.NewGuid(),
        Category = new Category { Id = Guid.NewGuid(), Name = "Viajes" },
        Description = "Vuelo",
        Amount = 500m,
        ExpenseDate = DateTime.UtcNow.AddDays(-2),
        RequestedById = Guid.NewGuid(),
        RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "Test User" },
        Status = ExpenseStatus.Pending
    };

    [Fact]
    public async Task ExecuteAsync_PendingRequest_SetsApproved()
    {
        var entity = CreatePendingEntity();
        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExpenseRequest>())).Returns(Task.CompletedTask);

        var result = await _useCase.ExecuteAsync(entity.Id, _decisionById);

        result.Status.Should().Be("Approved");
        result.DecisionDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_AlreadyApproved_ThrowsInvalidOperation()
    {
        var entity = CreatePendingEntity();
        entity.Status = ExpenseStatus.Approved;

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var act = () => _useCase.ExecuteAsync(entity.Id, _decisionById);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class ExpenseRequestRejectUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestRejectUseCase _useCase;

    private readonly Guid _decisionById = Guid.NewGuid();

    public ExpenseRequestRejectUseCaseTests()
    {
        var loggerMock = new Mock<ILogger<ExpenseRequestRejectUseCase>>();
        _useCase = new ExpenseRequestRejectUseCase(_repoMock.Object, loggerMock.Object);
    }

    private ExpenseRequest CreatePendingEntity() => new()
    {
        Id = Guid.NewGuid(),
        CategoryId = Guid.NewGuid(),
        Category = new Category { Id = Guid.NewGuid(), Name = "Viajes" },
        Description = "Vuelo",
        Amount = 500m,
        ExpenseDate = DateTime.UtcNow.AddDays(-2),
        RequestedById = Guid.NewGuid(),
        RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "Test User" },
        Status = ExpenseStatus.Pending
    };

    [Fact]
    public async Task ExecuteAsync_PendingRequest_SetsRejected()
    {
        var entity = CreatePendingEntity();
        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExpenseRequest>())).Returns(Task.CompletedTask);

        var result = await _useCase.ExecuteAsync(entity.Id, _decisionById);

        result.Status.Should().Be("Rejected");
        result.DecisionDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_AlreadyRejected_ThrowsInvalidOperation()
    {
        var entity = CreatePendingEntity();
        entity.Status = ExpenseStatus.Rejected;

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var act = () => _useCase.ExecuteAsync(entity.Id, _decisionById);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class ExpenseRequestGetAllUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestGetAllUseCase _useCase;

    public ExpenseRequestGetAllUseCaseTests()
    {
        _useCase = new ExpenseRequestGetAllUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedDtos()
    {
        var catId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entities = new List<ExpenseRequest>
        {
            new()
            {
                Id = Guid.NewGuid(), CategoryId = catId,
                Category = new Category { Id = catId, Name = "A" },
                Description = "D", Amount = 10, ExpenseDate = DateTime.UtcNow,
                RequestedById = userId,
                RequestedBy = new AppUser { Id = userId, FullName = "User" },
                Status = ExpenseStatus.Pending
            },
            new()
            {
                Id = Guid.NewGuid(), CategoryId = catId,
                Category = new Category { Id = catId, Name = "B" },
                Description = "E", Amount = 20, ExpenseDate = DateTime.UtcNow,
                RequestedById = userId,
                RequestedBy = new AppUser { Id = userId, FullName = "User" },
                Status = ExpenseStatus.Approved
            }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _useCase.ExecuteAsync();

        result.Should().HaveCount(2);
    }
}

public class ExpenseRequestGetByIdUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestGetByIdUseCase _useCase;

    public ExpenseRequestGetByIdUseCaseTests()
    {
        _useCase = new ExpenseRequestGetByIdUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Found_ReturnsDto()
    {
        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "A" },
            Description = "D", Amount = 10, ExpenseDate = DateTime.UtcNow,
            RequestedById = Guid.NewGuid(),
            RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "User" },
            Status = ExpenseStatus.Pending
        };

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _useCase.ExecuteAsync(entity.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseRequest?)null);

        var result = await _useCase.ExecuteAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}

public class ExpenseRequestGetMetricsUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestGetMetricsUseCase _useCase;

    public ExpenseRequestGetMetricsUseCaseTests()
    {
        _useCase = new ExpenseRequestGetMetricsUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsCorrectMetrics()
    {
        var catId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cat = new Category { Id = catId, Name = "X" };
        var user = new AppUser { Id = userId, FullName = "U" };

        var entities = new List<ExpenseRequest>
        {
            new() { Id = Guid.NewGuid(), CategoryId = catId, Category = cat, Description = "D", Amount = 100, ExpenseDate = DateTime.UtcNow, RequestedById = userId, RequestedBy = user, Status = ExpenseStatus.Approved },
            new() { Id = Guid.NewGuid(), CategoryId = catId, Category = cat, Description = "E", Amount = 200, ExpenseDate = DateTime.UtcNow, RequestedById = userId, RequestedBy = user, Status = ExpenseStatus.Approved },
            new() { Id = Guid.NewGuid(), CategoryId = catId, Category = cat, Description = "F", Amount = 50, ExpenseDate = DateTime.UtcNow, RequestedById = userId, RequestedBy = user, Status = ExpenseStatus.Rejected },
            new() { Id = Guid.NewGuid(), CategoryId = catId, Category = cat, Description = "G", Amount = 75, ExpenseDate = DateTime.UtcNow, RequestedById = userId, RequestedBy = user, Status = ExpenseStatus.Pending }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _useCase.ExecuteAsync();

        result.TotalRequests.Should().Be(4);
        result.ApprovedCount.Should().Be(2);
        result.RejectedCount.Should().Be(1);
        result.PendingCount.Should().Be(1);
        result.TotalApprovedAmount.Should().Be(300m);
    }
}

public class ExpenseRequestFilterUseCaseTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestFilterUseCase _useCase;

    public ExpenseRequestFilterUseCaseTests()
    {
        _useCase = new ExpenseRequestFilterUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidStatus_DelegatesToRepository()
    {
        _repoMock.Setup(r => r.FilterAsync(ExpenseStatus.Pending, null, null, null))
            .ReturnsAsync(new List<ExpenseRequest>());

        var filter = new FilterExpenseRequestDto("Pending", null, null, null);
        var result = await _useCase.ExecuteAsync(filter);

        result.Should().BeEmpty();
        _repoMock.Verify(r => r.FilterAsync(ExpenseStatus.Pending, null, null, null), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidStatus_PassesNullToRepository()
    {
        _repoMock.Setup(r => r.FilterAsync(null, null, null, null))
            .ReturnsAsync(new List<ExpenseRequest>());

        var filter = new FilterExpenseRequestDto("InvalidStatus", null, null, null);
        var result = await _useCase.ExecuteAsync(filter);

        result.Should().BeEmpty();
        _repoMock.Verify(r => r.FilterAsync(null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_CategoryFilter_PassesCategoryToRepository()
    {
        _repoMock.Setup(r => r.FilterAsync(null, "Travel", null, null))
            .ReturnsAsync(new List<ExpenseRequest>());

        var filter = new FilterExpenseRequestDto(null, "Travel", null, null);
        var result = await _useCase.ExecuteAsync(filter);

        _repoMock.Verify(r => r.FilterAsync(null, "Travel", null, null), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DateRangeFilter_PassesDatesToRepository()
    {
        var from = new DateTime(2025, 1, 1);
        var to = new DateTime(2025, 12, 31);

        _repoMock.Setup(r => r.FilterAsync(null, null, from, to))
            .ReturnsAsync(new List<ExpenseRequest>());

        var filter = new FilterExpenseRequestDto(null, null, from, to);
        var result = await _useCase.ExecuteAsync(filter);

        _repoMock.Verify(r => r.FilterAsync(null, null, from, to), Times.Once);
    }
}

public class ExpenseRequestApproveUseCaseAdditionalTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestApproveUseCase _useCase;

    public ExpenseRequestApproveUseCaseAdditionalTests()
    {
        var loggerMock = new Mock<ILogger<ExpenseRequestApproveUseCase>>();
        _useCase = new ExpenseRequestApproveUseCase(_repoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseRequest?)null);

        var act = () => _useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_RejectedRequest_ThrowsInvalidOperation()
    {
        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "X" },
            Description = "D", Amount = 100, ExpenseDate = DateTime.UtcNow,
            RequestedById = Guid.NewGuid(),
            RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "U" },
            Status = ExpenseStatus.Rejected
        };

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var act = () => _useCase.ExecuteAsync(entity.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class ExpenseRequestRejectUseCaseAdditionalTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestRejectUseCase _useCase;

    public ExpenseRequestRejectUseCaseAdditionalTests()
    {
        var loggerMock = new Mock<ILogger<ExpenseRequestRejectUseCase>>();
        _useCase = new ExpenseRequestRejectUseCase(_repoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseRequest?)null);

        var act = () => _useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_ApprovedRequest_ThrowsInvalidOperation()
    {
        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "X" },
            Description = "D", Amount = 100, ExpenseDate = DateTime.UtcNow,
            RequestedById = Guid.NewGuid(),
            RequestedBy = new AppUser { Id = Guid.NewGuid(), FullName = "U" },
            Status = ExpenseStatus.Approved
        };

        _repoMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var act = () => _useCase.ExecuteAsync(entity.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class ExpenseRequestGetMetricsUseCaseAdditionalTests
{
    private readonly Mock<IExpenseRequestRepository> _repoMock = new();
    private readonly ExpenseRequestGetMetricsUseCase _useCase;

    public ExpenseRequestGetMetricsUseCaseAdditionalTests()
    {
        _useCase = new ExpenseRequestGetMetricsUseCase(_repoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyList_ReturnsZeroMetrics()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ExpenseRequest>());

        var result = await _useCase.ExecuteAsync();

        result.TotalRequests.Should().Be(0);
        result.ApprovedCount.Should().Be(0);
        result.RejectedCount.Should().Be(0);
        result.PendingCount.Should().Be(0);
        result.TotalApprovedAmount.Should().Be(0m);
    }

    [Fact]
    public async Task ExecuteAsync_AllPending_ReturnsZeroApprovedAmount()
    {
        var cat = new Category { Id = Guid.NewGuid(), Name = "X" };
        var user = new AppUser { Id = Guid.NewGuid(), FullName = "U" };

        var entities = new List<ExpenseRequest>
        {
            new() { Id = Guid.NewGuid(), CategoryId = cat.Id, Category = cat, Description = "A", Amount = 100, ExpenseDate = DateTime.UtcNow, RequestedById = user.Id, RequestedBy = user, Status = ExpenseStatus.Pending },
            new() { Id = Guid.NewGuid(), CategoryId = cat.Id, Category = cat, Description = "B", Amount = 200, ExpenseDate = DateTime.UtcNow, RequestedById = user.Id, RequestedBy = user, Status = ExpenseStatus.Pending }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _useCase.ExecuteAsync();

        result.TotalRequests.Should().Be(2);
        result.PendingCount.Should().Be(2);
        result.ApprovedCount.Should().Be(0);
        result.TotalApprovedAmount.Should().Be(0m);
    }
}
