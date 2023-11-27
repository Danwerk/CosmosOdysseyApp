using System.Security.Claims;
using App.BLL.DTO;
using App.Contracts.BLL.Services;
using App.Domain.Identity;
using DAL.EF;
using Microsoft.Extensions.Logging;
using Reservation = App.Domain.Reservation;

namespace App.BLL.Services;

public class ReservationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(ApplicationDbContext ctx, ILogger<ReservationService> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<Guid?> CreateReservation(ICreateReservation reservationModel, Guid appUserId)
    {
        Reservation reservation = new Reservation()
        {
            FirstName = reservationModel.FirstName,
            LastName = reservationModel.LastName,
            CreatedAt = DateTime.UtcNow,
            AppUserId = appUserId,
            TotalPrice = reservationModel.ProviderReport.Price,
            TotalTravelTime = GetTravelTime(reservationModel.ProviderReport)
        };

        _ctx.Reservations.Add(reservation);
        await _ctx.SaveChangesAsync();
        
        return reservation.Id;
    }


    
    
    public static TimeSpan GetTravelTime(ProviderReport report)
    {
        return report.FlightEnd - report.FlightStart;
    }

}