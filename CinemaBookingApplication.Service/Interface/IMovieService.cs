// CinemaBookingApplication.Service/Interface/IMovieService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IMovieService
{
    List<Movie> All();
    Movie? Get(Guid id);
    Movie Create(Movie m);
    Movie Update(Movie m);
    void Delete(Guid id);
    void InsertMany(ICollection<Movie> movies);

}
