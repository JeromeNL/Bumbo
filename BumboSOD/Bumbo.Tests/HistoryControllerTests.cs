using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Web.Controllers;
using Moq;
using Xunit;

namespace Bumbo.Tests;

public class HistoryControllerTests
{
    private readonly HistoryController _controller;

    public HistoryControllerTests()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(m => m.GetUserAdvanced().Result).Returns(new ApplicationUser { Branch = new Branch() });
        var historyRepository = new Mock<IHistoryRepository>();
        var workStandardsRepository = new Mock<IWorkStandardsRepository>();
        _controller = new HistoryController(userService.Object, historyRepository.Object, workStandardsRepository.Object);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public async Task GetLocalHistoricalData_AmountOfItemsShouldBeEqualToAmountOfDays(int amountOfDays)
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(1);
        var endDate = DateTime.Now.AddDays(amountOfDays);

        // Act
        var result = await _controller.GetLocalHistoricalData(startDate, endDate);

        // Assert
        Assert.Equal(amountOfDays, result.Count);
    }

    [Fact]
    public async Task GetLocalHistoricalData_NegativeAmountBetweenStartAndEndDateShouldReturnZero()
    {
        // Arrange
        var expected = 0;
        var startDate = DateTime.Now.AddDays(1);
        var endDate = DateTime.Now.AddDays(-5);

        // Act
        var result = await _controller.GetLocalHistoricalData(startDate, endDate);

        // Assert
        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public async Task GetLocalHistoricalData_ItemsHaveCorrectDatesAndHaveZeroValues()
    {
        // Arrange
        var expected = 0;
        var startDate = DateTime.Now.AddDays(1);
        var endDate = DateTime.Now.AddDays(7);

        // Act
        var result = await _controller.GetLocalHistoricalData(startDate, endDate);

        // Assert
        Assert.Equal(startDate.ToShortDateString(), result[0].Date.ToShortDateString());
        Assert.Equal(startDate.AddDays(1).ToShortDateString(), result[1].Date.ToShortDateString());
        Assert.Equal(endDate.ToShortDateString(), result[result.Count - 1].Date.ToShortDateString());
        Assert.Equal(expected, result[0].AmountColi);
        Assert.Equal(expected, result[0].AmountCustomers);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    public async Task UpdateListToCorrectDBInfo_HistoricalDataOnAmountOfDaysShouldOnlyBeUpdated(int amountOfDays)
    {
        // Arrange
        var expectedColi = 100;
        var expectedCustomers = 100;
        var startDate = DateTime.Now.Date.AddDays(1);
        var changeDate = startDate.Date.AddDays(amountOfDays);
        var endDate = DateTime.Now.Date.AddDays(amountOfDays + 1 + new Random().Next(7));

        var localData = await _controller.GetLocalHistoricalData(startDate, endDate);
        var testDbData = new List<HistoricalData> { new() { Date = changeDate, AmountColi = expectedColi, AmountCustomers = expectedCustomers } };

        // Act
        _controller.UpdateListToCorrectDbInfo(localData, testDbData);

        // Assert
        // Check if the changed date has changed values
        Assert.Equal(expectedColi, localData.FirstOrDefault(hd => hd.Date == changeDate)!.AmountColi);
        Assert.Equal(expectedCustomers, localData.FirstOrDefault(hd => hd.Date == changeDate)!.AmountCustomers);
        // Check if other values aren't changed
        Assert.Equal(0, localData.FirstOrDefault(hd => hd.Date == startDate)!.AmountColi);
        Assert.Equal(0, localData.FirstOrDefault(hd => hd.Date == startDate)!.AmountCustomers);
    }
}