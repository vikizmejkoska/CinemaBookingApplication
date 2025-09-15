// CinemaBookingApplication.Service/Interface/IHallService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IHallService
{
    List<Hall> All();
    Hall? Get(Guid id);
    Hall Create(Hall h);
    Hall Update(Hall h);
    void Delete(Guid id);
}
