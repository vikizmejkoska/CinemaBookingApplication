// CinemaBookingApplication.Service/Implementation/ReservationService.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingApplication.Service.Implementation;

public class ReservationService : IReservationService
{
    private readonly IRepository<Reservation> _repo;

    public ReservationService(IRepository<Reservation> repo) => _repo = repo;

    public List<Reservation> AllWithIncludes() =>
        _repo.GetAll(selector: x => x,
            include: q => q.Include(r => r.Screening)
                           .ThenInclude(s => s.Movie)
                           .Include(r => r.Screening)
                           .ThenInclude(s => s.Hall)).ToList();

    public Reservation? Get(Guid id) =>
        _repo.Get(selector: x => x, predicate: x => x.Id == id,
            include: q => q.Include(r => r.Screening)
                           .ThenInclude(s => s.Movie)
                           .Include(r => r.Screening)
                           .ThenInclude(s => s.Hall));

    public Reservation Create(Reservation r) => _repo.Insert(r);

    public Reservation Update(Reservation r) => _repo.Update(r);

    public void Delete(Guid id)
    {
        var e = _repo.Get(selector: x => x, predicate: x => x.Id == id);
        if (e != null) _repo.Delete(e);
    }
}
