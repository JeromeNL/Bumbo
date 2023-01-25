using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models;

public class CopyScheduleViewModel : IValidatableObject
{
    public string StartDateTime { get; set; }
    public string EndDateTime { get; set; }

    public string DesiredWeekStartDateTime { get; set; }
    public string DesiredWeekEndDateTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var correctDesiredWeekStartDateTime = DateTime.Parse(DesiredWeekStartDateTime);
        var correctDesiredWeekEndDateTime = DateTime.Parse(DesiredWeekEndDateTime);
        var correctEndDateTime = DateTime.Parse(EndDateTime);
        if ((correctDesiredWeekEndDateTime - correctDesiredWeekStartDateTime).Days != 6)
        {
            yield return new ValidationResult("Je hebt geprobeerd om een week te selecteren die niet precies 7 dagen is. Probeer het opnieuw met een juiste week.", new[] { nameof(DesiredWeekStartDateTime), nameof(DesiredWeekEndDateTime) });
        }
        else if (correctDesiredWeekStartDateTime < correctEndDateTime)
        {
            yield return new ValidationResult("Je hebt geprobeerd om een week te selecteren die in het verleden ligt. Probeer het opnieuw met een juiste week.", new[] { nameof(DesiredWeekStartDateTime), nameof(DesiredWeekEndDateTime) });
        }
    }
}