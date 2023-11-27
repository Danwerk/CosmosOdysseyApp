using App.BLL.WebDTO;

namespace App.BLL.DTO.WebDTO;

public class PriceList
{
    public Guid Id { get; set; }
    public DateTime ValidUntil { get; set; }
    public ICollection<Leg> Legs { get; set; } = default!;
}