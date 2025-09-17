// CinemaBookingApplication.Service/Implementation/ReservationService.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingApplication.Service.Implementation;

public class ReservationService : IReservationService
{
    private readonly IRepository<Reservation> _reservations;
    private readonly IRepository<Screening> _screenings;

    // броиме места само за Pending + Confirmed (Cancelled/Expired не важат)
    private static readonly ReservationStatus[] CountStatuses =
        new[] { ReservationStatus.Pending, ReservationStatus.Confirmed };

    public ReservationService(
        IRepository<Reservation> reservations,
        IRepository<Screening> screenings)
    {
        _reservations = reservations;
        _screenings = screenings;
    }

    public List<Reservation> All()
        => _reservations.GetAll(
                selector: x => x,
                include: q => q.Include(r => r.Screening)
                               .ThenInclude(s => s.Movie)
                               .Include(r => r.Screening)
                               .ThenInclude(s => s.Hall))
           .ToList();

    public Reservation? Get(Guid id)
        => _reservations.Get(
            selector: x => x,
            predicate: x => x.Id == id,
            include: q => q.Include(r => r.Screening)
                           .ThenInclude(s => s.Movie)
                           .Include(r => r.Screening)
                           .ThenInclude(s => s.Hall));

    public Reservation Create(string userId, Guid screeningId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");

        var screening = _screenings.Get(
            selector: x => x,
            predicate: x => x.Id == screeningId,
            include: q => q.Include(s => s.Hall).Include(s => s.Movie))
            ?? throw new Exception("Screening not found");

        var alreadyReserved = _reservations.GetAll(
                selector: x => x,
                predicate: x => x.ScreeningId == screeningId && CountStatuses.Contains(x.Status))
            .Sum(r => r.Quantity);

        var available = screening.Hall.Capacity - alreadyReserved;
        if (quantity > available)
            throw new Exception($"Not enough seats. Available: {available}");

        var res = new Reservation
        {
            Id = Guid.NewGuid(),
            ScreeningId = screeningId,
            UserId = userId,
            Quantity = quantity,
            TotalPrice = screening.Price * quantity,
            CreatedAt = DateTime.UtcNow,
            Status = ReservationStatus.Pending
        };

        _reservations.Insert(res);
        return res;
    }

    public Reservation Confirm(Guid id, string currentUserId, bool isAdmin = false)
    {
        var r = Get(id) ?? throw new Exception("Reservation not found");
        if (r.Status != ReservationStatus.Pending)
            throw new Exception("Only pending reservations can be confirmed.");
        if (r.UserId != currentUserId && !isAdmin)
            throw new Exception("Not allowed to confirm this reservation.");

        // re-check capacity (без оваа резервација)
        var alreadyReservedOther = _reservations.GetAll(
                selector: x => x,
                predicate: x => x.ScreeningId == r.ScreeningId
                                && x.Id != r.Id
                                && CountStatuses.Contains(x.Status))
            .Sum(x => x.Quantity);

        var available = r.Screening.Hall.Capacity - alreadyReservedOther;
        if (r.Quantity > available)
            throw new Exception($"Not enough seats anymore. Available: {available}");

        r.Status = ReservationStatus.Confirmed;
        _reservations.Update(r);
        return r;
    }

    public Reservation Cancel(Guid id, string currentUserId, bool isAdmin = false)
    {
        var r = Get(id) ?? throw new Exception("Reservation not found");
        if (r.UserId != currentUserId && !isAdmin)
            throw new Exception("Not allowed to cancel this reservation.");

        if (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Expired)
            return r;

        r.Status = ReservationStatus.Cancelled;
        _reservations.Update(r);
        return r;
    }

    public int ExpireOlderPending(TimeSpan ttl)
    {
        var now = DateTime.UtcNow;
        var cutoff = now - ttl; // <-- пресметај надвор од query

        var toExpire = _reservations.GetAll(
                selector: x => x,
                predicate: x => x.Status == ReservationStatus.Pending
                                && x.CreatedAt < cutoff
            )
            .ToList();

        foreach (var r in toExpire)
        {
            r.Status = ReservationStatus.Expired;
            _reservations.Update(r);
        }

        return toExpire.Count;
    }


    public void Delete(Guid id)
    {
        var e = Get(id);
        if (e != null) _reservations.Delete(e);
    }
    public List<Reservation> ForUser(string userId)
    => _reservations.GetAll(
           selector: x => x,
           predicate: x => x.UserId == userId,
           include: q => q.Include(r => r.Screening)
                          .ThenInclude(s => s.Movie)
                          .Include(r => r.Screening)
                          .ThenInclude(s => s.Hall))
       .ToList();


}
