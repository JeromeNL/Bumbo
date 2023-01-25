using System.ComponentModel;

namespace Bumbo.Data.Models.Enums;

public enum WorkStandardTypes
{
    [Description("Coli uitladen")]
    Uitladen = 0,

    [Description("Vakken vullen")]
    VakkenVullen = 1,

    [Description("Tijd aan kassa per klant")]
    Kassa = 2,

    [Description("Tijd per klant voor vers")]
    Vers = 3,

    [Description("Spiegelen per meter")]
    Spiegelen = 4
}