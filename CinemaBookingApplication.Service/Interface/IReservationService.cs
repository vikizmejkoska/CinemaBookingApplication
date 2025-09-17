// CinemaBookingApplication.Service/Interface/IReservationService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IReservationService
{
    List<Reservation> All();
    Reservation? Get(Guid id);
    Reservation Create(string userId, Guid screeningId, int quantity);

   // Reservation Update(Reservation r);
    void Delete(Guid id);

    Reservation Confirm(Guid id, string currentUserId, bool isAdmin = false);
    Reservation Cancel(Guid id, string currentUserId, bool isAdmin = false);
    int ExpireOlderPending(TimeSpan ttl);
    List<Reservation> ForUser(string userId);
}
