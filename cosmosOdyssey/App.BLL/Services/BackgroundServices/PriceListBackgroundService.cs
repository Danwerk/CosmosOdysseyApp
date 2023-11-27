using App.BLL.DTO.WebDTO;
using DAL.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.BLL.Services.BackgroundServices;

public class PriceListBackgroundService : TimedBackgroundService
{
    private readonly IServiceProvider _services;
    
    public PriceListBackgroundService(ILogger<PriceListBackgroundService> logger, IServiceProvider services) 
        : base(logger, services, TimeSpan.FromMinutes(1))
    {
        _services = services;
    }

    protected override async Task DoWork(object? state)
    {
        await using var scope = _services.CreateAsyncScope();
        var priceListService = scope.ServiceProvider.GetRequiredService<PriceListService>();

        PriceList? priceList = null;
        try
        {
            Logger.LogInformation("Fetching data from the API...");

            // Call the PriceListService to fetch data from the API
            priceList = await priceListService.GetPriceListAsync();

            // Now 'priceList' holds the actual response
            if (priceList != null)
            {
                // Logger.LogInformation($"API response: {JsonSerializer.Serialize(priceList)}");
            }
            else
            {
                Logger.LogWarning("API request failed or response is null.");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while fetching data from the API: {ex.Message}");
        }
        
       await priceListService.AddPriceList(priceList);
       await priceListService.Filter15LastActivePriceLists();
       await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().SaveChangesAsync();
    }
}