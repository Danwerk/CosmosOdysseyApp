using Domain.Base;

namespace App.Domain;

public class Company : DomainEntityId
{
    public string Name { get; set; } = default!;
    
    public ICollection<Provider>? Providers { get; set; }
}