using Bumbo.Data.Models;

namespace Bumbo.WorkingRules.Repositories;

public class CaoDummyRepository : ICaoRepository
{
    private readonly List<Shift> _shifts = new();
    private readonly List<SpecialAvailability> _specialAvailabilities = new();
    private readonly List<SpecialSchoolHours> _specialSchoolHours = new();
    private readonly List<StandardAvailability> _standardAvailabilities = new();

    private readonly List<StandardSchoolHours> _standardSchoolHours = new();
    private readonly List<ClockedHours> _workedHours = new();
    private Branch _branch = new();
    private Department _department = new();
    private ApplicationUser _employee = new();
    private ApplicationUser _employee2 = new();

    public CaoDummyRepository()
    {
        CreateTestData();
    }

    public async Task<decimal?> GetSpecialSchoolHoursForDay(DateTime time, string employeeId)
    {
        var schoolDay = _specialSchoolHours.FirstOrDefault(e => e.Start.Date == time.Date && e.EmployeeId == employeeId);
        if (schoolDay != null)
        {
            return await Task.FromResult(schoolDay.Hours);
        }

        return null;
    }

    public async Task<decimal?> GetDefaultSchoolHoursForDay(DateTime time, string employeeId)
    {
        var schoolDay = _standardSchoolHours.FirstOrDefault(e => e.DayOfWeek == time.DayOfWeek && e.EmployeeId == employeeId);
        if (schoolDay != null)
        {
            return await Task.FromResult(schoolDay.Hours);
        }

        return null;
    }

