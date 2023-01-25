namespace Bumbo.Data.Models;

public class ApplicationUserDepartment
{
    public int DepartmentId { get; set; }
    public Department Department;

    public string EmployeeId { get; set; }
    public ApplicationUser Employee { get; set; }
}