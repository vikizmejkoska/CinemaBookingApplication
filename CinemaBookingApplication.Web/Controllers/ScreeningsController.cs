// CinemaBookingApplication.Web/Controllers/ScreeningsController.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaBookingApplication.Web.Controllers;

public class ScreeningsController : Controller
{
    private readonly IScreeningService _screenings;
    private readonly IHallService _halls;
    private readonly IMovieService _movies;

    public ScreeningsController(IScreeningService screenings, IHallService halls, IMovieService movies)
    {
        _screenings = screenings;
        _halls = halls;
        _movies = movies;
    }

    public IActionResult Index() => View(_screenings.AllWithIncludes());

    public IActionResult Details(Guid? id)
    {
        if (id is null) return NotFound();
        var s = _screenings.Get(id.Value);
        if (s is null) return NotFound();
        return View(s);
    }

    public IActionResult Create()
    {
        ViewData["HallId"] = new SelectList(_halls.All(), "Id", "Name");
        ViewData["MovieId"] = new SelectList(_movies.All(), "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("MovieId,HallId,StartTime,Price,Id")] Screening screening)
    {
        if (!ModelState.IsValid)
        {
            ViewData["HallId"] = new SelectList(_halls.All(), "Id", "Name", screening.HallId);
            ViewData["MovieId"] = new SelectList(_movies.All(), "Id", "Title", screening.MovieId);
            return View(screening);
        }
        screening.Id = Guid.NewGuid();
        _screenings.Create(screening);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var s = _screenings.Get(id.Value);
        if (s is null) return NotFound();

        ViewData["HallId"] = new SelectList(_halls.All(), "Id", "Name", s.HallId);
        ViewData["MovieId"] = new SelectList(_movies.All(), "Id", "Title", s.MovieId);
        return View(s);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, [Bind("MovieId,HallId,StartTime,Price,Id")] Screening screening)
    {
        if (id != screening.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewData["HallId"] = new SelectList(_halls.All(), "Id", "Name", screening.HallId);
            ViewData["MovieId"] = new SelectList(_movies.All(), "Id", "Title", screening.MovieId);
            return View(screening);
        }
        _screenings.Update(screening);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var s = _screenings.Get(id.Value);
        if (s is null) return NotFound();
        return View(s);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _screenings.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
