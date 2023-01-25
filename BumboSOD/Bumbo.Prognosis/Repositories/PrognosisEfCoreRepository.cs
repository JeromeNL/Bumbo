using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Prognosis.Repositories;

public class PrognosisEfCoreRepository : IPrognosisRepository
{
    private readonly BumboDbContext _bumboDbContext;

    public PrognosisEfCoreRepository(BumboDbContext bumboDbContext)
    {
        _bumboDbContext = bumboDbContext;
    }

    public HistoricalData? FindHistoricalDataForThisDate(DateTime currentDate, int branchId)
    {
        return _bumboDbContext.HistoricalData.FirstOrDefault(e => e.Date == currentDate && e.BranchId == branchId);
    }

    public IList<Department> GetAllDepartments()
    {
        return _bumboDbContext.Departments.ToList();
    }

    public Branch? BranchWithWorkStandard(int branchId)
    {
        return _bumboDbContext.Branches.Include(b => b.WorkStandards).FirstOrDefault(b => b.Id == branchId);
    }

    public List<HistoricalData> GetHistoricalDataDescending(int branchId)
    {
        return _bumboDbContext.HistoricalData.Where(e => e.BranchId == branchId).OrderByDescending(e => e.Date).ToList();
    }

    public IDictionary<Department, Data.Models.Prognosis> GetExistingPrognosis(DateTime day, int branchId)
    {
        var prognosisForDay = _bumboDbContext.Prognoses
            .Include(pr => pr.Department)
            .Where(pr => pr.BranchId == branchId && pr.Date == day).ToList();

        return prognosisForDay.ToDictionary(pr => pr.Department, pr => pr);
    }

    public async Task UpdateOrInsertPrognosis(IList<Data.Models.Prognosis> prognoses)
    {
        // loop through all input prognoses
        // if there's already a prognosis on that date for given department branch
        //  - query it and update it's fields
        // else
        // - add it to the database

        foreach (var newPrognosis in prognoses)
        {
            var oldPrognosis = await _bumboDbContext.Prognoses.Where(oldPrognosis => oldPrognosis.Date == newPrognosis.Date && oldPrognosis.BranchId == newPrognosis.BranchId && oldPrognosis.DepartmentId == newPrognosis.DepartmentId).FirstOrDefaultAsync();

            if (oldPrognosis is null)
            {
                _bumboDbContext.Prognoses.Add(newPrognosis);
                continue;
            }

            oldPrognosis.CalculationSuccessful = newPrognosis.CalculationSuccessful;
            oldPrognosis.ManHoursExpected = newPrognosis.ManHoursExpected;
            oldPrognosis.CreationDate = newPrognosis.CreationDate;
            _bumboDbContext.Entry(oldPrognosis).State = EntityState.Modified;
            await _bumboDbContext.SaveChangesAsync();
        }

        // Save changes to database
        await _bumboDbContext.SaveChangesAsync();
    }

    public async Task<IList<Data.Models.Prognosis>> GetAllPrognosesFromIds(IList<int> ints)
    {
        var prognoses = await _bumboDbContext.Prognoses.Where(pr => ints.Contains(pr.Id)).ToListAsync();
        return prognoses;
    }

    /// <summary>
    /// Increases the prognoses for a branch by adding/subtracting given amount of hours.
    /// </summary>
    /// <param name="branchId"></param>
    /// <param name="day"></param>
    /// <param name="data">
    /// Key => Prognosis ID
    /// Value => Amount of hours to add or subtract (can be negative)
    /// </param>
    public async Task ForceIncreasePrognosis(int branchId, DateTime day, IDictionary<int, int> data)
    {
        var oldPrognoses = await _bumboDbContext.Prognoses.Where(pr => pr.BranchId == branchId && pr.Date.Date.Equals(day.Date)).ToListAsync();

        foreach (var prognosisOffset in data)
        {
            var manHours = oldPrognoses.First(pr => pr.DepartmentId == prognosisOffset.Key).ManHoursExpected;

            manHours += prognosisOffset.Value;

            if (manHours <= 0)
            {
                manHours = 0;
            }

            if (manHours > 999)
            {
                manHours = 999;
            }

            oldPrognoses.First(pr => pr.DepartmentId == prognosisOffset.Key).ManHoursExpected = manHours;
        }

        await _bumboDbContext.SaveChangesAsync();
    }
}