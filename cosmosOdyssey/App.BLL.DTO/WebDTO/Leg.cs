﻿using App.BLL.DTO.WebDTO;

namespace App.BLL.WebDTO;

public class Leg
{
    public Guid Id { get; set; }
    public RouteInfo RouteInfo { get; set; } = default!;
    public ICollection<Provider> Providers { get; set; } = default!;
}