using System.Runtime.Serialization;

namespace Bumbo.Web.Models.Timeline;

public enum VisItemType
{
    [EnumMember(Value = "box")]
    Box,

    [EnumMember(Value = "point")]
    Point,

    [EnumMember(Value = "range")]
    Range,

    [EnumMember(Value = "background")]
    Background
}