namespace App.Contracts.BLL.Services;

public interface ISearchProvider
{
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Company { get; set; }
}