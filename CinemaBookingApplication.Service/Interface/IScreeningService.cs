// CinemaBookingApplication.Service/Interface/IScreeningService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IScreeningService
{
    List<Screening> AllWithIncludes();
    Screening? Get(Guid id);
    Screening Create(Screening s);
    Screening Update(Screening s);
    void Delete(Guid id);

    int AvailableSeats(Guid screeningId); // optional helper
}
