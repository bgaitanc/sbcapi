using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class AccountRepository(SbcDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public override async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.ParentAccount)
            .Include(a => a.SubAccounts)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public override async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.ParentAccount)
            .OrderBy(a => a.Code)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _dbSet.AnyAsync(a => a.Code == code);
    }
}
