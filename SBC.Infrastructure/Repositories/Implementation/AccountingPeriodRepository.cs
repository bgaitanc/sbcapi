using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class AccountingPeriodRepository(SbcDbContext context) 
    : BaseRepository<AccountingPeriod>(context), IAccountingPeriodRepository
{
    public async Task<AccountingPeriod?> GetByPeriodAsync(int year, int month)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
    }

    public async Task<bool> IsPeriodClosedAsync(int year, int month)
    {
        return await _dbSet.AnyAsync(p => p.Year == year && p.Month == month && p.IsClosed);
    }
}
