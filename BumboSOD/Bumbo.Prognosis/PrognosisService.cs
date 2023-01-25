using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Prognosis.Repositories;

namespace Bumbo.Prognosis;

public sealed class PrognosisService : IPrognosisService
{
    private readonly IPrognosisRepository _repository;
    private readonly IUserService _userService;
    private PrognosisCalculator _prognosisCalculator;

    public PrognosisService(IPrognosisRepository repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
        _prognosisCalculator = new PrognosisCalculator(_repository);
    }

    /// <summary>
    /// Searches for the prognosis for the given dates.
    /// Creates a prognoses for the dates with missing prognoses
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public async Task<IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>>> GetExistingOrGenerateFreshPrognosisBetweenTime(IList<DateTime> days)
    {
        // How this method works
        // Searches for the existing prognoses
        // If departments are missing one or more prognoses, generate new ones and add the to the DB

        var prognosis = await GetExistingPrognosis(days);
        var totalDepartemntCount = _repository.GetAllDepartments().Count;

        var datesWithIncompletePrognosis = prognosis
            .Where(prognosisForDate => prognosisForDate.Value.Count != totalDepartemntCount)
            .Select(prognosisForDate => prognosisForDate.Key)
            .ToList();

        if (datesWithIncompletePrognosis.Count == 0) return prognosis;

        prognosis = await GenerateFreshPrognosis(datesWithIncompletePrognosis);

        var newPrognose = prognosis.SelectMany(pr => pr.Value.Values, (_, value) => value).ToList();

        await _repository.UpdateOrInsertPrognosis(newPrognose);

        prognosis = await GetExistingPrognosis(days);

        return prognosis;
    }

    /// <summary>
    /// Regenerate the prognosis for the given id's and save save them to the database
    /// </summary>
    /// <param name="ids"></param>
    public async Task OverWritePrognosis(IEnumerable<int> ids)
    {
        // How this method works
        // Find the dates corresponding with the input prognosisIds
        // Regenerate the prognosis for those dates
        // Save to database

        var user = await _userService.GetUser();

        var prognoses = await _repository.GetAllPrognosesFromIds(ids.ToList());

        var datesToUpdate = prognoses.Select(pr => pr.Date).Distinct();

        var freshPrognosisForDays = datesToUpdate
            .SelectMany(date => _prognosisCalculator.CalculateManHoursForEntireBranchForGivenDay(date, user.BranchId!.Value).Values)
            .ToList();

        await _repository.UpdateOrInsertPrognosis(freshPrognosisForDays);
    }

    /// <summary>
    /// Checks if the prognosis where successfully created
    /// Validates if the prognosis used the latest workingstandards
    /// Validates if the prognosis used had access to valid historical data
    /// </summary>
    /// <param name="prognoses"></param>
    /// <returns>Tuple of two booleans</returns>
    public async Task<(bool upToDate, bool validData)> CheckIfPrognosesAreValid(IEnumerable<Data.Models.Prognosis> prognoses)
    {
        // How this methods works:
        // Get the date of the latest working standard
        // check if the prognosis was created after that workingstandard
        // Check if the prognosis is flagged for using incomplete data

        var upToDate = true;
        var validData = true;

        var user = await _userService.GetUser();

        var latestWorkStandardForDep = _repository.BranchWithWorkStandard(user.BranchId!.Value)
            ?.WorkStandards!.MaxBy(ws => ws.DateEntered);

        if (latestWorkStandardForDep is null)
        {
            return (upToDate: true, validData: false);
        }

        foreach (var prognosis in prognoses)
        {
            if (prognosis.CreationDate.CompareTo(latestWorkStandardForDep.DateEntered) == -1)
            {
                upToDate = false;
            }

            if (!prognosis.CalculationSuccessful)
            {
                validData = false;
            }
        }

        return (upToDate, validData);
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
        await _repository.ForceIncreasePrognosis(branchId, day, data);
    }

    private async Task<IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>>> GetExistingPrognosis(IList<DateTime> days)
    {
        var user = await _userService.GetUser();

        return days.ToDictionary(day => day, day => _repository.GetExistingPrognosis(day, user.BranchId!.Value));
    }

    private async Task<IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>>> GenerateFreshPrognosis(IList<DateTime> days)
    {
        var user = await _userService.GetUser();
        return days.ToDictionary(day => day, day => _prognosisCalculator.CalculateManHoursForEntireBranchForGivenDay(day, user.BranchId!.Value));
    }

    public async Task WriteNewPrognosisToDatabase(IList<Data.Models.Prognosis> prognoses)
    {
        await _repository.UpdateOrInsertPrognosis(prognoses);
    }
}