    public Task<List<SpecialSchoolHours>> GetSpecialSchoolHoursForPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        return null!;
    }

    public Task<List<StandardSchoolHours>> GetDefaultSchoolHoursForPeriod(int branchId)
    {
        return null!;
    }

    public Task<List<Shift>> GetShiftsForDay(DateTime time, string employeeId)
    {
        return Task.FromResult(_shifts.Where(e => e.EmployeeId == employeeId && e.Start.Date == time.Date).ToList());
    }

    public Task<List<Shift>> GetAllShiftsWithinPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        return Task.FromResult(_shifts.Where(e => e.Employee.Branch.Id == branchId && e.Start.Date == startDate.Date && e.End.Date == endDate.Date).ToList());
    }

    public Task<SpecialAvailability> GetSpecialAvailabilityForDay(DateTime time, string employeeId)
    {
        return Task.FromResult(_specialAvailabilities.FirstOrDefault(e => e.Start.Date == time.Date && e.EmployeeId == employeeId)!);
    }

    public Task<StandardAvailability> GetDefaultAvailabilityForDay(DateTime time, string employeeId)
    {
        return Task.FromResult(_standardAvailabilities.FirstOrDefault(e => e.DayOfWeek == time.DayOfWeek && e.EmployeeId == employeeId)!);
    }

    public Task<List<ClockedHours>> GetAllWorkedHoursWithinPeriod(DateTime startTime, DateTime endTime, Branch branch)
    {
        return Task.FromResult(_workedHours.Where(e => e.Employee!.Branch == branch && e.ClockedIn.Date == startTime && e.ClockedOut == endTime).ToList());
    }

    public async Task<List<StandardAvailability>> GetDefaultAvailabilityForPeriod(int branchId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<SpecialAvailability>> GetSpecialAvailabilityForPeriod(DateTime startDate, DateTime endDate, int branchId)
    {
        throw new NotImplementedException();
    }

    public ApplicationUser GetFirstUser()
    {
        return _employee;
    }

    private void CreateTestData()
    {
        _department = new Department()
        {
            Id = 1,
            Name = "Kassa"
        };

        _employee = new ApplicationUser
        {
            Id = "1",
            UserName = "jeromenl",
            NormalizedUserName = "JEROMENL",
            Email = "jerome@kwetters.nl",
            NormalizedEmail = "JORAM@KWETTERS.NL",
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
            FirstName = "Jerome",
            MiddleName = null,
            LastName = "Kwetters",
            RegistrationDate = DateTime.Now,
            Address = null,
            PayoutScale = 1,
            Departments = null,
            Branch = _branch,
            BirthDate = DateTime.Parse("2008-01-01")
        };

        _employee2 = new ApplicationUser
        {
            Id = "2",
            UserName = "kees",
            NormalizedUserName = "keesnl",
            Email = "kees@kwetters.nl",
            NormalizedEmail = "KEES@KWETTERS.NL",
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
            FirstName = "Kees",
            MiddleName = null,
            LastName = "Kwetters",
            RegistrationDate = DateTime.Now,
            Address = null,
            PayoutScale = 1,
            Departments = null,
            Branch = _branch,
            BirthDate = DateTime.Parse("1998-04-20")
        };

        StandardSchoolHours tuesdayDefault = new()
        {
            DayOfWeek = DayOfWeek.Tuesday,
            Employee = _employee,
            Hours = 6
        };

        _standardSchoolHours.Add(tuesdayDefault);

        _specialSchoolHours.AddRange(new List<SpecialSchoolHours>()
        {
            new SpecialSchoolHours()
            {
                Id = 1,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/24"),
                Hours = 8
            },

            new SpecialSchoolHours()
            {
                Id = 3,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/26"),
                Hours = 3
            },
            new SpecialSchoolHours()
            {
                Id = 4,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/27"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 5,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/28"),
                Hours = 5
            },

            new SpecialSchoolHours()
            {
                Id = 6,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/29"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 7,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/30"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 8,
                Employee = _employee,
                Start = DateTime.Parse("2022/10/31"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 10,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/02"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 11,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/03"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 12,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/04"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 13,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/05"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 14,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/06"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 15,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/07"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 17,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/09"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 18,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/10"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 19,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/11"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 20,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/12"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 21,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/13"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 22,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/14"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 24,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/16"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 25,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/17"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 26,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/18"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 27,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/19"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 28,
                Employee = _employee,
                Start = DateTime.Parse("2022/11/20"),
                Hours = 0
            }
        });


        _specialSchoolHours.AddRange(new List<SpecialSchoolHours>()
        {
            new SpecialSchoolHours()
            {
                Id = 100,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/24"),
                Hours = 8
            },
            new SpecialSchoolHours()
            {
                Id = 101,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/25"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 102,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/26"),
                Hours = 3
            },
            new SpecialSchoolHours()
            {
                Id = 103,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/27"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 104,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/28"),
                Hours = 5
            },

            new SpecialSchoolHours()
            {
                Id = 105,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/29"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 106,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/30"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 107,
                Employee = _employee2,
                Start = DateTime.Parse("2022/10/31"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 108,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/01"),
                Hours = 3
            },
            new SpecialSchoolHours()
            {
                Id = 109,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/02"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 110,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/03"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 111,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/04"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 112,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/05"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 113,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/06"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 114,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/07"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 115,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/08"),
                Hours = 3
            },
            new SpecialSchoolHours()
            {
                Id = 116,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/09"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 117,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/10"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 118,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/11"),
                Hours = 3.5m
            },

            new SpecialSchoolHours()
            {
                Id = 119,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/12"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 120,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/13"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 121,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/14"),
                Hours = 5
            },
            new SpecialSchoolHours()
            {
                Id = 122,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/15"),
                Hours = 3
            },
            new SpecialSchoolHours()
            {
                Id = 123,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/16"),
                Hours = 0
            },
            new SpecialSchoolHours()
            {
                Id = 124,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/17"),
                Hours = 7
            },
            new SpecialSchoolHours()
            {
                Id = 125,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/18"),
                Hours = 3.5m
            },
            new SpecialSchoolHours()
            {
                Id = 126,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/19"),
                Hours = 0
            },

            new SpecialSchoolHours()
            {
                Id = 127,
                Employee = _employee2,
                Start = DateTime.Parse("2022/11/20"),
                Hours = 0
            }
        });

        _shifts.AddRange(new List<Shift>()
        {
            new Shift()
            {
                Id = 51,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("10/23/2022 14:00:00"),
                End = DateTime.Parse("10/23/2022 18:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 52,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("10/27/2022 16:00:00"),
                End = DateTime.Parse("10/27/2022 21:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 53,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("10/29/2022 12:00:00"),
                End = DateTime.Parse("10/29/2022 20:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 54,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/03/2022 10:00:00"),
                End = DateTime.Parse("11/03/2022 18:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 55,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/04/2022 14:00:00"),
                End = DateTime.Parse("11/04/2022 22:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 56,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/09/2022 08:00:00"),
                End = DateTime.Parse("11/09/2022 16:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 57,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/14/2022 14:00:00"),
                End = DateTime.Parse("11/14/2022 16:00:00"),
                IsIll = true,
                IsPublished = true
            },
            new Shift()
            {
                Id = 58,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/16/2022 18:00:00"),
                End = DateTime.Parse("11/16/2022 23:00:00"),
                IsIll = true,
                IsPublished = true
            },
            new Shift()
            {
                Id = 59,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/18/2022 15:00:00"),
                End = DateTime.Parse("11/18/2022 22:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 61,
                Employee = _employee2,
                Department = _department,
                Start = DateTime.Parse("11/19/2022 15:00:00"),
                End = DateTime.Parse("11/19/2022 17:00:00"),
                IsIll = false,
                IsPublished = true
            }
        });

        // SHIFTS EMPLOYEE 1
        _shifts.AddRange(new List<Shift>()
        {
            new Shift()
            {
                Id = 1,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("10/23/2022 14:00:00"),
                End = DateTime.Parse("10/23/2022 18:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 2,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("10/27/2022 16:00:00"),
                End = DateTime.Parse("10/27/2022 21:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 3,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("10/29/2022 12:00:00"),
                End = DateTime.Parse("10/29/2022 20:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 4,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/03/2022 10:00:00"),
                End = DateTime.Parse("11/03/2022 18:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 5,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/04/2022 14:00:00"),
                End = DateTime.Parse("11/04/2022 22:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 6,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/09/2022 08:00:00"),
                End = DateTime.Parse("11/09/2022 16:00:00"),
                IsIll = false,
                IsPublished = true
            },
            new Shift()
            {
                Id = 7,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/14/2022 14:00:00"),
                End = DateTime.Parse("11/14/2022 16:00:00"),
                IsIll = true,
                IsPublished = true
            },
            new Shift()
            {
                Id = 8,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/16/2022 18:00:00"),
                End = DateTime.Parse("11/16/2022 23:00:00"),
                IsIll = true,
                IsPublished = true
            },
            new Shift()
            {
                Id = 9,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/18/2022 15:00:00"),
                End = DateTime.Parse("11/18/2022 22:00:00"),
                IsIll = false,
                IsPublished = true
            },

            new Shift()

            {
                Id = 60,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/20/2022 15:00:00"),
                End = DateTime.Parse("11/20/2022 22:00:00"),
                IsIll = false,
                IsPublished = true
            },

            new Shift()
            {
                Id = 11,
                Employee = _employee,
                Department = _department,
                Start = DateTime.Parse("11/19/2022 15:00:00"),
                End = DateTime.Parse("11/19/2022 17:00:00"),
                IsIll = false,
                IsPublished = true
            }
        });

        _standardAvailabilities.AddRange(new List<StandardAvailability>()
        {
            new StandardAvailability()
            {
                DayOfWeek = DayOfWeek.Tuesday,
                Employee = _employee,
                Start = DateTime.Parse("13:00"),
                End = DateTime.Parse("23:00")
            }
        });

        _specialAvailabilities.AddRange(new List<SpecialAvailability>()
        {
            new SpecialAvailability()
            {
                Id = 2,
                Employee = _employee,
                Start = DateTime.Parse("11/15/2022 00:00:00"),
                End = DateTime.Parse("11/15/2022 00:00:00")
            },
            new SpecialAvailability()
            {
                Id = 3,
                Employee = _employee,
                Start = DateTime.Parse("11/16/2022 16:15:00"),
                End = DateTime.Parse("11/16/2022 23:59:59")
            },
            new SpecialAvailability()
            {
                Id = 4,
                Employee = _employee,
                Start = DateTime.Parse("11/17/2022 12:00:00"),
                End = DateTime.Parse("11/17/2022 18:00:00")
            },
            new SpecialAvailability()
            {
                Id = 5,
                Employee = _employee,
                Start = DateTime.Parse("11/18/2022 13:00:00"),
                End = DateTime.Parse("11/18/2022 22:30:00")
            },
            new SpecialAvailability()
            {
                Id = 6,
                Employee = _employee,
                Start = DateTime.Parse("11/19/2022 06:00:00"),
                End = DateTime.Parse("11/19/2022 17:30:00")
            },
            new SpecialAvailability()
            {
                Id = 7,
                Employee = _employee,
                Start = DateTime.Parse("11/20/2022 13:00:00"),
                End = DateTime.Parse("11/20/2022 23:59:59")
            }
        });

        _workedHours.AddRange(new List<ClockedHours>()
        {
            new ClockedHours()
            {
                Id = 1,
                ClockedIn = DateTime.Parse("11/14/2022 14:10:00"),
                ClockedOut = DateTime.Parse("11/14/2022 16:05:00"),
                Employee = _employee
            },
            new ClockedHours()
            {
                Id = 2,
                ClockedIn = DateTime.Parse("11/16/2022 17:55:00"),
                ClockedOut = DateTime.Parse("11/16/2022 22:30:00"),
                Employee = _employee
            },
            new ClockedHours()
            {
                Id = 2,
                ClockedIn = DateTime.Parse("11/16/2022 14:55:00"),
                ClockedOut = DateTime.Parse("11/16/2022 21:50:00"),
                Employee = _employee
            },
            new ClockedHours()
            {
                Id = 2,
                ClockedIn = DateTime.Parse("11/16/2022 17:55:00"),
                ClockedOut = DateTime.Parse("11/16/2022 22:30:00"),
                Employee = _employee
            },
            new ClockedHours()
            {
                Id = 2,
                ClockedIn = DateTime.Parse("11/19/2022 17:00:00"),
                ClockedOut = DateTime.Parse("11/19/2022 20:30:00"),
                Employee = _employee
            }
        });

        _branch = new Branch
        {
            Id = 1,
            Name = "Bumbo Hellouw",
            ShelfLength = 0,
            Address = null,
            OpeningHours = null,
            WorkStandards = null,
            Employees = { _employee, _employee2 },
            HistoricalData = null,
            Prognoses = null
        };
    }
}