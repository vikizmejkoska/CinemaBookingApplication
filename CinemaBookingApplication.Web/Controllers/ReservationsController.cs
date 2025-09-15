// CinemaBookingApplication.Web/Controllers/ReservationsController.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaBookingApplication.Web.Controllers;

public class ReservationsController : Controller
{
    private readonly IReservationService _reservations;
    private readonly IScreeningService _screenings;

    public ReservationsController(IReservationService reservations, IScreeningService screenings)
    {
        _reservations = reservations;
        _screenings = screenings;
    }

    public IActionResult Index() => View(_reservations.AllWithIncludes());

    public IActionResult Details(Guid? id)
    {
        if (id is null) return NotFound();
        var r = _reservations.Get(id.Value);
        if (r is null) return NotFound();
        return View(r);
    }

    public IActionResult Create()
    {
        ViewData["ScreeningId"] = new SelectList(_screenings.AllWithIncludes(), "Id", "Id");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("ScreeningId,UserId,Quantity,TotalPrice,CreatedAt,Status,Id")] Reservation reservation)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ScreeningId"] = new SelectList(_screenings.AllWithIncludes(), "Id", "Id", reservation.ScreeningId);
            return View(reservation);
        }
        reservation.Id = Guid.NewGuid();
        _reservations.Create(reservation);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var r = _reservations.Get(id.Value);
        if (r is null) return NotFound();
        ViewData["ScreeningId"] = new SelectList(_screenings.AllWithIncludes(), "Id", "Id", r.ScreeningId);
        return View(r);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, [Bind("ScreeningId,UserId,Quantity,TotalPrice,CreatedAt,Status,Id")] Reservation reservation)
    {
        if (id != reservation.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewData["ScreeningId"] = new SelectList(_screenings.AllWithIncludes(), "Id", "Id", reservation.ScreeningId);
            return View(reservation);
        }
        _reservations.Update(reservation);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var r = _reservations.Get(id.Value);
        if (r is null) return NotFound();
        return View(r);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _reservations.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
