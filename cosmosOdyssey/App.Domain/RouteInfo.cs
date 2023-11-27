using Domain.Base;

namespace App.Domain;

public class RouteInfo : DomainEntityId
{
    public Guid FromLocationId { get; set; }
    public FromLocation? FromLocation { get; set; }

    public Guid ToLocationId { get; set; }
    public ToLocation? ToLocation { get; set; }

    public long Distance { get; set; }

    public ICollection<Leg>? Legs { get; set; }
}