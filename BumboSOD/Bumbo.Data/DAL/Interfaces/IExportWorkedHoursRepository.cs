using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IExportWorkedHoursRepository
{
    Task<Branch?> GetBranch(int? branchId);
    Task RenumerateClockedHours(int? branchId, DateTime startDate, DateTime endDate);
}