using App.BLL.DTO.WebDTO;
using App.BLL.WebDTO;
using App.Contracts.DAL;
using Base.Contracts.DAL;


namespace App.Contracts.BLL.Services;

public interface IPriceListService 
{
    public Task<PriceList?> GetPriceListAsync();
}