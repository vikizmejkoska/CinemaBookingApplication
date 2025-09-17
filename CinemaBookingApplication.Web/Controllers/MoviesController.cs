// CinemaBookingApplication.Web/Controllers/MoviesController.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingApplication.Web.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieService _movies;
    private readonly IDataFetchService _fetch;

    public MoviesController(IMovieService movies, IDataFetchService fetch)
    {
        _movies = movies;
        _fetch = fetch;
    }

    // GET: Movies
    public IActionResult Index()
    {
        ViewBag.Toast = TempData["Toast"] as string;
        return View(_movies.All());
    }

    // Import from TMDB (popular/top_rated or search term)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(string query = "popular")
    {
        try
        {
            var count = await _fetch.ImportMoviesFromTmdbAsync(query);
            TempData["Toast"] = count > 0
                ? $"Imported {count} movies from TMDB."
                : "No movies imported from TMDB.";
        }
        catch (Exception ex)
        {
            TempData["Toast"] = $"Import failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // Details/Create/Edit/Delete stay the same but call _movies instead of DbContext

    public IActionResult Details(Guid? id)
    {
        if (id is null) return NotFound();
        var m = _movies.Get(id.Value);
        if (m is null) return NotFound();
        return View(m);
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Title,Overview,Runtime,Rating,PosterUrl,Id")] Movie movie)
    {
        if (!ModelState.IsValid) return View(movie);
        movie.Id = Guid.NewGuid();
        _movies.Create(movie);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var m = _movies.Get(id.Value);
        if (m is null) return NotFound();
        return View(m);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, [Bind("Title,Overview,Runtime,Rating,PosterUrl,Id")] Movie movie)
    {
        if (id != movie.Id) return NotFound();
        if (!ModelState.IsValid) return View(movie);
        _movies.Update(movie);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var m = _movies.Get(id.Value);
        if (m is null) return NotFound();
        return View(m);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _movies.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
