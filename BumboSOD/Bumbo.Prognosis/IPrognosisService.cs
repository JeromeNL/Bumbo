using Bumbo.Data.Models;

namespace Bumbo.Prognosis;

public interface IPrognosisService
{
    /// <summary>
    /// Searches for the prognosis for the given dates.
    /// Creates a prognoses for the dates with missing prognoses
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public Task<IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>>> GetExistingOrGenerateFreshPrognosisBetweenTime(IList<DateTime> days);

    /// <summary>
    /// Regenerate the prognosis for the given id's and save save them to the database
    /// </summary>
    /// <param name="ids"></param>
    public Task OverWritePrognosis(IEnumerable<int> ids);

    /// <summary>
    /// Checks if the prognosis where successfully created
    /// Validates if the prognosis used the latest workingstandards
    /// Validates if the prognosis used had access to valid historical data
    /// </summary>
    /// <param name="prognoses"></param>
    /// <returns>Tuple of two booleans</returns>
    public Task<(bool upToDate, bool validData)> CheckIfPrognosesAreValid(IEnumerable<Data.Models.Prognosis> prognoses);

    /// <summary>
    /// Increases the prognoses for a branch by adding/subtracting given amount of hours.
    /// </summary>
    /// <param name="branchId"></param>
    /// <param name="day"></param>
    /// <param name="data">
    /// Key => Prognosis ID
    /// Value => Amount of hours to add or subtract (can be negative)
    /// </param>
    public Task ForceIncreasePrognosis(int branchId, DateTime day, IDictionary<int, int> data);
}