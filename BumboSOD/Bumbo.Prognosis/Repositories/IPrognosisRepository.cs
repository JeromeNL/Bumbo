using Bumbo.Data.Models;

namespace Bumbo.Prognosis.Repositories;

public interface IPrognosisRepository
{
    HistoricalData? FindHistoricalDataForThisDate(DateTime currentDate, int branchId);

    IList<Department> GetAllDepartments();

    Branch? BranchWithWorkStandard(int branchId);

    List<HistoricalData> GetHistoricalDataDescending(int branchId);

    IDictionary<Department, Data.Models.Prognosis> GetExistingPrognosis(DateTime day, int branchId);

    Task UpdateOrInsertPrognosis(IList<Data.Models.Prognosis> prognoses);

    Task<IList<Data.Models.Prognosis>> GetAllPrognosesFromIds(IList<int> ints);

    /// <summary>
    /// Increases the prognoses for a branch by adding/subtracting given amount of hours.
    /// </summary>
    /// <param name="branchId"></param>
    /// <param name="day"></param>
    /// <param name="data">
    /// Key => Prognosis ID
    /// Value => Amount of hours to add or subtract (can be negative)
    /// </param>
    Task ForceIncreasePrognosis(int branchId, DateTime day, IDictionary<int, int> data);
}