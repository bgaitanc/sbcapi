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

    public async Task<Account?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<IEnumerable<Account>> GetRootsWithChildrenAsync()
    {
        // Para soportar N niveles, traemos todas las cuentas y construimos el árbol en memoria o usamos Includes profundos.
        // Dado que un catálogo contable no suele ser masivo, traer todo es viable.
        return await _dbSet
            .Include(a => a.SubAccounts)
            .Where(a => a.ParentAccountId == null)
            .OrderBy(a => a.Code)
            .ToListAsync();
    }
}
