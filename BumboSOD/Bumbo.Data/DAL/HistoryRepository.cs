using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class HistoryRepository : IHistoryRepository
{
    private readonly BumboDbContext _context;
    private readonly IUserService _userService;

    public HistoryRepository(BumboDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<List<HistoricalData>> GetHistoricalData(DateTime startDate, DateTime endDate)
    {
        var user = await _userService.GetUser();
        return await _context.HistoricalData.AsNoTracking().Where(x => x.Date >= startDate.Date && x.Date <= endDate.Date && x.Branch!.Id == user.BranchId).ToListAsync();
    }

    public async Task UpdateHistoricalDataInDb(List<HistoricalData> submittedHistoricalData, List<HistoricalData> historicalDbData)
    {
        var historicalDataToAddToDb = submittedHistoricalData.ToList();

        foreach (var item in historicalDataToAddToDb)
        {
            var user = await _userService.GetUserAdvanced();
            item.Branch = user.Branch;
        }

        // Check if there are dates that should only be updated, not added
        foreach (var historicalData in historicalDbData)
        {
            foreach (var item in submittedHistoricalData.Where(item => item.Date.CompareTo(historicalData.Date) == 0))
            {
                historicalData.AmountColi = item.AmountColi;
                historicalData.AmountCustomers = item.AmountCustomers;
                // Remove item from db add list as it is already updated
                historicalDataToAddToDb.Remove(item);
            }
        }

        // All dates that aren't in the db will be added
        _context.AddRange(historicalDataToAddToDb);
        await _context.SaveChangesAsync();
    }
}