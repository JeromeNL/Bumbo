using Bumbo.Data.DAL.Interfaces;
using Bumbo.Prognosis;
using Bumbo.Web.Controllers;
using Bumbo.Web.Models;
using Bumbo.Web.Services.Interfaces;
using Bumbo.WorkingRules.CAORules;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bumbo.Tests;

public class ScheduleTests
{
    private readonly Mock<IPrognosisService> _prognosisServiceMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly Mock<ITimelineService> _timelineService;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IWorkingRules> _workingRules;

    public ScheduleTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _prognosisServiceMock = new Mock<IPrognosisService>();
        _workingRules = new Mock<IWorkingRules>();
        _timelineService = new Mock<ITimelineService>();
        // _scheduleRepositoryMock.Setup()
    }

    [Fact]
    public async Task CopyScheduleViewModelGet_ShouldReturnCorrectViewResult()
    {
        // Arrange
        var controller = new ScheduleController(_scheduleRepositoryMock.Object, _prognosisServiceMock.Object, _userServiceMock.Object, _workingRules.Object, _timelineService.Object);
        var startDate = new DateTime(2021, 1, 1);
        var endDate = new DateTime(2021, 1, 31);

        // Act
        var result = controller.CopySchedule(startDate, endDate);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<CopyScheduleViewModel>(viewResult.ViewData.Model);
    }
}