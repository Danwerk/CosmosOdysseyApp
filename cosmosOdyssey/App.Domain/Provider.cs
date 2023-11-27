using Domain.Base;

namespace App.Domain;

public class Provider : DomainEntityId
{
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public Guid LegId { get; set; }
    public Leg? Leg { get; set; }
    //
    // public Guid ReservationId { get; set; }
    // public Reservation? Reservation { get; set; }
    
    public decimal Price { get; set; }
    public DateTime FlightStart { get; set; }
    public DateTime FlightEnd { get; set; }
}