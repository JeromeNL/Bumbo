using System.Globalization;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers
{
    [Authorize(Roles = nameof(Role.Employee))]
    public class EmployeeAvailabilityController : Controller
    {
        private readonly IEmployeeAvailabilityRepository _employeeAvailabilityRepository;
        private readonly IUserService _userService;

        public EmployeeAvailabilityController(IUserService userService, IEmployeeAvailabilityRepository employeeAvailabilityRepository)
        {
            _userService = userService;
            _employeeAvailabilityRepository = employeeAvailabilityRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employeeAvailabilityViewModel = new EmployeeAvailabilityViewModel();
            var specialAvailabilityViewModel = new SpecialAvailabilityViewModel();

            var standardAvailabilitiesList = await _employeeAvailabilityRepository.GetStandardAvailabilities(_userService.GetUserId());
            employeeAvailabilityViewModel.Availabilities = CreateAvailabilitiesForView(standardAvailabilitiesList);
            employeeAvailabilityViewModel.SchoolHoursForDayOfWeek = await CreateSchoolHoursPerDayOfWeek(employeeAvailabilityViewModel);
            employeeAvailabilityViewModel.UserLoggedIn = await _userService.GetUser();

            employeeAvailabilityViewModel.SpecialAvailabilityViewModel = specialAvailabilityViewModel;
            employeeAvailabilityViewModel.SpecialAvailabilityViewModel.StandardAvailability = await _employeeAvailabilityRepository.GetStandardAvailabilityForDayOfWeek(_userService.GetUserId(), DateTime.Now.DayOfWeek);
            specialAvailabilityViewModel.SpecialAvailabilities = await _employeeAvailabilityRepository.GetSpecialAvailabilities(_userService.GetUserId());
            specialAvailabilityViewModel.SpecialSchoolHoursList = await _employeeAvailabilityRepository.GetSpecialSchoolHours(_userService.GetUserId());
            specialAvailabilityViewModel.UserLoggedIn = await _userService.GetUser();
            return View(employeeAvailabilityViewModel);
        }

        //StandardAvailability Methods
        private List<StandardAvailabilityViewModel> CreateAvailabilitiesForView(IEnumerable<StandardAvailability> standardAvailabilitiesList)
        {
            var localStandardAvailabilites = new List<StandardAvailability>();
            var day = DayOfWeek.Monday;
            for (var days = 0; days < 7; days++)
            {
                if (!Enum.IsDefined(typeof(DayOfWeek), day))
                {
                    day = DayOfWeek.Sunday;
                }

                localStandardAvailabilites.Add(new StandardAvailability
                {
                    DayOfWeek = day,
                    Start = DateTime.MinValue,
                    End = DateTime.MinValue,
                    EmployeeId = _userService.GetUserId()
                });

                day++;
            }

            foreach (var standardAvailability in standardAvailabilitiesList)
            {
                foreach (var localStandardAvailabilite in localStandardAvailabilites)
                {
                    if (localStandardAvailabilite.DayOfWeek == standardAvailability.DayOfWeek)
                    {
                        localStandardAvailabilite.Start = standardAvailability.Start;
                        localStandardAvailabilite.End = standardAvailability.End;
                    }
                }
            }

            var standardAvailabilityViewModels = new List<StandardAvailabilityViewModel>();
            foreach (var standardAvailability in localStandardAvailabilites)
            {
                standardAvailabilityViewModels.Add(new StandardAvailabilityViewModel()
                {
                    DayOfWeek = standardAvailability.DayOfWeek,
                    StartTime = standardAvailability.Start.TimeOfDay.ToString(),
                    EndTime = standardAvailability.End.TimeOfDay.ToString()
                });
            }

            return standardAvailabilityViewModels;
        }

        private async Task<Dictionary<DayOfWeek, double>> CreateSchoolHoursPerDayOfWeek(EmployeeAvailabilityViewModel employeeAvailabilityViewModel)
        {
            var schoolHoursPerDayOfWeek = new Dictionary<DayOfWeek, double>();
            foreach (var availability in employeeAvailabilityViewModel.Availabilities)
            {
                schoolHoursPerDayOfWeek.Add(availability.DayOfWeek, await _employeeAvailabilityRepository.GetStandardSchoolHoursForDayOfWeek(_userService.GetUserId(), availability.DayOfWeek));
            }

            return schoolHoursPerDayOfWeek;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStandardAvailability(EmployeeAvailabilityViewModel employeeAvailabilityViewModel)
        {
            var errorList = ValidateStandardAvailabilityViewModel(employeeAvailabilityViewModel);
            if (errorList.Count > 0)
            {
                var user = await _userService.GetUser();
                employeeAvailabilityViewModel.Errors = errorList;
                employeeAvailabilityViewModel.SpecialAvailabilityViewModel = new SpecialAvailabilityViewModel
                {
                    SpecialAvailabilities = await _employeeAvailabilityRepository.GetSpecialAvailabilities(_userService.GetUserId()),
                    UserLoggedIn = user
                };
                employeeAvailabilityViewModel.UserLoggedIn = user;
                return View("Index", employeeAvailabilityViewModel);
            }

            await _employeeAvailabilityRepository.UpdateStandardAvailability(ConvertStandardAvailabilityViewModelToStandardAvailabilityObjects(employeeAvailabilityViewModel.Availabilities));
            var zeroTime = new DateTime(1, 1, 1);
            var timeSpan = DateTime.Now - _userService.GetUser().Result.BirthDate;
            var years = (zeroTime + timeSpan).Year - 1;
            if (years < 18)
            {
                await _employeeAvailabilityRepository.SetStandardSchoolHours(_userService.GetUserId(), employeeAvailabilityViewModel.SchoolHoursForDayOfWeek);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Converts the string based start and end time to a DateTime object.
        /// </summary>
        /// <param name="standardAvailabilityViewModels">Viewmodel to be converted</param>
        /// <returns>A list with standardAvailabilities</returns>
        private List<StandardAvailability> ConvertStandardAvailabilityViewModelToStandardAvailabilityObjects(IEnumerable<StandardAvailabilityViewModel> standardAvailabilityViewModels)
        {
            var standardAvailabilities = new List<StandardAvailability>();
            foreach (var standardAvailabilityViewModel in standardAvailabilityViewModels)
            {
                standardAvailabilities.Add(new StandardAvailability()
                {
                    DayOfWeek = standardAvailabilityViewModel.DayOfWeek,
                    Start = DateTime.Parse(standardAvailabilityViewModel.StartTime),
                    End = DateTime.Parse(standardAvailabilityViewModel.EndTime),
                    EmployeeId = _userService.GetUserId()
                });
            }

            return standardAvailabilities;
        }

        private List<string> ValidateStandardAvailabilityViewModel(EmployeeAvailabilityViewModel employeeAvailabilityViewModel)
        {
            var errors = new List<string>();

            foreach (var standardAvailability in employeeAvailabilityViewModel.Availabilities)
            {
                var startTimeOnly = TimeOnly.Parse(standardAvailability.StartTime);
                var endTimeOnly = TimeOnly.Parse(standardAvailability.EndTime);
                if (startTimeOnly > endTimeOnly)
                {
                    errors.Add("Start tijd kan niet na de eind tijd zijn op " + CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(standardAvailability.DayOfWeek));
                }
            }

            return errors;
        }

        public async Task<IActionResult> RemoveStandardAvailability(DayOfWeek dayOfWeek)
        {
            await _employeeAvailabilityRepository.RemoveStandardAvailability(dayOfWeek, _userService.GetUserId());
            return RedirectToAction("Index");
        }

        //SpecialAvailability Methods
        [HttpPost]
        public async Task<IActionResult> UpdateSpecialAvailability(SpecialAvailabilityViewModel specialAvailabilityViewModel)
        {
            if (specialAvailabilityViewModel.StartTime.CompareTo(specialAvailabilityViewModel.EndTime) > -1)
            {
                return RedirectToAction("Index");
            }

            if (ConvertTimeForSpecialAvailability(specialAvailabilityViewModel) == false)
            {
                return RedirectToAction("Index");
            }

            await _employeeAvailabilityRepository.SetSpecialAvailability(_userService.GetUserId(), specialAvailabilityViewModel.SpecialAvailability);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSpecialSchoolHours(SpecialAvailabilityViewModel specialAvailabilityViewModel)
        {
            var specialSchoolHour = new SpecialSchoolHours()
            {
                EmployeeId = _userService.GetUserId(),
                Start = specialAvailabilityViewModel.StartTime,
                Hours = (decimal)specialAvailabilityViewModel.SchoolHours
            };

            await _employeeAvailabilityRepository.SetSpecialSchoolHours(_userService.GetUserId(), specialSchoolHour);

            return RedirectToAction("Index");
        }

        private bool ConvertTimeForSpecialAvailability(SpecialAvailabilityViewModel specialAvailabilityViewModel)
        {
            DateTime startTimeForDate;
            DateTime endTimeForDate;

            var date = specialAvailabilityViewModel.Date;
            if (specialAvailabilityViewModel.IsWholeDayAvailable)
            {
                startTimeForDate = date.Date + new TimeSpan(0, 0, 0);
                endTimeForDate = date.Date + new TimeSpan(23, 59, 59);
            }
            else
            {
                startTimeForDate = date.Date + specialAvailabilityViewModel.StartTime.TimeOfDay;
                endTimeForDate = date.Date + specialAvailabilityViewModel.EndTime.TimeOfDay;
            }

            if (startTimeForDate > endTimeForDate)
            {
                return false;
            }

            var specialAvailability = new SpecialAvailability()
            {
                Start = startTimeForDate,
                End = endTimeForDate,
                EmployeeId = _userService.GetUserId(),
                IsAvailableForDate = specialAvailabilityViewModel.IsWholeDayAvailable
            };
            specialAvailabilityViewModel.SpecialAvailability = specialAvailability;
            return true;
        }

        public async Task<IActionResult> DeleteSpecialAvailability(int? id)
        {
            await _employeeAvailabilityRepository.RemoveSpecialAvailability(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSpecialSchoolHour(int? id)
        {
            await _employeeAvailabilityRepository.RemoveSpecialSchoolHour(id);
            return RedirectToAction("Index");
        }
    }
}