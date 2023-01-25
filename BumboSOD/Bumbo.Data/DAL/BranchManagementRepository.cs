using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class BranchManagementRepository : IBranchManagementRepository
{
    private readonly BumboDbContext _context;

    public BranchManagementRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<Branch> GetBranchById(int id)
    {
        return await _context.Branches
            .Include(branch => branch.Address)
            .Include(branch => branch.OpeningHours)
            .FirstOrDefaultAsync(branch => branch.Id == id);
    }

    public async Task<List<Branch>> GetAllBranches()
    {
        return await _context.Branches
            .ToListAsync();
    }

    public async Task<bool> CreateBranch(Branch addedBranch)
    {
        if (_context.Branches.Any(branch => branch == addedBranch))
        {
            return false;
        }

        _context.Branches.Add(addedBranch);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateBranch(Branch updatedBranch, List<OpeningHours> openingHoursList, Address addresses)
    {
        var openingHoursDb = await _context.OpeningHours.Where(oh => oh.BranchId == updatedBranch.Id).ToListAsync();

        //Updating most branch properties
        if (_context.Branches.Any(branch => branch == updatedBranch))
        {
            var foundBranch = await _context.Branches.FirstOrDefaultAsync(branch => branch.Id == updatedBranch.Id);
            foundBranch.Name = updatedBranch.Name;
            foundBranch.ShelfLength = updatedBranch.ShelfLength;
            foundBranch.Address = addresses;
        }
        else
        {
            return false;
        }

        //Updating Opening Hours
        foreach (var openingHours in openingHoursList)
        {
            foreach (var hoursDb in openingHoursDb)
            {
                if (openingHours.Day == hoursDb.Day)
                {
                    hoursDb.OpeningTime = openingHours.OpeningTime;
                    hoursDb.ClosingTime = openingHours.ClosingTime;
                    _context.OpeningHours.Update(hoursDb);
                    break;
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBranch(Branch deletedBranch)
    {
        if (_context.Branches.Any(branch => branch == deletedBranch))
        {
            _context.Branches.Remove(deletedBranch);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}