using Bumbo.Data.Models;

namespace Bumbo.Data.DAL.Interfaces;

public interface IBranchManagementRepository
{
    public Task<Branch> GetBranchById(int id);

    public Task<List<Branch>> GetAllBranches();

    public Task<bool> CreateBranch(Branch branch);

    public Task<bool> UpdateBranch(Branch branch, List<OpeningHours> openingHoursList, Address openingAddressList);

    public Task<bool> DeleteBranch(Branch branch);
}