// CinemaBookingApplication.Service/Implementation/ScreeningService.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingApplication.Service.Implementation;

public class ScreeningService : IScreeningService
{
    private readonly IRepository<Screening> _screenings;
    private readonly IRepository<Reservation> _reservations;

    public ScreeningService(IRepository<Screening> screenings, IRepository<Reservation> reservations)
    {
        _screenings = screenings;
        _reservations = reservations;
    }

    public List<Screening> AllWithIncludes() =>
        _screenings.GetAll(selector: x => x,
            include: q => q.Include(s => s.Movie)
                           .Include(s => s.Hall)).ToList();

    public Screening? Get(Guid id) =>
        _screenings.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: q => q.Include(s => s.Movie)
                           .Include(s => s.Hall));

    public Screening Create(Screening s) => _screenings.Insert(s);

    public Screening Update(Screening s) => _screenings.Update(s);

    public void Delete(Guid id)
    {
        var e = _screenings.Get(selector: x => x, predicate: x => x.Id == id);
        if (e != null) _screenings.Delete(e);
    }

    public int AvailableSeats(Guid screeningId)
    {
        var s = Get(screeningId) ?? throw new Exception("Screening not found");

        // брои само Pending + Confirmed (исто како во ReservationService)
        var taken = _reservations.GetAll(x => x,
                       predicate: r => r.ScreeningId == screeningId
                                    && (r.Status == ReservationStatus.Pending
                                        || r.Status == ReservationStatus.Confirmed))
                                 .Sum(r => r.Quantity);

        return Math.Max(0, (s.Hall?.Capacity ?? 0) - taken);
    }


}
