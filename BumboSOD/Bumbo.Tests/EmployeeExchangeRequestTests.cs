using Bumbo.Data;
using Bumbo.Data.DAL;
using Bumbo.Data.Models;
using Moq;
using Xunit;

namespace Bumbo.Tests;

public class EmployeeExchangeRequestTests
{
    readonly Mock<BumboDbContext> _context;
    readonly EmployeeExchangeRequestRepository _employeeExchangeRequestRepository;
    readonly Mock<ApplicationUser> _mockApplicationUserOriginal;
    readonly Mock<ApplicationUser> _mockApplicationUserReplacement;

    public EmployeeExchangeRequestTests()
    {
        _mockApplicationUserOriginal = new Mock<ApplicationUser>();
        _mockApplicationUserReplacement = new Mock<ApplicationUser>();
        _context = new Mock<BumboDbContext>();
        _employeeExchangeRequestRepository = new EmployeeExchangeRequestRepository(_context.Object);
    }

    [Theory]
    [InlineData("11-11-2022 12:00:00", "11-11-2022 13:00:00")]
    [InlineData("11-11-2022 12:00:00", "11-11-2022 14:00:00")]
    [InlineData("11-11-2022 18:00:00", "11-11-2022 21:00:00")]
    [InlineData("11-11-2022 17:00:00", "11-11-2022 21:00:00")]
    [InlineData("10-10-2022 12:00:00", "10-10-2022 18:00:00")]
    public void EmployeeExchangeRequest_ShiftFromUserLoggedInNotOverlapsExchangeRequestShift(string exchangeRequestStartDate, string exchangeRequestEndDate)
    {
        // Arrange
        var expected = false;

        var shiftFromOriginalUser = new Shift { Id = 1, Start = DateTime.Parse(exchangeRequestStartDate), End = DateTime.Parse(exchangeRequestEndDate), Employee = _mockApplicationUserOriginal.Object };

        var shiftFromNewUser = new List<Shift> { new() { Id = 2, Start = DateTime.Parse("11-11-2022 14:00:00"), End = DateTime.Parse("11-11-2022 17:00:00"), Employee = _mockApplicationUserReplacement.Object } };

        var exchangeRequest = new ExchangeRequest { Id = 1, ShiftId = 1, Shift = shiftFromOriginalUser, OriginalUser = _mockApplicationUserOriginal.Object };

        // Act
        var actual = _employeeExchangeRequestRepository.CheckUserLoggedInHasShiftAlreadyAsync(exchangeRequest, shiftFromNewUser);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("11-11-2022 12:00:00", "11-11-2022 19:00:00")]
    [InlineData("11-11-2022 14:00:00", "11-11-2022 17:00:00")]
    [InlineData("11-11-2022 15:00:00", "11-11-2022 16:00:00")]
    [InlineData("11-11-2022 12:00:00", "11-11-2022 15:00:00")]
    [InlineData("11-11-2022 15:00:00", "11-11-2022 20:00:00")]
    public void EmployeeExchangeRequest_ShiftFromUserLoggedInOverlapsExchangeRequestShift(string exchangeRequestStartDate, string exchangeRequestEndDate)
    {
        // Arrange
        var expected = true;

        var shiftFromOriginalUser = new Shift { Id = 1, Start = DateTime.Parse(exchangeRequestStartDate), End = DateTime.Parse(exchangeRequestEndDate), Employee = _mockApplicationUserOriginal.Object };

        var shiftFromNewUser = new List<Shift> { new() { Id = 2, Start = DateTime.Parse("11-11-2022 14:00:00"), End = DateTime.Parse("11-11-2022 17:00:00"), Employee = _mockApplicationUserReplacement.Object } };

        var exchangeRequest = new ExchangeRequest { Id = 1, ShiftId = 1, Shift = shiftFromOriginalUser, OriginalUser = _mockApplicationUserOriginal.Object };

        // Act
        var actual = _employeeExchangeRequestRepository.CheckUserLoggedInHasShiftAlreadyAsync(exchangeRequest, shiftFromNewUser);

        // Assert
        Assert.Equal(expected, actual);
    }
}