using App.Contracts.DAL;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF.Repositories;

public class PriceListRepository: EFBaseRepository<PriceList, ApplicationDbContext>, IPriceListRepository
{
    public PriceListRepository(ApplicationDbContext dataContext) : base(dataContext)
    {
    }


    public async Task<IEnumerable<PriceList>> AllAsync()
    {
        return await RepositoryDbSet
            .Include(p => p.Legs)
            .ToListAsync();
    }

    public async Task<PriceList?> FindAsync(Guid id)
    {
        return await RepositoryDbSet
            .Include(p => p.Legs)
            .FirstOrDefaultAsync(m => m.Id == id && m.Id == id);
    }
}