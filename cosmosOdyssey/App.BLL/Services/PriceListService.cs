using System.Text.Json;
using App.Contracts.BLL.Services;
using App.Domain;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PriceList = App.BLL.DTO.WebDTO.PriceList;
using Provider = App.BLL.DTO.WebDTO.Provider;

namespace App.BLL.Services;

public class PriceListService : IPriceListService
{
    private readonly string apiUrl = "https://cosmos-odyssey.azurewebsites.net/api/v1.0/TravelPrices";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly ApplicationDbContext _ctx;
    private readonly ILogger<PriceListService> _logger;

    public PriceListService(ApplicationDbContext ctx, ILogger<PriceListService> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<PriceList?> GetPriceListAsync()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetStreamAsync(apiUrl);
        var priceList = await JsonSerializer.DeserializeAsync<PriceList>(response, Options);

        if (priceList == null)
        {
            throw new ApplicationException("Failed to fetch price list");
        }

        if (priceList.ValidUntil <= DateTime.UtcNow)
        {
            throw new ApplicationException(
                $"Fetched price list was expired! {nameof(priceList.ValidUntil)}: {priceList.ValidUntil}, current time: {DateTime.UtcNow}");
        }

        return priceList;
    }

    public async Task AddPriceList(PriceList priceListDto)
    {
        var priceListFromDb = await _ctx.PriceLists.FindAsync(priceListDto.Id);

        if (priceListFromDb != null && priceListFromDb.ValidUntil > DateTime.UtcNow)
        {
            _logger.LogInformation(
                $"PriceList with ID {priceListDto.Id} already exists in the database, and is valid.");
            return;
        }

        var priceListId = priceListDto.Id;
        var priceListValidUntil = priceListDto.ValidUntil;


        var priceListToSave = new Domain.PriceList()
        {
            Id = priceListId,
            ValidUntil = priceListValidUntil
        };
        _ctx.PriceLists.Add(priceListToSave);


        await AddLegs(priceListDto, priceListToSave);
        await _ctx.SaveChangesAsync();
    }


    private async Task AddLegs(PriceList priceListDto, Domain.PriceList priceList)
    {
        foreach (var legDto in priceListDto.Legs)
        {
            var routeInfoDto = legDto.RouteInfo;

            var routeInfoToSave = new Domain.RouteInfo()
            {
                Id = routeInfoDto.Id,
                FromLocation = new FromLocation { Id = routeInfoDto.From.Id, Name = routeInfoDto.From.Name },
                ToLocation = new ToLocation { Id = routeInfoDto.To.Id, Name = routeInfoDto.To.Name },
                Distance = routeInfoDto.Distance
            };

            var routeInfoFromDb = await _ctx.RouteInfos.FindAsync(routeInfoDto.Id);

            if (routeInfoFromDb == null)
            {
                _ctx.RouteInfos.Add(routeInfoToSave);
            }

            var legToSave = new Domain.Leg()
            {
                Id = legDto.Id,
                PriceList = priceList,
                RouteInfo = routeInfoToSave
            };

            _ctx.Legs.Add(legToSave);
            await AddProviders(legDto.Providers.ToList(), legToSave);
        }
    }

    private async Task AddProviders(List<Provider> providerDtos, Domain.Leg leg)
    {
        foreach (var providerDto in providerDtos)
        {
            var companyId = providerDto.Company.Id;
            var existingCompany = _ctx.Companies.Local.FirstOrDefault(c => c.Id == companyId);
            
            var providerToSave = new Domain.Provider()
            {
                Id = providerDto.Id,
                Company = existingCompany ?? new Company { Id = providerDto.Company.Id, Name = providerDto.Company.Name },
                FlightStart = providerDto.FlightStart,
                FlightEnd = providerDto.FlightEnd,
                Leg = leg,
                Price = providerDto.Price
            };
            _ctx.Providers.Add(providerToSave);
        }

        await _ctx.SaveChangesAsync();
    }

    
    public async Task Filter15LastActivePriceLists()
    {
        try
        {
            var pricelistsToDelete = await _ctx.PriceLists
                .Where(p => p.ValidUntil < DateTime.UtcNow)
                .OrderByDescending(p => p.ValidUntil)
                .Skip(15)
                .ToListAsync();
            
            if (pricelistsToDelete.Count > 0)
            {
                
                await _ctx.FromLocations
                    .Where(location => !_ctx.Legs
                        .Any(leg => leg.RouteInfo!.FromLocationId == location.Id))
                    .ExecuteDeleteAsync();
            
                await _ctx.ToLocations
                    .Where(location => !_ctx.Legs
                        .Any(leg => leg.RouteInfo!.ToLocationId == location.Id))
                    .ExecuteDeleteAsync();
                
                // await _ctx.Companies
                //     .Where(c => _ctx.Providers
                //         .Any(p => p.CompanyId == c.Id))
                //     .ExecuteDeleteAsync();
                
                
                // Delete the selected price lists
                _ctx.PriceLists.RemoveRange(pricelistsToDelete);

                // Save changes to the database
                await _ctx.SaveChangesAsync();
                _logger.LogInformation($"{pricelistsToDelete.Count} pricelists deleted successfully.");
            }
            else
            {
                _logger.LogInformation("No pricelists found for deletion.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while deleting price lists: {e.Message}");
        }
    }
}