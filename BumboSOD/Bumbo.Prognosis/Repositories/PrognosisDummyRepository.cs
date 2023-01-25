using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;

namespace Bumbo.Prognosis.Repositories;

public class PrognosisDummyRepository : IPrognosisRepository
{
    private readonly List<Branch> _branches = new();
    private readonly List<Department> _departments = new();
    private readonly List<HistoricalData> _historicalData = new();
    private readonly List<WorkStandards> _workStandards = new();

    public PrognosisDummyRepository()
    {
        CreateTestData();
    }

    public HistoricalData? FindHistoricalDataForThisDate(DateTime currentDate, int branchId)
    {
        return _historicalData.Find(hd => hd.Date == currentDate && hd.BranchId == branchId);
    }

    public IList<Department> GetAllDepartments()
    {
        return _departments;
    }

    public Branch? BranchWithWorkStandard(int branchId)
    {
        return _branches.FirstOrDefault(b => b.Id == branchId);
    }

    public List<HistoricalData> GetHistoricalDataDescending(int branchId)
    {
        return _historicalData.Where(e => e.BranchId == branchId).OrderByDescending(e => e.Date).ToList();
    }

    public IDictionary<Department, Data.Models.Prognosis> GetExistingPrognosis(DateTime day, int branchId)
    {
        return null;
    }

    public Task UpdateOrInsertPrognosis(IList<Data.Models.Prognosis> prognoses)
    {
        return null;
    }

    public Task<IList<Data.Models.Prognosis>> GetAllPrognosesFromIds(IList<int> ints)
    {
        return null;
    }

    public Task ForceIncreasePrognosis(int branchId, DateTime day, IDictionary<int, int> data)
    {
        throw null!;
    }

