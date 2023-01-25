using Bumbo.Data.Models;

namespace Bumbo.Web.Services.Interfaces;

public interface ITimelinePrognosisService
{
    IDictionary<string, IDictionary<string, decimal>> RewriteDictionary(IDictionary<DateTime, IDictionary<Department, decimal>> originalDict);
    IDictionary<string, IDictionary<string, decimal>> RewriteDictionary(IDictionary<DateTime, IDictionary<Department, Data.Models.Prognosis>> originalDict);
}