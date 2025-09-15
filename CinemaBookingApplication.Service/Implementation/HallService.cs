// CinemaBookingApplication.Service/Implementation/HallService.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Interface;

namespace CinemaBookingApplication.Service.Implementation;

public class HallService : IHallService
{
    private readonly IRepository<Hall> _repo;
    public HallService(IRepository<Hall> repo) => _repo = repo;

    public List<Hall> All() => _repo.GetAll(selector: x => x).ToList();

    public Hall? Get(Guid id) => _repo.Get(selector: x => x, predicate: x => x.Id == id);

    public Hall Create(Hall h) => _repo.Insert(h);

    public Hall Update(Hall h) => _repo.Update(h);

    public void Delete(Guid id)
    {
        var e = Get(id);
        if (e != null) _repo.Delete(e);
    }
}
