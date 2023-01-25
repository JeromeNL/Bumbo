using Bumbo.Data.Models;
using Newtonsoft.Json;

namespace Bumbo.Web.Models.Timeline;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class VisGroupModel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("employeeId")]
    public string EmployeeId => Employee.Id;

    [JsonIgnore]
    public ApplicationUser Employee { get; set; }

    [JsonProperty("availableDepartments")]
    public List<int> AvailableDepartments { get; set; }

    [JsonProperty("subgroupOrder")]
    public string? SubgroupOrder { get; set; }

    [JsonProperty("subgroupStack")]
    public bool SubgroupStack { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("order")]
    public int Order { get; set; }
}