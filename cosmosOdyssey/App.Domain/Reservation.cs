using App.Domain.Identity;
using Domain.Base;

namespace App.Domain;

public class Reservation : DomainEntityId
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime CreatedAt { get; set; } 
    
    public decimal TotalPrice { get; set; }
    public TimeSpan TotalTravelTime { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    // public ICollection<Provider>? Providers { get; set; }
}