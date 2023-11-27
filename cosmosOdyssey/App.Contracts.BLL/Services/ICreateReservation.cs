using App.BLL.DTO;

namespace App.Contracts.BLL.Services;

public class ICreateReservation
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public ProviderReport ProviderReport { get; set; } = default!;
}