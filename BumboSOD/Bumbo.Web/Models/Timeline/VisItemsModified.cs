namespace Bumbo.Web.Models.Timeline;

public class VisItemsModified
{
    public List<VisItemModel>? NewItems { get; set; }

    public List<VisItemModel>? UpdatedItems { get; set; }

    public List<VisItemModel>? RemovedItems { get; set; }
}