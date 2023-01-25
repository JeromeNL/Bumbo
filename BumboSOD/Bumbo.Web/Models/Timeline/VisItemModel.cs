using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bumbo.Web.Models.Timeline;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class VisItemModel
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("customId")]
    public int? CustomId { get; set; }

    [JsonProperty("employeeId")]
    public string? EmployeeId { get; set; }

    [JsonProperty("group")]
    public int Group { get; set; }

    [JsonProperty("start")]
    public string StartDate { get; set; }

    [JsonProperty("end")]
    public string EndDate { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("isIll")]
    public bool IsIll { get; set; }

    [JsonProperty("departmentId")]
    public int? DepartmentId { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public VisItemType? Type { get; set; }

    [JsonProperty("subgroup")]
    public int? Subgroup { get; set; }

    [JsonProperty("className")]
    public string? ClassName { get; set; }

    [JsonProperty("editable")]
    public bool Editable { get; set; }
}