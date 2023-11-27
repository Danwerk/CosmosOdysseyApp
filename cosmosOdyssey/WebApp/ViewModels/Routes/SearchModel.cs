using App.BLL.DTO;
using App.Contracts.BLL.Services;

namespace WebApp.ViewModels.Routes;
#pragma warning disable 1591


public class SearchModel : ISearchProvider
{
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Company { get; set; }
    
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }

    public List<ProviderReport>? Results { get; set; }

   
}