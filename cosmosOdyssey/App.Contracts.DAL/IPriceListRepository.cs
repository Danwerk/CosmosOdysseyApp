using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IPriceListRepository: IBaseRepository<PriceList>,  IPriceListRepositoryCustom<PriceList>
{
}
public interface IPriceListRepositoryCustom<TEntity>
{
    //add here shared methods between repo and service
    // public Task<IEnumerable<TEntity>> AllAsync();
    
    public Task<TEntity?> FindAsync(Guid id);

   
}

