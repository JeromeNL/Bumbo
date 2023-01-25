namespace Bumbo.Web.Models.Timeline;

public class TimelinePrognosisViewModel
{
    public string HoursPlannedRangeJson { get; set; }

    public string PrognosisRangeJson { get; set; }

    public bool PrognosisOutdated { get; set; } = true;

    public bool PrognosisHadInvalidData { get; set; } = true;

    public int[] PrognosisToUpdate { get; set; }
}