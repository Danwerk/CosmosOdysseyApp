using App.BLL.DTO;
using App.Contracts.BLL.Services;
using App.Domain;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace App.BLL.Services;

public class RouteService
{
    private readonly ApplicationDbContext _ctx;
    private readonly ILogger<RouteService> _logger;

    public RouteService(ApplicationDbContext ctx, ILogger<RouteService> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<List<ProviderReport>?> GetFilteredAndSorteredRoutesResults(ISearchProvider searchModel,
        string sortBy)
    {
        var lastActivePriceList = await GetLastActivePriceList();

        if (lastActivePriceList == null)
        {
            _logger.LogInformation("No active priceLists present");
            return Enumerable.Empty<ProviderReport>().ToList();
        }

        var providersQuery = BuildProvidersQuery(lastActivePriceList, searchModel);

        var providers = await providersQuery.ToListAsync();

        var searchResults = MapProvidersToReports(providers);

        var filterResults = await SortProviderReportsResult(sortBy, searchResults);

        return filterResults;
    }


    private async Task<PriceList?> GetLastActivePriceList()
    {
        return await _ctx.PriceLists
            .OrderByDescending(pl => pl.ValidUntil)
            .FirstOrDefaultAsync();
    }


    private IQueryable<Provider> BuildProvidersQuery(PriceList lastActivePriceList, ISearchProvider searchModel)
    {
        return _ctx.Providers
            .Include(p => p.Company)
            .Include(p => p.Leg)
            .ThenInclude(r => r!.RouteInfo)
            .ThenInclude(r => r!.FromLocation)
            .Include(p => p.Leg)
            .ThenInclude(r => r!.RouteInfo)
            .ThenInclude(r => r!.ToLocation)
            .Where(provider => provider.Leg != null && provider.Leg.PriceListId == lastActivePriceList.Id)
            .Where(provider =>
                (string.IsNullOrEmpty(searchModel.From) || provider.Leg!.RouteInfo!.FromLocation!.Name.ToUpper()
                    .Contains(searchModel.From.ToUpper())) &&
                (string.IsNullOrEmpty(searchModel.To) || provider.Leg!.RouteInfo!.ToLocation!.Name.ToUpper()
                    .Contains(searchModel.To.ToUpper())) &&
                (string.IsNullOrEmpty(searchModel.Company) ||
                 provider.Company!.Name.ToUpper().Contains(searchModel.Company.ToUpper()))
            );
    }

    private List<ProviderReport> MapProvidersToReports(List<Provider> providers)
    {
        return providers.Select(provider => new ProviderReport
        {
            Id = provider.Id,
            LegId = provider.LegId,
            Price = provider.Price,
            FlightStart = provider.FlightStart,
            FlightEnd = provider.FlightEnd,
            CompanyName = provider.Company!.Name,
            FromLocation = provider.Leg!.RouteInfo!.FromLocation!.Name,
            ToLocation = provider.Leg!.RouteInfo!.ToLocation!.Name,
            Distance = provider.Leg!.RouteInfo!.Distance
        }).ToList();
    }

    public async Task<List<ProviderReport>?> SortProviderReportsResult(string orderBy,
        List<ProviderReport>? routesResults)
    {
        if (routesResults != null)
        {
            routesResults = orderBy switch
            {
                "price_desc" => routesResults.OrderByDescending(s => s.Price).ToList(),
                "price_asc" => routesResults.OrderBy(s => s.Price).ToList(),
                "distance_desc" => routesResults.OrderByDescending(s => s.Distance).ToList(),
                "distance_asc" => routesResults.OrderBy(s => s.Distance).ToList(),
                "travel_time_desc" => routesResults.OrderByDescending(s => s.FlightEnd - s.FlightStart).ToList(),
                "travel_time_asc" => routesResults.OrderBy(s => s.FlightEnd - s.FlightStart).ToList(),
                _ => routesResults.OrderBy(s => s.FromLocation).ToList(),
            };
        }

        return routesResults;
    }


    public async Task<Provider?> getProviderById(Guid? providerReportId)
    {
        return await _ctx.Providers.Where(e => e.Id.Equals(providerReportId))
            .Include(e => e.Leg)
            .ThenInclude(e => e!.RouteInfo)
            .ThenInclude(e => e!.FromLocation)
            .Include(e => e.Leg)
            .ThenInclude(e => e!.RouteInfo)
            .ThenInclude(e => e!.ToLocation)
            .FirstOrDefaultAsync();
    }

    public static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}h:{1:D2}m", timeSpan.Hours, timeSpan.Minutes);
    }
}