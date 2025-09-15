// CinemaBookingApplication.Service/Interface/IDataFetchService.cs
using CinemaBookingApplication.Domain.DomainModels;

namespace CinemaBookingApplication.Service.Interface;

public interface IDataFetchService
{
    Task<int> ImportMoviesFromTmdbAsync(string query = "popular");
}
