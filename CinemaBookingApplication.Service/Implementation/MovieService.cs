// CinemaBookingApplication.Service/Implementation/MovieService.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Interface;

namespace CinemaBookingApplication.Service.Implementation;

public class MovieService : IMovieService
{
    private readonly IRepository<Movie> _repo;

    public MovieService(IRepository<Movie> repo) => _repo = repo;

    public List<Movie> All()
        => _repo.GetAll(selector: x => x).ToList();

    public Movie? Get(Guid id)
        => _repo.Get(selector: x => x, predicate: x => x.Id == id);

    public Movie Create(Movie m) => _repo.Insert(m);

    public Movie Update(Movie m) => _repo.Update(m);

    public void Delete(Guid id)
    {
        var e = Get(id);
        if (e != null) _repo.Delete(e);
    }

    public void InsertMany(ICollection<Movie> movies)
        => _repo.InsertMany(movies);
}
