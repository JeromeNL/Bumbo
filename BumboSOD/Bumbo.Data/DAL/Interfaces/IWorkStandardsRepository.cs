using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IWorkStandardsRepository
{
    Task<WorkStandards?> GetUitladenWorkStandard();

    Task<WorkStandards?> GetVakkenVullenWorkStandard();

    Task<WorkStandards?> GetKassaWorkStandard();

    Task<WorkStandards?> GetVersWorkStandard();

    Task<WorkStandards?> GetSpiegelenWorkStandard();
    Task AddWorkStandards(List<WorkStandards> workStandards);

    Task<List<WorkStandards>?> GetPastWorkStandardsList(WorkStandards currentWorkStandard);

    Task<Dictionary<DateTime, List<WorkStandards>>?> GetPastWorkStandardsDictionary(WorkStandards currentWorkStandard);
}