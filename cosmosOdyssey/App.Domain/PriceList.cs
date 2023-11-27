using Domain.Base;

namespace App.Domain;

public class PriceList : DomainEntityId
{
    public DateTime ValidUntil { get; set; }
    
    public ICollection<Leg>? Legs { get; set; }
}