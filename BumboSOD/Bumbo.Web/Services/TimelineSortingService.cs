using Bumbo.Web.Models.Timeline;
using Bumbo.Web.Services.Interfaces;

namespace Bumbo.Web.Services;

public class TimelineSortingService : ITimelineSortingService
{
    public Dictionary<string, List<int>> GenerateSortings(TimelineViewModel timelineViewModel)
    {
        var groups = timelineViewModel.Groups;
        var shiftItems = timelineViewModel.ShiftItems;

        // All order options
        var startShift = shiftItems.OrderBy(o => o.StartDate).ToList()
            .Select(o => o.Group).Distinct().ToList();
        var endShift = shiftItems.OrderBy(o => o.EndDate).ToList()
            .Select(o => o.Group).Distinct().ToList();
        var ageSort = groups.OrderByDescending(o => o.Employee.BirthDate).ToList()
            .Select(o => o.Id).ToList();

        var workedHoursDifference = GetWorkedHoursDifference(groups, shiftItems, timelineViewModel.ClockedHourItems);
        var leastWorkedHours = GetLeastAmountOfShiftTime(groups, shiftItems);
        var mostAvailability = GetMostAmountOfAvailability(groups, timelineViewModel.AvailabilityItems);


        var dict = new Dictionary<string, List<int>>
        {
            { "Voornaam", groups.Select(o => o.Id).ToList() },
            { "Starttijd dienst", startShift },
            { "Eindtijd dienst", endShift },
            { "Verschil in gewerkte uren", workedHoursDifference },
            { "Ingeplande uren", leastWorkedHours },
            { "Beschikbaarheid", mostAvailability },
            { "Leeftijd", ageSort }
        };
        return dict;
    }

    private List<int> GetWorkedHoursDifference(List<VisGroupModel> groups, List<VisItemModel> shiftItems, List<VisItemModel> clockedHoursItems)
    {
        var minuteDifferenceDictionary = new Dictionary<int, int>();
        foreach (var group in groups)
        {
            var hours = 0;
            foreach (var item in shiftItems)
            {
                if (item.Group == group.Id)
                {
                    hours += (int)(DateTime.Parse(item.EndDate) - DateTime.Parse(item.StartDate)).TotalMinutes;
                }
            }

            foreach (var item in clockedHoursItems)
            {
                if (item.Group == group.Id)
                {
                    hours -= (int)(DateTime.Parse(item.EndDate) - DateTime.Parse(item.StartDate)).TotalMinutes;
                }
            }

            minuteDifferenceDictionary.Add(group.Id, Math.Abs(hours));
        }

        var sorterdDictionary = minuteDifferenceDictionary.OrderByDescending(d => d.Value).ToDictionary(x => x.Key, x => x.Value);
        return sorterdDictionary.Keys.ToList();
    }

    private List<int> GetLeastAmountOfShiftTime(List<VisGroupModel> groups, List<VisItemModel> shiftItems)
    {
        var minuteDifferenceDictionary = new Dictionary<int, int>();
        foreach (var group in groups)
        {
            var hours = 0;
            foreach (var item in shiftItems)
            {
                if (item.Group == group.Id)
                {
                    hours += (int)(DateTime.Parse(item.EndDate) - DateTime.Parse(item.StartDate)).TotalMinutes;
                }
            }

            minuteDifferenceDictionary.Add(group.Id, Math.Abs(hours));
        }

        var sortedDictionary = minuteDifferenceDictionary.OrderBy(d => d.Value).ToDictionary(x => x.Key, x => x.Value);
        return sortedDictionary.Keys.ToList();
    }

    private List<int> GetMostAmountOfAvailability(List<VisGroupModel> groups, List<VisItemModel> availability)
    {
        var minuteDifferenceDictionary = new Dictionary<int, int>();
        foreach (var group in groups)
        {
            var hours = 0;
            foreach (var item in availability)
            {
                if (item.Group == group.Id)
                {
                    hours += (int)(DateTime.Parse(item.EndDate) - DateTime.Parse(item.StartDate)).TotalMinutes;
                }
            }

            minuteDifferenceDictionary.Add(group.Id, Math.Abs(hours));
        }

        var sortedDictionary = minuteDifferenceDictionary.OrderByDescending(d => d.Value).ToDictionary(x => x.Key, x => x.Value);
        return sortedDictionary.Keys.ToList();
    }
}