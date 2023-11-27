using App.BLL.WebDTO;

namespace App.BLL.DTO.WebDTO;

public class RouteInfo
{
    public Guid Id { get; set; }
    public Location From { get; set; } = default!;
    public Location To { get; set; } = default!;
    public long Distance { get; set; }
}