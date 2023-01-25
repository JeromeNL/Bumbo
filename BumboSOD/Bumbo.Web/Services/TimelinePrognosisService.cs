using Bumbo.Data.Models;
using Bumbo.Web.Services.Interfaces;

namespace Bumbo.Web.Services;

public class TimelinePrognosisService : ITimelinePrognosisService
{
    public IDictionary<string, IDictionary<string, decimal>> RewriteDictionary(IDictionary<DateTime, IDictionary<Department, decimal>> originalDict)
    {
        var newDict = new Dictionary<string, IDictionary<string, decimal>>();

        // Iterate over the original dictionary and add the key-value pairs to the new dictionary
        foreach (var entry in originalDict)
        {
            var innerDict = entry.Value.ToDictionary(innerEntry => innerEntry.Key.Name, innerEntry => innerEntry.Value);
            newDict.Add(entry.Key.ToString("yyyy-MM-dd"), innerDict);
        }

        return newDict;
    }

    public IDictionary<string, IDictionary<string, decimal>> RewriteDictionary(IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>> originalDict)
    {
        var newDict = new Dictionary<string, IDictionary<string, decimal>>();

        // Iterate over the original dictionary and add the key-value pairs to the new dictionary
        foreach (var entry in originalDict)
        {
            var innerDict = entry.Value.ToDictionary(innerEntry => innerEntry.Key.Name, innerEntry => innerEntry.Value.ManHoursExpected);
            newDict.Add(entry.Key.ToString("yyyy-MM-dd"), innerDict);
        }

        return newDict;
    }
}