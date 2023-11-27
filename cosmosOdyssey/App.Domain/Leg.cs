using Domain.Base;

namespace App.Domain;

public class Leg : DomainEntityId
{
    public Guid RouteInfoId { get; set; }
    public RouteInfo? RouteInfo { get; set; }
    
    public Guid PriceListId { get; set; }
    public PriceList? PriceList { get; set; }

    public ICollection<Provider>? Providers { get; set; }
}