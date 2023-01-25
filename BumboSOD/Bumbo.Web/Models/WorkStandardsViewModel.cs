using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;

namespace Bumbo.Web.Models;

public class WorkStandardsViewModel
{
    public WorkStandards Uitladen { get; set; }
    public WorkStandards VakkenVullen { get; set; }
    public WorkStandards Kassa { get; set; }
    public WorkStandards Vers { get; set; }
    public WorkStandards Spiegelen { get; set; }

    public Dictionary<DateTime, List<WorkStandards>> PastWorkStandards { get; set; }

    public void SetWorkStandardTypes()
    {
        Uitladen.Task = WorkStandardTypes.Uitladen;
        VakkenVullen.Task = WorkStandardTypes.VakkenVullen;
        Kassa.Task = WorkStandardTypes.Kassa;
        Vers.Task = WorkStandardTypes.Vers;
        Spiegelen.Task = WorkStandardTypes.Spiegelen;
    }
}