    /// <summary>
    /// Fills the list properties with the custom dummy data
    /// </summary>
    private void CreateTestData()
    {
        Address address = new()
        {
            Id = 1,
            City = "Amsterdam",
            Zipcode = "1111 AA",
            HouseNumber = 1
        };

        Branch branch = new()
        {
            Id = 1,
            Address = address,
            Name = "Bumbo Amsterdam",
            ShelfLength = 300
        };

        Department kassa = new()
        {
            Id = 1,
            Name = "Kassa"
        };
        Department vers = new()
        {
            Id = 2,
            Name = "Vers"
        };
        Department vakkenvullen = new()
        {
            Id = 3,
            Name = "Vakkenvullen"
        };

        _departments.Add(kassa);
        _departments.Add(vers);
        _departments.Add(vakkenvullen);

        _branches.Add(branch);

        _workStandards.AddRange(new List<WorkStandards>
        {
            new()
            {
                Id = 3,
                Task = WorkStandardTypes.Spiegelen,
                RequiredTimeInMinutes = 0.5m,
                DateEntered = DateTime.Parse("2022-11-20"),
                Branch = branch
            },
            new()
            {
                Id = 4,
                Task = WorkStandardTypes.Uitladen,
                RequiredTimeInMinutes = 5m,
                DateEntered = DateTime.Parse("2022-11-20"),
                Branch = branch
            },
            new()
            {
                Id = 5,
                Task = WorkStandardTypes.Vers,
                RequiredTimeInMinutes = 0.6m,
                DateEntered = DateTime.Parse("2022-11-20"),
                Branch = branch
            },
            new()
            {
                Id = 6,
                Task = WorkStandardTypes.VakkenVullen,
                RequiredTimeInMinutes = 30m,
                DateEntered = DateTime.Parse("2022-11-20"),
                Branch = branch
            },
            new()
            {
                Id = 7,
                Task = WorkStandardTypes.Kassa,
                RequiredTimeInMinutes = 2m,
                DateEntered = DateTime.Parse("2022-11-20"),
                Branch = branch
            }
        });

        _historicalData.AddRange(new List<HistoricalData>
            {
                new()
                {
                    Id = 1, Branch = branch, Date = DateTime.Parse("2021/11/15"), AmountCustomers = 844, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 2, Branch = branch, Date = DateTime.Parse("2021/11/16"), AmountCustomers = 957, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 3, Branch = branch, Date = DateTime.Parse("2021/11/17"), AmountCustomers = 780, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 4, Branch = branch, Date = DateTime.Parse("2021/11/18"), AmountCustomers = 881, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 5, Branch = branch, Date = DateTime.Parse("2021/11/19"), AmountCustomers = 989, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 6, Branch = branch, Date = DateTime.Parse("2021/11/20"), AmountCustomers = 751, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 7, Branch = branch, Date = DateTime.Parse("2021/11/21"), AmountCustomers = 750, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 8, Branch = branch, Date = DateTime.Parse("2021/11/22"), AmountCustomers = 836, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 9, Branch = branch, Date = DateTime.Parse("2021/11/23"), AmountCustomers = 866, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 10, Branch = branch, Date = DateTime.Parse("2021/11/24"), AmountCustomers = 859, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 11, Branch = branch, Date = DateTime.Parse("2021/11/25"), AmountCustomers = 775, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 12, Branch = branch, Date = DateTime.Parse("2021/11/26"), AmountCustomers = 1005, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 13, Branch = branch, Date = DateTime.Parse("2021/11/27"), AmountCustomers = 869, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 14, Branch = branch, Date = DateTime.Parse("2021/11/28"), AmountCustomers = 827, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 15, Branch = branch, Date = DateTime.Parse("2021/11/29"), AmountCustomers = 1046, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 16, Branch = branch, Date = DateTime.Parse("2021/11/30"), AmountCustomers = 825, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 17, Branch = branch, Date = DateTime.Parse("2021/12/01"), AmountCustomers = 976, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 18, Branch = branch, Date = DateTime.Parse("2021/12/02"), AmountCustomers = 969, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 19, Branch = branch, Date = DateTime.Parse("2021/12/03"), AmountCustomers = 1045, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 20, Branch = branch, Date = DateTime.Parse("2021/12/04"), AmountCustomers = 787, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 21, Branch = branch, Date = DateTime.Parse("2021/12/05"), AmountCustomers = 771, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 22, Branch = branch, Date = DateTime.Parse("2021/12/06"), AmountCustomers = 799, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 23, Branch = branch, Date = DateTime.Parse("2021/12/07"), AmountCustomers = 1012, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 24, Branch = branch, Date = DateTime.Parse("2021/12/08"), AmountCustomers = 934, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 25, Branch = branch, Date = DateTime.Parse("2021/12/09"), AmountCustomers = 780, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 26, Branch = branch, Date = DateTime.Parse("2021/12/10"), AmountCustomers = 812, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 27, Branch = branch, Date = DateTime.Parse("2021/12/11"), AmountCustomers = 775, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 28, Branch = branch, Date = DateTime.Parse("2021/12/12"), AmountCustomers = 863, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 29, Branch = branch, Date = DateTime.Parse("2021/12/13"), AmountCustomers = 1020, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 30, Branch = branch, Date = DateTime.Parse("2021/12/14"), AmountCustomers = 801, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 31, Branch = branch, Date = DateTime.Parse("2021/12/15"), AmountCustomers = 885, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 32, Branch = branch, Date = DateTime.Parse("2021/12/16"), AmountCustomers = 820, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 33, Branch = branch, Date = DateTime.Parse("2021/12/17"), AmountCustomers = 948, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 34, Branch = branch, Date = DateTime.Parse("2021/12/18"), AmountCustomers = 1042, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 35, Branch = branch, Date = DateTime.Parse("2021/12/19"), AmountCustomers = 1100, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 36, Branch = branch, Date = DateTime.Parse("2021/12/20"), AmountCustomers = 1300, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 37, Branch = branch, Date = DateTime.Parse("2021/12/21"), AmountCustomers = 1400, AmountColi = 60, BranchId = 1
                },
                new()
                {
                    Id = 38, Branch = branch, Date = DateTime.Parse("2021/12/22"), AmountCustomers = 1700, AmountColi = 70, BranchId = 1
                },
                new()
                {
                    Id = 39, Branch = branch, Date = DateTime.Parse("2021/12/23"), AmountCustomers = 2300, AmountColi = 85, BranchId = 1
                },
                new()
                {
                    Id = 40, Branch = branch, Date = DateTime.Parse("2021/12/24"), AmountCustomers = 2000, AmountColi = 70, BranchId = 1
                },
                new()
                {
                    Id = 41, Branch = branch, Date = DateTime.Parse("2021/12/25"), AmountCustomers = 600, AmountColi = 0, BranchId = 1
                },
                new()
                {
                    Id = 42, Branch = branch, Date = DateTime.Parse("2021/12/26"), AmountCustomers = 700, AmountColi = 60, BranchId = 1
                },
                new()
                {
                    Id = 43, Branch = branch, Date = DateTime.Parse("2021/12/27"), AmountCustomers = 868, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 44, Branch = branch, Date = DateTime.Parse("2021/12/28"), AmountCustomers = 917, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 45, Branch = branch, Date = DateTime.Parse("2021/12/29"), AmountCustomers = 1007, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 46, Branch = branch, Date = DateTime.Parse("2021/12/30"), AmountCustomers = 1400, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 47, Branch = branch, Date = DateTime.Parse("2021/12/31"), AmountCustomers = 1400, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 48, Branch = branch, Date = DateTime.Parse("2022/01/01"), AmountCustomers = 500, AmountColi = 30, BranchId = 1
                },
                new()
                {
                    Id = 49, Branch = branch, Date = DateTime.Parse("2022/01/02"), AmountCustomers = 848, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 50, Branch = branch, Date = DateTime.Parse("2022/01/03"), AmountCustomers = 970, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 51, Branch = branch, Date = DateTime.Parse("2022/01/04"), AmountCustomers = 839, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 52, Branch = branch, Date = DateTime.Parse("2022/01/05"), AmountCustomers = 784, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 53, Branch = branch, Date = DateTime.Parse("2022/01/06"), AmountCustomers = 919, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 54, Branch = branch, Date = DateTime.Parse("2022/01/07"), AmountCustomers = 775, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 55, Branch = branch, Date = DateTime.Parse("2022/01/08"), AmountCustomers = 828, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 56, Branch = branch, Date = DateTime.Parse("2022/01/09"), AmountCustomers = 827, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 57, Branch = branch, Date = DateTime.Parse("2022/01/10"), AmountCustomers = 755, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 58, Branch = branch, Date = DateTime.Parse("2022/01/11"), AmountCustomers = 783, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 59, Branch = branch, Date = DateTime.Parse("2022/01/12"), AmountCustomers = 843, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 60, Branch = branch, Date = DateTime.Parse("2022/01/13"), AmountCustomers = 1010, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 61, Branch = branch, Date = DateTime.Parse("2022/01/14"), AmountCustomers = 993, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 62, Branch = branch, Date = DateTime.Parse("2022/01/15"), AmountCustomers = 835, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 63, Branch = branch, Date = DateTime.Parse("2022/01/16"), AmountCustomers = 772, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 64, Branch = branch, Date = DateTime.Parse("2022/01/17"), AmountCustomers = 968, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 65, Branch = branch, Date = DateTime.Parse("2022/01/18"), AmountCustomers = 1002, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 66, Branch = branch, Date = DateTime.Parse("2022/01/19"), AmountCustomers = 775, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 67, Branch = branch, Date = DateTime.Parse("2022/01/20"), AmountCustomers = 1028, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 68, Branch = branch, Date = DateTime.Parse("2022/01/21"), AmountCustomers = 766, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 69, Branch = branch, Date = DateTime.Parse("2022/01/22"), AmountCustomers = 975, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 70, Branch = branch, Date = DateTime.Parse("2022/01/23"), AmountCustomers = 908, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 71, Branch = branch, Date = DateTime.Parse("2022/01/24"), AmountCustomers = 786, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 72, Branch = branch, Date = DateTime.Parse("2022/01/25"), AmountCustomers = 992, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 73, Branch = branch, Date = DateTime.Parse("2022/01/26"), AmountCustomers = 989, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 74, Branch = branch, Date = DateTime.Parse("2022/01/27"), AmountCustomers = 976, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 75, Branch = branch, Date = DateTime.Parse("2022/01/28"), AmountCustomers = 1033, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 76, Branch = branch, Date = DateTime.Parse("2022/01/29"), AmountCustomers = 811, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 77, Branch = branch, Date = DateTime.Parse("2022/01/30"), AmountCustomers = 886, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 78, Branch = branch, Date = DateTime.Parse("2022/01/31"), AmountCustomers = 819, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 79, Branch = branch, Date = DateTime.Parse("2022/02/01"), AmountCustomers = 855, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 80, Branch = branch, Date = DateTime.Parse("2022/02/02"), AmountCustomers = 1038, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 81, Branch = branch, Date = DateTime.Parse("2022/02/03"), AmountCustomers = 948, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 82, Branch = branch, Date = DateTime.Parse("2022/02/04"), AmountCustomers = 790, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 83, Branch = branch, Date = DateTime.Parse("2022/02/05"), AmountCustomers = 765, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 84, Branch = branch, Date = DateTime.Parse("2022/02/06"), AmountCustomers = 929, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 85, Branch = branch, Date = DateTime.Parse("2022/02/07"), AmountCustomers = 860, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 86, Branch = branch, Date = DateTime.Parse("2022/02/08"), AmountCustomers = 904, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 87, Branch = branch, Date = DateTime.Parse("2022/02/09"), AmountCustomers = 912, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 88, Branch = branch, Date = DateTime.Parse("2022/02/10"), AmountCustomers = 966, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 89, Branch = branch, Date = DateTime.Parse("2022/02/11"), AmountCustomers = 810, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 90, Branch = branch, Date = DateTime.Parse("2022/02/12"), AmountCustomers = 834, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 91, Branch = branch, Date = DateTime.Parse("2022/02/13"), AmountCustomers = 836, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 92, Branch = branch, Date = DateTime.Parse("2022/02/14"), AmountCustomers = 942, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 93, Branch = branch, Date = DateTime.Parse("2022/02/15"), AmountCustomers = 870, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 94, Branch = branch, Date = DateTime.Parse("2022/02/16"), AmountCustomers = 924, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 95, Branch = branch, Date = DateTime.Parse("2022/02/17"), AmountCustomers = 817, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 96, Branch = branch, Date = DateTime.Parse("2022/02/18"), AmountCustomers = 754, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 97, Branch = branch, Date = DateTime.Parse("2022/02/19"), AmountCustomers = 983, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 98, Branch = branch, Date = DateTime.Parse("2022/02/20"), AmountCustomers = 764, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 99, Branch = branch, Date = DateTime.Parse("2022/02/21"), AmountCustomers = 986, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 100, Branch = branch, Date = DateTime.Parse("2022/02/22"), AmountCustomers = 799, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 101, Branch = branch, Date = DateTime.Parse("2022/02/23"), AmountCustomers = 950, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 102, Branch = branch, Date = DateTime.Parse("2022/02/24"), AmountCustomers = 1006, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 103, Branch = branch, Date = DateTime.Parse("2022/02/25"), AmountCustomers = 838, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 104, Branch = branch, Date = DateTime.Parse("2022/02/26"), AmountCustomers = 844, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 105, Branch = branch, Date = DateTime.Parse("2022/02/27"), AmountCustomers = 1021, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 106, Branch = branch, Date = DateTime.Parse("2022/02/28"), AmountCustomers = 926, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 107, Branch = branch, Date = DateTime.Parse("2022/03/01"), AmountCustomers = 828, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 108, Branch = branch, Date = DateTime.Parse("2022/03/02"), AmountCustomers = 766, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 109, Branch = branch, Date = DateTime.Parse("2022/03/03"), AmountCustomers = 920, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 110, Branch = branch, Date = DateTime.Parse("2022/03/04"), AmountCustomers = 957, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 111, Branch = branch, Date = DateTime.Parse("2022/03/05"), AmountCustomers = 976, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 112, Branch = branch, Date = DateTime.Parse("2022/03/06"), AmountCustomers = 898, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 113, Branch = branch, Date = DateTime.Parse("2022/03/07"), AmountCustomers = 957, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 114, Branch = branch, Date = DateTime.Parse("2022/03/08"), AmountCustomers = 806, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 115, Branch = branch, Date = DateTime.Parse("2022/03/09"), AmountCustomers = 973, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 116, Branch = branch, Date = DateTime.Parse("2022/03/10"), AmountCustomers = 987, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 117, Branch = branch, Date = DateTime.Parse("2022/03/11"), AmountCustomers = 988, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 118, Branch = branch, Date = DateTime.Parse("2022/03/12"), AmountCustomers = 785, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 119, Branch = branch, Date = DateTime.Parse("2022/03/13"), AmountCustomers = 899, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 120, Branch = branch, Date = DateTime.Parse("2022/03/14"), AmountCustomers = 1034, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 121, Branch = branch, Date = DateTime.Parse("2022/03/15"), AmountCustomers = 779, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 122, Branch = branch, Date = DateTime.Parse("2022/03/16"), AmountCustomers = 872, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 123, Branch = branch, Date = DateTime.Parse("2022/03/17"), AmountCustomers = 796, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 124, Branch = branch, Date = DateTime.Parse("2022/03/18"), AmountCustomers = 774, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 125, Branch = branch, Date = DateTime.Parse("2022/03/19"), AmountCustomers = 1022, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 126, Branch = branch, Date = DateTime.Parse("2022/03/20"), AmountCustomers = 963, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 127, Branch = branch, Date = DateTime.Parse("2022/03/21"), AmountCustomers = 963, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 128, Branch = branch, Date = DateTime.Parse("2022/03/22"), AmountCustomers = 1026, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 129, Branch = branch, Date = DateTime.Parse("2022/03/23"), AmountCustomers = 836, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 130, Branch = branch, Date = DateTime.Parse("2022/03/24"), AmountCustomers = 810, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 131, Branch = branch, Date = DateTime.Parse("2022/03/25"), AmountCustomers = 971, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 132, Branch = branch, Date = DateTime.Parse("2022/03/26"), AmountCustomers = 1000, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 133, Branch = branch, Date = DateTime.Parse("2022/03/27"), AmountCustomers = 982, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 134, Branch = branch, Date = DateTime.Parse("2022/03/28"), AmountCustomers = 975, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 135, Branch = branch, Date = DateTime.Parse("2022/03/29"), AmountCustomers = 982, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 136, Branch = branch, Date = DateTime.Parse("2022/03/30"), AmountCustomers = 1032, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 137, Branch = branch, Date = DateTime.Parse("2022/03/31"), AmountCustomers = 752, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 138, Branch = branch, Date = DateTime.Parse("2022/04/01"), AmountCustomers = 1027, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 139, Branch = branch, Date = DateTime.Parse("2022/04/02"), AmountCustomers = 821, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 140, Branch = branch, Date = DateTime.Parse("2022/04/03"), AmountCustomers = 753, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 141, Branch = branch, Date = DateTime.Parse("2022/04/04"), AmountCustomers = 1023, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 142, Branch = branch, Date = DateTime.Parse("2022/04/05"), AmountCustomers = 754, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 143, Branch = branch, Date = DateTime.Parse("2022/04/06"), AmountCustomers = 958, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 144, Branch = branch, Date = DateTime.Parse("2022/04/07"), AmountCustomers = 964, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 145, Branch = branch, Date = DateTime.Parse("2022/04/08"), AmountCustomers = 884, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 146, Branch = branch, Date = DateTime.Parse("2022/04/09"), AmountCustomers = 1031, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 147, Branch = branch, Date = DateTime.Parse("2022/04/10"), AmountCustomers = 919, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 148, Branch = branch, Date = DateTime.Parse("2022/04/11"), AmountCustomers = 826, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 149, Branch = branch, Date = DateTime.Parse("2022/04/12"), AmountCustomers = 852, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 150, Branch = branch, Date = DateTime.Parse("2022/04/13"), AmountCustomers = 898, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 151, Branch = branch, Date = DateTime.Parse("2022/04/14"), AmountCustomers = 777, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 152, Branch = branch, Date = DateTime.Parse("2022/04/15"), AmountCustomers = 965, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 153, Branch = branch, Date = DateTime.Parse("2022/04/16"), AmountCustomers = 873, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 154, Branch = branch, Date = DateTime.Parse("2022/04/17"), AmountCustomers = 800, AmountColi = 15, BranchId = 1
                },
                new()
                {
                    Id = 155, Branch = branch, Date = DateTime.Parse("2022/04/18"), AmountCustomers = 900, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 156, Branch = branch, Date = DateTime.Parse("2022/04/19"), AmountCustomers = 1000, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 157, Branch = branch, Date = DateTime.Parse("2022/04/20"), AmountCustomers = 878, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 158, Branch = branch, Date = DateTime.Parse("2022/04/21"), AmountCustomers = 1022, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 159, Branch = branch, Date = DateTime.Parse("2022/04/22"), AmountCustomers = 860, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 160, Branch = branch, Date = DateTime.Parse("2022/04/23"), AmountCustomers = 771, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 161, Branch = branch, Date = DateTime.Parse("2022/04/24"), AmountCustomers = 785, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 162, Branch = branch, Date = DateTime.Parse("2022/04/25"), AmountCustomers = 900, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 163, Branch = branch, Date = DateTime.Parse("2022/04/26"), AmountCustomers = 1100, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 164, Branch = branch, Date = DateTime.Parse("2022/04/27"), AmountCustomers = 1400, AmountColi = 60, BranchId = 1
                },
                new()
                {
                    Id = 165, Branch = branch, Date = DateTime.Parse("2022/04/28"), AmountCustomers = 761, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 166, Branch = branch, Date = DateTime.Parse("2022/04/29"), AmountCustomers = 979, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 167, Branch = branch, Date = DateTime.Parse("2022/04/30"), AmountCustomers = 1030, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 168, Branch = branch, Date = DateTime.Parse("2022/05/01"), AmountCustomers = 852, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 169, Branch = branch, Date = DateTime.Parse("2022/05/02"), AmountCustomers = 963, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 170, Branch = branch, Date = DateTime.Parse("2022/05/03"), AmountCustomers = 1037, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 171, Branch = branch, Date = DateTime.Parse("2022/05/04"), AmountCustomers = 785, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 172, Branch = branch, Date = DateTime.Parse("2022/05/05"), AmountCustomers = 925, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 173, Branch = branch, Date = DateTime.Parse("2022/05/06"), AmountCustomers = 977, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 174, Branch = branch, Date = DateTime.Parse("2022/05/07"), AmountCustomers = 803, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 175, Branch = branch, Date = DateTime.Parse("2022/05/08"), AmountCustomers = 998, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 176, Branch = branch, Date = DateTime.Parse("2022/05/09"), AmountCustomers = 997, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 177, Branch = branch, Date = DateTime.Parse("2022/05/10"), AmountCustomers = 891, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 178, Branch = branch, Date = DateTime.Parse("2022/05/11"), AmountCustomers = 831, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 179, Branch = branch, Date = DateTime.Parse("2022/05/12"), AmountCustomers = 967, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 180, Branch = branch, Date = DateTime.Parse("2022/05/13"), AmountCustomers = 883, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 181, Branch = branch, Date = DateTime.Parse("2022/05/14"), AmountCustomers = 925, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 182, Branch = branch, Date = DateTime.Parse("2022/05/15"), AmountCustomers = 1005, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 183, Branch = branch, Date = DateTime.Parse("2022/05/16"), AmountCustomers = 957, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 184, Branch = branch, Date = DateTime.Parse("2022/05/17"), AmountCustomers = 754, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 185, Branch = branch, Date = DateTime.Parse("2022/05/18"), AmountCustomers = 759, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 186, Branch = branch, Date = DateTime.Parse("2022/05/19"), AmountCustomers = 972, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 187, Branch = branch, Date = DateTime.Parse("2022/05/20"), AmountCustomers = 938, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 188, Branch = branch, Date = DateTime.Parse("2022/05/21"), AmountCustomers = 924, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 189, Branch = branch, Date = DateTime.Parse("2022/05/22"), AmountCustomers = 890, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 190, Branch = branch, Date = DateTime.Parse("2022/05/23"), AmountCustomers = 883, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 191, Branch = branch, Date = DateTime.Parse("2022/05/24"), AmountCustomers = 1014, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 192, Branch = branch, Date = DateTime.Parse("2022/05/25"), AmountCustomers = 815, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 193, Branch = branch, Date = DateTime.Parse("2022/05/26"), AmountCustomers = 951, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 194, Branch = branch, Date = DateTime.Parse("2022/05/27"), AmountCustomers = 814, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 195, Branch = branch, Date = DateTime.Parse("2022/05/28"), AmountCustomers = 1013, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 196, Branch = branch, Date = DateTime.Parse("2022/05/29"), AmountCustomers = 766, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 197, Branch = branch, Date = DateTime.Parse("2022/05/30"), AmountCustomers = 912, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 198, Branch = branch, Date = DateTime.Parse("2022/05/31"), AmountCustomers = 938, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 199, Branch = branch, Date = DateTime.Parse("2022/06/01"), AmountCustomers = 837, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 200, Branch = branch, Date = DateTime.Parse("2022/06/02"), AmountCustomers = 907, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 201, Branch = branch, Date = DateTime.Parse("2022/06/03"), AmountCustomers = 892, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 202, Branch = branch, Date = DateTime.Parse("2022/06/04"), AmountCustomers = 1006, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 203, Branch = branch, Date = DateTime.Parse("2022/06/05"), AmountCustomers = 827, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 204, Branch = branch, Date = DateTime.Parse("2022/06/06"), AmountCustomers = 809, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 205, Branch = branch, Date = DateTime.Parse("2022/06/07"), AmountCustomers = 809, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 206, Branch = branch, Date = DateTime.Parse("2022/06/08"), AmountCustomers = 1022, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 207, Branch = branch, Date = DateTime.Parse("2022/06/09"), AmountCustomers = 1048, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 208, Branch = branch, Date = DateTime.Parse("2022/06/10"), AmountCustomers = 795, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 209, Branch = branch, Date = DateTime.Parse("2022/06/11"), AmountCustomers = 982, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 210, Branch = branch, Date = DateTime.Parse("2022/06/12"), AmountCustomers = 888, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 211, Branch = branch, Date = DateTime.Parse("2022/06/13"), AmountCustomers = 951, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 212, Branch = branch, Date = DateTime.Parse("2022/06/14"), AmountCustomers = 940, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 213, Branch = branch, Date = DateTime.Parse("2022/06/15"), AmountCustomers = 793, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 214, Branch = branch, Date = DateTime.Parse("2022/06/16"), AmountCustomers = 767, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 215, Branch = branch, Date = DateTime.Parse("2022/06/17"), AmountCustomers = 987, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 216, Branch = branch, Date = DateTime.Parse("2022/06/18"), AmountCustomers = 845, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 217, Branch = branch, Date = DateTime.Parse("2022/06/19"), AmountCustomers = 793, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 218, Branch = branch, Date = DateTime.Parse("2022/06/20"), AmountCustomers = 905, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 219, Branch = branch, Date = DateTime.Parse("2022/06/21"), AmountCustomers = 800, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 220, Branch = branch, Date = DateTime.Parse("2022/06/22"), AmountCustomers = 815, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 221, Branch = branch, Date = DateTime.Parse("2022/06/23"), AmountCustomers = 1003, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 222, Branch = branch, Date = DateTime.Parse("2022/06/24"), AmountCustomers = 770, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 223, Branch = branch, Date = DateTime.Parse("2022/06/25"), AmountCustomers = 996, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 224, Branch = branch, Date = DateTime.Parse("2022/06/26"), AmountCustomers = 756, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 225, Branch = branch, Date = DateTime.Parse("2022/06/27"), AmountCustomers = 848, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 226, Branch = branch, Date = DateTime.Parse("2022/06/28"), AmountCustomers = 1005, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 227, Branch = branch, Date = DateTime.Parse("2022/06/29"), AmountCustomers = 1015, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 228, Branch = branch, Date = DateTime.Parse("2022/06/30"), AmountCustomers = 884, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 229, Branch = branch, Date = DateTime.Parse("2022/07/01"), AmountCustomers = 915, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 230, Branch = branch, Date = DateTime.Parse("2022/07/02"), AmountCustomers = 800, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 231, Branch = branch, Date = DateTime.Parse("2022/07/03"), AmountCustomers = 756, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 232, Branch = branch, Date = DateTime.Parse("2022/07/04"), AmountCustomers = 758, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 233, Branch = branch, Date = DateTime.Parse("2022/07/05"), AmountCustomers = 831, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 234, Branch = branch, Date = DateTime.Parse("2022/07/06"), AmountCustomers = 1036, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 235, Branch = branch, Date = DateTime.Parse("2022/07/07"), AmountCustomers = 992, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 236, Branch = branch, Date = DateTime.Parse("2022/07/08"), AmountCustomers = 1034, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 237, Branch = branch, Date = DateTime.Parse("2022/07/09"), AmountCustomers = 1028, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 238, Branch = branch, Date = DateTime.Parse("2022/07/10"), AmountCustomers = 955, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 239, Branch = branch, Date = DateTime.Parse("2022/07/11"), AmountCustomers = 1011, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 240, Branch = branch, Date = DateTime.Parse("2022/07/12"), AmountCustomers = 1046, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 241, Branch = branch, Date = DateTime.Parse("2022/07/13"), AmountCustomers = 962, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 242, Branch = branch, Date = DateTime.Parse("2022/07/14"), AmountCustomers = 965, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 243, Branch = branch, Date = DateTime.Parse("2022/07/15"), AmountCustomers = 797, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 244, Branch = branch, Date = DateTime.Parse("2022/07/16"), AmountCustomers = 816, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 245, Branch = branch, Date = DateTime.Parse("2022/07/17"), AmountCustomers = 911, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 246, Branch = branch, Date = DateTime.Parse("2022/07/18"), AmountCustomers = 779, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 247, Branch = branch, Date = DateTime.Parse("2022/07/19"), AmountCustomers = 1026, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 248, Branch = branch, Date = DateTime.Parse("2022/07/20"), AmountCustomers = 760, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 249, Branch = branch, Date = DateTime.Parse("2022/07/21"), AmountCustomers = 780, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 250, Branch = branch, Date = DateTime.Parse("2022/07/22"), AmountCustomers = 838, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 251, Branch = branch, Date = DateTime.Parse("2022/07/23"), AmountCustomers = 896, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 252, Branch = branch, Date = DateTime.Parse("2022/07/24"), AmountCustomers = 849, AmountColi = 46, BranchId = 1
                },
                /*new HistoricalData
                {
                    Id = 253, Branch = branch, Date = DateTime.Parse("2022/07/25"), AmountCustomers = 775, AmountColi = 36, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 254, Branch = branch, Date = DateTime.Parse("2022/07/26"), AmountCustomers = 1005, AmountColi = 47, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 255, Branch = branch, Date = DateTime.Parse("2022/07/27"), AmountCustomers = 1022, AmountColi = 47, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 256, Branch = branch, Date = DateTime.Parse("2022/07/28"), AmountCustomers = 852, AmountColi = 52, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 257, Branch = branch, Date = DateTime.Parse("2022/07/29"), AmountCustomers = 977, AmountColi = 40, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 258, Branch = branch, Date = DateTime.Parse("2022/07/30"), AmountCustomers = 760, AmountColi = 45, BranchId = 1
                },
                new HistoricalData
                {
                    Id = 259, Branch = branch, Date = DateTime.Parse("2022/07/31"), AmountCustomers = 969, AmountColi = 53, BranchId = 1
                },*/
                new()
                {
                    Id = 260, Branch = branch, Date = DateTime.Parse("2022/08/01"), AmountCustomers = 1003, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 261, Branch = branch, Date = DateTime.Parse("2022/08/02"), AmountCustomers = 858, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 262, Branch = branch, Date = DateTime.Parse("2022/08/03"), AmountCustomers = 925, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 263, Branch = branch, Date = DateTime.Parse("2022/08/04"), AmountCustomers = 993, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 264, Branch = branch, Date = DateTime.Parse("2022/08/05"), AmountCustomers = 983, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 265, Branch = branch, Date = DateTime.Parse("2022/08/06"), AmountCustomers = 898, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 266, Branch = branch, Date = DateTime.Parse("2022/08/07"), AmountCustomers = 799, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 267, Branch = branch, Date = DateTime.Parse("2022/08/08"), AmountCustomers = 933, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 268, Branch = branch, Date = DateTime.Parse("2022/08/09"), AmountCustomers = 950, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 269, Branch = branch, Date = DateTime.Parse("2022/08/10"), AmountCustomers = 1000, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 270, Branch = branch, Date = DateTime.Parse("2022/08/11"), AmountCustomers = 801, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 271, Branch = branch, Date = DateTime.Parse("2022/08/12"), AmountCustomers = 1013, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 272, Branch = branch, Date = DateTime.Parse("2022/08/13"), AmountCustomers = 799, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 273, Branch = branch, Date = DateTime.Parse("2022/08/14"), AmountCustomers = 770, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 274, Branch = branch, Date = DateTime.Parse("2022/08/15"), AmountCustomers = 835, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 275, Branch = branch, Date = DateTime.Parse("2022/08/16"), AmountCustomers = 982, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 276, Branch = branch, Date = DateTime.Parse("2022/08/17"), AmountCustomers = 964, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 277, Branch = branch, Date = DateTime.Parse("2022/08/18"), AmountCustomers = 824, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 278, Branch = branch, Date = DateTime.Parse("2022/08/19"), AmountCustomers = 954, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 279, Branch = branch, Date = DateTime.Parse("2022/08/20"), AmountCustomers = 1029, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 280, Branch = branch, Date = DateTime.Parse("2022/08/21"), AmountCustomers = 754, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 281, Branch = branch, Date = DateTime.Parse("2022/08/22"), AmountCustomers = 931, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 282, Branch = branch, Date = DateTime.Parse("2022/08/23"), AmountCustomers = 893, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 283, Branch = branch, Date = DateTime.Parse("2022/08/24"), AmountCustomers = 883, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 284, Branch = branch, Date = DateTime.Parse("2022/08/25"), AmountCustomers = 817, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 285, Branch = branch, Date = DateTime.Parse("2022/08/26"), AmountCustomers = 837, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 286, Branch = branch, Date = DateTime.Parse("2022/08/27"), AmountCustomers = 958, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 287, Branch = branch, Date = DateTime.Parse("2022/08/28"), AmountCustomers = 958, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 288, Branch = branch, Date = DateTime.Parse("2022/08/29"), AmountCustomers = 951, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 289, Branch = branch, Date = DateTime.Parse("2022/08/30"), AmountCustomers = 820, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 290, Branch = branch, Date = DateTime.Parse("2022/08/31"), AmountCustomers = 918, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 291, Branch = branch, Date = DateTime.Parse("2022/09/01"), AmountCustomers = 1018, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 292, Branch = branch, Date = DateTime.Parse("2022/09/02"), AmountCustomers = 883, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 293, Branch = branch, Date = DateTime.Parse("2022/09/03"), AmountCustomers = 888, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 294, Branch = branch, Date = DateTime.Parse("2022/09/04"), AmountCustomers = 934, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 295, Branch = branch, Date = DateTime.Parse("2022/09/05"), AmountCustomers = 951, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 296, Branch = branch, Date = DateTime.Parse("2022/09/06"), AmountCustomers = 1043, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 297, Branch = branch, Date = DateTime.Parse("2022/09/07"), AmountCustomers = 934, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 298, Branch = branch, Date = DateTime.Parse("2022/09/08"), AmountCustomers = 829, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 299, Branch = branch, Date = DateTime.Parse("2022/09/09"), AmountCustomers = 793, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 300, Branch = branch, Date = DateTime.Parse("2022/09/10"), AmountCustomers = 818, AmountColi = 47, BranchId = 1
                },
                new()
                {
                    Id = 301, Branch = branch, Date = DateTime.Parse("2022/09/11"), AmountCustomers = 774, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 302, Branch = branch, Date = DateTime.Parse("2022/09/12"), AmountCustomers = 873, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 303, Branch = branch, Date = DateTime.Parse("2022/09/13"), AmountCustomers = 1023, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 304, Branch = branch, Date = DateTime.Parse("2022/09/14"), AmountCustomers = 1023, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 305, Branch = branch, Date = DateTime.Parse("2022/09/15"), AmountCustomers = 871, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 306, Branch = branch, Date = DateTime.Parse("2022/09/16"), AmountCustomers = 1018, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 307, Branch = branch, Date = DateTime.Parse("2022/09/17"), AmountCustomers = 851, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 308, Branch = branch, Date = DateTime.Parse("2022/09/18"), AmountCustomers = 877, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 309, Branch = branch, Date = DateTime.Parse("2022/09/19"), AmountCustomers = 993, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 310, Branch = branch, Date = DateTime.Parse("2022/09/20"), AmountCustomers = 950, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 311, Branch = branch, Date = DateTime.Parse("2022/09/21"), AmountCustomers = 918, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 312, Branch = branch, Date = DateTime.Parse("2022/09/22"), AmountCustomers = 989, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 313, Branch = branch, Date = DateTime.Parse("2022/09/23"), AmountCustomers = 780, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 314, Branch = branch, Date = DateTime.Parse("2022/09/24"), AmountCustomers = 797, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 315, Branch = branch, Date = DateTime.Parse("2022/09/25"), AmountCustomers = 764, AmountColi = 36, BranchId = 1
                },
                new()
                {
                    Id = 316, Branch = branch, Date = DateTime.Parse("2022/09/26"), AmountCustomers = 844, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 317, Branch = branch, Date = DateTime.Parse("2022/09/27"), AmountCustomers = 1018, AmountColi = 43, BranchId = 1
                },
                new()
                {
                    Id = 318, Branch = branch, Date = DateTime.Parse("2022/09/28"), AmountCustomers = 843, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 319, Branch = branch, Date = DateTime.Parse("2022/09/29"), AmountCustomers = 833, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 320, Branch = branch, Date = DateTime.Parse("2022/09/30"), AmountCustomers = 844, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 321, Branch = branch, Date = DateTime.Parse("2022/10/01"), AmountCustomers = 887, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 322, Branch = branch, Date = DateTime.Parse("2022/10/02"), AmountCustomers = 867, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 323, Branch = branch, Date = DateTime.Parse("2022/10/03"), AmountCustomers = 919, AmountColi = 45, BranchId = 1
                },
                new()
                {
                    Id = 324, Branch = branch, Date = DateTime.Parse("2022/10/04"), AmountCustomers = 798, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 325, Branch = branch, Date = DateTime.Parse("2022/10/05"), AmountCustomers = 778, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 326, Branch = branch, Date = DateTime.Parse("2022/10/06"), AmountCustomers = 955, AmountColi = 38, BranchId = 1
                },
                new()
                {
                    Id = 327, Branch = branch, Date = DateTime.Parse("2022/10/07"), AmountCustomers = 877, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 328, Branch = branch, Date = DateTime.Parse("2022/10/08"), AmountCustomers = 957, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 329, Branch = branch, Date = DateTime.Parse("2022/10/09"), AmountCustomers = 1015, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 330, Branch = branch, Date = DateTime.Parse("2022/10/10"), AmountCustomers = 970, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 331, Branch = branch, Date = DateTime.Parse("2022/10/11"), AmountCustomers = 947, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 332, Branch = branch, Date = DateTime.Parse("2022/10/12"), AmountCustomers = 758, AmountColi = 51, BranchId = 1
                },
                new()
                {
                    Id = 333, Branch = branch, Date = DateTime.Parse("2022/10/13"), AmountCustomers = 849, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 334, Branch = branch, Date = DateTime.Parse("2022/10/14"), AmountCustomers = 1031, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 335, Branch = branch, Date = DateTime.Parse("2022/10/15"), AmountCustomers = 891, AmountColi = 44, BranchId = 1
                },
                new()
                {
                    Id = 336, Branch = branch, Date = DateTime.Parse("2022/10/16"), AmountCustomers = 816, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 337, Branch = branch, Date = DateTime.Parse("2022/10/17"), AmountCustomers = 939, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 338, Branch = branch, Date = DateTime.Parse("2022/10/18"), AmountCustomers = 890, AmountColi = 48, BranchId = 1
                },
                new()
                {
                    Id = 339, Branch = branch, Date = DateTime.Parse("2022/10/19"), AmountCustomers = 948, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 340, Branch = branch, Date = DateTime.Parse("2022/10/20"), AmountCustomers = 914, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 341, Branch = branch, Date = DateTime.Parse("2022/10/21"), AmountCustomers = 782, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 342, Branch = branch, Date = DateTime.Parse("2022/10/22"), AmountCustomers = 803, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 343, Branch = branch, Date = DateTime.Parse("2022/10/23"), AmountCustomers = 897, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 344, Branch = branch, Date = DateTime.Parse("2022/10/24"), AmountCustomers = 799, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 345, Branch = branch, Date = DateTime.Parse("2022/10/25"), AmountCustomers = 854, AmountColi = 40, BranchId = 1
                },
                new()
                {
                    Id = 346, Branch = branch, Date = DateTime.Parse("2022/10/26"), AmountCustomers = 1014, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 347, Branch = branch, Date = DateTime.Parse("2022/10/27"), AmountCustomers = 1006, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 348, Branch = branch, Date = DateTime.Parse("2022/10/28"), AmountCustomers = 940, AmountColi = 52, BranchId = 1
                },
                new()
                {
                    Id = 349, Branch = branch, Date = DateTime.Parse("2022/10/29"), AmountCustomers = 967, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 350, Branch = branch, Date = DateTime.Parse("2022/10/30"), AmountCustomers = 815, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 351, Branch = branch, Date = DateTime.Parse("2022/10/31"), AmountCustomers = 943, AmountColi = 35, BranchId = 1
                },
                new()
                {
                    Id = 352, Branch = branch, Date = DateTime.Parse("2022/11/01"), AmountCustomers = 808, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 353, Branch = branch, Date = DateTime.Parse("2022/11/02"), AmountCustomers = 971, AmountColi = 49, BranchId = 1
                },
                new()
                {
                    Id = 354, Branch = branch, Date = DateTime.Parse("2022/11/03"), AmountCustomers = 776, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 355, Branch = branch, Date = DateTime.Parse("2022/11/04"), AmountCustomers = 969, AmountColi = 42, BranchId = 1
                },
                new()
                {
                    Id = 356, Branch = branch, Date = DateTime.Parse("2022/11/05"), AmountCustomers = 969, AmountColi = 37, BranchId = 1
                },
                new()
                {
                    Id = 357, Branch = branch, Date = DateTime.Parse("2022/11/06"), AmountCustomers = 840, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 358, Branch = branch, Date = DateTime.Parse("2022/11/07"), AmountCustomers = 801, AmountColi = 41, BranchId = 1
                },
                new()
                {
                    Id = 359, Branch = branch, Date = DateTime.Parse("2022/11/08"), AmountCustomers = 1044, AmountColi = 53, BranchId = 1
                },
                new()
                {
                    Id = 360, Branch = branch, Date = DateTime.Parse("2022/11/09"), AmountCustomers = 975, AmountColi = 39, BranchId = 1
                },
                new()
                {
                    Id = 361, Branch = branch, Date = DateTime.Parse("2022/11/10"), AmountCustomers = 935, AmountColi = 50, BranchId = 1
                },
                new()
                {
                    Id = 362, Branch = branch, Date = DateTime.Parse("2022/11/11"), AmountCustomers = 792, AmountColi = 55, BranchId = 1
                },
                new()
                {
                    Id = 363, Branch = branch, Date = DateTime.Parse("2022/11/12"), AmountCustomers = 956, AmountColi = 46, BranchId = 1
                },
                new()
                {
                    Id = 364, Branch = branch, Date = DateTime.Parse("2022/11/13"), AmountCustomers = 1049, AmountColi = 54, BranchId = 1
                },
                new()
                {
                    Id = 365, Branch = branch, Date = DateTime.Parse("2022/11/14"), AmountCustomers = 754, AmountColi = 47, BranchId = 1
                }
            }
        );


        branch.WorkStandards = _workStandards;
        branch.HistoricalData = _historicalData;
    }
}