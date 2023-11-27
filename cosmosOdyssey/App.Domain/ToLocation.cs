using Domain.Base;

namespace App.Domain;

public class ToLocation : DomainEntityId
{
    public string Name { get; set; } = default!;
    public ICollection<RouteInfo>? RouteInfos { get; set; }
}