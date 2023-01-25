using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Controllers;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bumbo.Tests;

public class WorkStandardsTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IWorkStandardsRepository> _workStandardRepositoryMock;

    public WorkStandardsTests()
    {
        _workStandardRepositoryMock = new Mock<IWorkStandardsRepository>();
        _userServiceMock = new Mock<IUserService>();
    }

    [Fact]
    public async Task Index_ShouldReturnCorrectViewResult()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<WorkStandardsViewModel>(
            viewResult.ViewData.Model);
    }

    [Fact]
    public async Task CreateGet_ShouldReturnCorrectViewResult()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);

        // Act
        var result = await controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<WorkStandardsViewModel>(
            viewResult.ViewData.Model);
    }

    [Fact]
    public async Task CreatePost_ShouldRedirectToAction()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);
        _userServiceMock.Setup(user => user.GetUserAdvanced().Result).Returns(GetUser());
        var workStandardsViewModel = FillWorkStandardsViewModel();

        // Act
        var result = await controller.Create(workStandardsViewModel);

        // Assert
        var viewResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", viewResult.ActionName);
    }

    [Fact]
    public async Task CreatePost_ShouldRequestUserData()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);
        _userServiceMock.Setup(user => user.GetUserAdvanced().Result).Returns(GetUser());
        var workStandardsViewModel = FillWorkStandardsViewModel();

        // Act
        await controller.Create(workStandardsViewModel);

        // Assert
        _userServiceMock.Verify(user => user.GetUserAdvanced(), Times.Once());
    }

    [Fact]
    public void RetrieveWorkStandardData_ShouldCallAllDataMethods()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);
        _userServiceMock.Setup(user => user.GetUserAdvanced().Result).Returns(GetUser());
        var workStandardsViewModel = FillWorkStandardsViewModel();

        // Act
        controller.RetrieveWorkStandardData(workStandardsViewModel);

        // Assert
        _workStandardRepositoryMock.Verify(repo => repo.GetKassaWorkStandard(), Times.Once());
        _workStandardRepositoryMock.Verify(repo => repo.GetSpiegelenWorkStandard(), Times.Once());
        _workStandardRepositoryMock.Verify(repo => repo.GetUitladenWorkStandard(), Times.Once());
        _workStandardRepositoryMock.Verify(repo => repo.GetVersWorkStandard(), Times.Once());
        _workStandardRepositoryMock.Verify(repo => repo.GetKassaWorkStandard(), Times.Once());
        _workStandardRepositoryMock.Verify(repo => repo.GetPastWorkStandardsDictionary(workStandardsViewModel.Kassa), Times.Once());
    }

    [Fact]
    public void History_ShouldReturnCorrectViewResult()
    {
        // Arrange
        var controller = new WorkStandardsController(_userServiceMock.Object, _workStandardRepositoryMock.Object);

        // Act
        var result = controller.History();

        // Assert
        var viewResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", viewResult.ActionName);
        Assert.Equal("History", viewResult.ControllerName);
    }

    private static WorkStandardsViewModel FillWorkStandardsViewModel()
    {
        var workStandardsViewModel = new WorkStandardsViewModel
        {
            Kassa = new WorkStandards
            {
                Id = 1,
                Task = WorkStandardTypes.Kassa,
                RequiredTimeInMinutes = 5,
                DateEntered = DateTime.Now,
                Branch = null
            },
            Spiegelen = new WorkStandards
            {
                Id = 2,
                Task = WorkStandardTypes.Spiegelen,
                RequiredTimeInMinutes = 5,
                DateEntered = DateTime.Now,
                Branch = null
            },
            Uitladen = new WorkStandards
            {
                Id = 3,
                Task = WorkStandardTypes.Uitladen,
                RequiredTimeInMinutes = 5,
                DateEntered = DateTime.Now,
                Branch = null
            },
            Vers = new WorkStandards
            {
                Id = 4,
                Task = WorkStandardTypes.Vers,
                RequiredTimeInMinutes = 5,
                DateEntered = DateTime.Now,
                Branch = null
            },
            VakkenVullen = new WorkStandards
            {
                Id = 5,
                Task = WorkStandardTypes.VakkenVullen,
                RequiredTimeInMinutes = 5,
                DateEntered = DateTime.Now,
                Branch = null
            }
        };

        return workStandardsViewModel;
    }

    private static ApplicationUser GetUser()
    {
        var branch = new Branch
        {
            Id = 1,
            Name = "Bumbo",
            ShelfLength = 0,
            Address = null,
            OpeningHours = null,
            WorkStandards = null,
            Employees = null,
            HistoricalData = null,
            Prognoses = null
        };
        return new ApplicationUser
        {
            Id = "1",
            UserName = "LamineSlot",
            NormalizedUserName = "LAMINESLOT",
            Email = "slotlamine@gmail.com",
            NormalizedEmail = "SLOTLAMINE@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = "ABCDEF",
            SecurityStamp = "null",
            ConcurrencyStamp = "null",
            PhoneNumber = "null",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            FirstName = "Lamine",
            MiddleName = null,
            LastName = "Slot",
            RegistrationDate = DateTime.Now,
            Address = null,
            PayoutScale = 1,
            Departments = null,
            Branch = branch
        };
    }
}