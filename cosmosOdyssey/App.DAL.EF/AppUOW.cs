using App.Contracts.DAL;
using Base.DAL.EF;
using DAL.EF.Repositories;

namespace DAL.EF;

public class AppUOW : EFBaseUOW<ApplicationDbContext>, IAppUOW
{
    public AppUOW(ApplicationDbContext dataContext) : base(dataContext)
    {
    }
    
    public IReservationRepository? _reservationRepository;
    public IPriceListRepository? _priceListRepository;
    public IReservationRepository ReservationRepository => _reservationRepository ??= new ReservationRepository(UowDbContext);
    public IPriceListRepository PriceListRepository => _priceListRepository ??= new PriceListRepository(UowDbContext);
    
}