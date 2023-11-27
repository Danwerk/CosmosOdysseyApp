using Base.DAL;
using AutoMapper;

namespace App.BLL.Mappers;

public class ReservationMapper : BaseMapper<BLL.DTO.Reservation, App.Domain.Reservation>
{
    public ReservationMapper(IMapper mapper) : base(mapper)
    {
    }
}