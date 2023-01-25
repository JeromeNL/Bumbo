using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IHistoryRepository
{
    Task<List<HistoricalData>> GetHistoricalData(DateTime startDate, DateTime endDate);
    Task UpdateHistoricalDataInDb(List<HistoricalData> submittedHistoricalData, List<HistoricalData> historicalDbData);
}