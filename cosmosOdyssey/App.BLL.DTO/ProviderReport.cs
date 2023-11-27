namespace App.BLL.DTO;

public class ProviderReport
{
    public Guid? Id { get; set; }
    public Guid? LegId { get; set; }
    public decimal Price { get; set; }
    public DateTime FlightStart { get; set; }
    public DateTime FlightEnd { get; set; }
    public TimeSpan TravelTime { get; set; }
    public string? CompanyName { get; set; }
    public string FromLocation { get; set; } = default!;
    public string ToLocation { get; set; } = default!;
    public long Distance { get; set; }
    public DateTime ValidUntil { get; set; }
}