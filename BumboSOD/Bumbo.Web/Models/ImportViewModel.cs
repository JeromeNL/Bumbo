using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models;

public class ImportViewModel
{
    [Display(Name = "Bestand")]
    public IFormFile File { get; set; }

    [Display(Name = "Filiaal")]
    public int BranchId { get; set; }
}