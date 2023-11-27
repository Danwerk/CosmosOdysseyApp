using App.Contracts.DAL;
using App.Domain;
using AutoMapper;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF.Repositories;

public class ReservationRepository : EFBaseRepository<Reservation, ApplicationDbContext>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext dataContext) : base(dataContext)
    {
    }


    public async Task<IEnumerable<Reservation>> AllAsync(Guid userId)
    {
        return await RepositoryDbSet
            .Include(e => e.AppUser)
            .Where(e => e.AppUserId == userId)
            .ToListAsync();
    }

    public async Task<Reservation?> FindAsync(Guid id, Guid userId)
    {
            return await RepositoryDbSet
                .Include(r => r.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);
    }
}