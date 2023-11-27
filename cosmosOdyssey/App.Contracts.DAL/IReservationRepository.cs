using App.Domain;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IReservationRepository : IBaseRepository<Reservation>,  IReservationRepositoryCustom<Reservation>
{
}
public interface IReservationRepositoryCustom<TEntity>
{
    //add here shared methods between repo and service
    public Task<IEnumerable<TEntity>> AllAsync(Guid userId);
    
    public Task<TEntity?> FindAsync(Guid id, Guid userId);

   
}







