// CinemaBookingApplication.Service/Interface/IReservationService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IReservationService
{
    List<Reservation> AllWithIncludes();
    Reservation? Get(Guid id);
    Reservation Create(Reservation r);
    Reservation Update(Reservation r);
    void Delete(Guid id);
}
