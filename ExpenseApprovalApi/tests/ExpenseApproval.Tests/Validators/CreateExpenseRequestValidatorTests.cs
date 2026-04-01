using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Validators;
using FluentAssertions;

namespace ExpenseApproval.Tests.Validators;

public class CreateExpenseRequestValidatorTests
{
    private readonly CreateExpenseRequestValidator _validator = new();

    [Fact]
    public async Task Valid_Request_Passes()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "Vuelo", 500m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_CategoryId_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.Empty, "Vuelo", 500m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Empty_Alimentacionription_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "", 100m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Zero_Amount_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "Alimentacion", 0m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Negative_Amount_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "Alimentacion", -5m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task FutureDate_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "Vuelo", 500m, DateTime.UtcNow.AddDays(30), Guid.NewGuid());
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Empty_RequestedById_Fails()
    {
        var dto = new CreateExpenseRequestDto(Guid.NewGuid(), "Vuelo", 500m, DateTime.UtcNow.AddDays(-1), Guid.Empty);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
