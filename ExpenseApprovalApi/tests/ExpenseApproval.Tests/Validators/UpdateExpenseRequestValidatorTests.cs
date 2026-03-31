using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Validators;
using FluentAssertions;

namespace ExpenseApproval.Tests.Validators;

public class UpdateExpenseRequestValidatorTests
{
    private readonly UpdateExpenseRequestValidator _validator = new();

    [Fact]
    public async Task Valid_Request_Passes()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "Updated expense", 250m, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_CategoryId_Fails()
    {
        var dto = new UpdateExpenseRequestDto(Guid.Empty, "Updated", 250m, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public async Task Empty_Description_Fails()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "", 250m, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Zero_Amount_Fails()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "Updated", 0m, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public async Task Negative_Amount_Fails()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "Updated", -10m, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public async Task FutureDate_Fails()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "Updated", 250m, DateTime.UtcNow.AddDays(30));
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ExpenseDate");
    }

    [Fact]
    public async Task TodaysDate_Passes()
    {
        var dto = new UpdateExpenseRequestDto(Guid.NewGuid(), "Updated", 250m, DateTime.UtcNow.Date);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
