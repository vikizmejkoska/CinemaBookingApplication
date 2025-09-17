// CinemaBookingApplication.Web/Controllers/ReservationsController.cs
using System.Security.Claims;
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Domain.Dtos;
using CinemaBookingApplication.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CinemaBookingApplication.Web.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly IReservationService _reservations;
    private readonly IScreeningService _screenings;

    public ReservationsController(IReservationService reservations, IScreeningService screenings)
    {
        _reservations = reservations;
        _screenings = screenings;
    }

    // GET: Reservations
    public IActionResult Index()
    {
        return View(_reservations.All());
    }

    // GET: Reservations/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // dropdown со сите проекции: "MovieTitle (Hall) - StartTime - Price"
        var all = _screenings.AllWithIncludes(); // претпоставувам дека постои во твојот сервис
        ViewBag.ScreeningId = new SelectList(
            all.Select(s => new
            {
                s.Id,
                Text = $"{s.Movie.Title} / {s.Hall.Name} / {s.StartTime:g} / {s.Price:C}"
            }),
            "Id", "Text");

        return View(new CreateReservationViewModel());
    }

    // POST: Reservations/Create
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateReservationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ReloadScreeningsDropDown(model.ScreeningId);
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            _reservations.Create(userId, model.ScreeningId, model.Quantity);
            TempData["Toast"] = "Reservation created.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ReloadScreeningsDropDown(model.ScreeningId);
            return View(model);
        }
    }

    // GET: Reservations/Details/5
    public IActionResult Details(Guid? id)
    {
        if (id is null) return NotFound();
        var r = _reservations.Get(id.Value);
        if (r is null) return NotFound();
        return View(r);
    }

    // GET: Reservations/Delete/5
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var r = _reservations.Get(id.Value);
        if (r is null) return NotFound();
        return View(r);
    }

    // POST: Reservations/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _reservations.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    private void ReloadScreeningsDropDown(Guid? selectedId = null)
    {
        var all = _screenings.AllWithIncludes();
        ViewBag.ScreeningId = new SelectList(
            all.Select(s => new
            {
                s.Id,
                Text = $"{s.Movie.Title} / {s.Hall.Name} / {s.StartTime:g} / {s.Price:C}"
            }),
            "Id", "Text", selectedId);
    }



[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Confirm(Guid id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var isAdmin = User.IsInRole("Admin");
    try
    {
        _reservations.Confirm(id, userId, isAdmin);
        TempData["Toast"] = "Reservation confirmed.";
    }
    catch (Exception ex)
    {
        TempData["Toast"] = ex.Message;
    }
    return RedirectToAction(nameof(Index));
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Cancel(Guid id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var isAdmin = User.IsInRole("Admin");
    try
    {
        _reservations.Cancel(id, userId, isAdmin);
        TempData["Toast"] = "Reservation cancelled.";
    }
    catch (Exception ex)
    {
        TempData["Toast"] = ex.Message;
    }
    return RedirectToAction(nameof(Index));
}

// админ-utility (пример: истечи pending постари од 15 минути)
[HttpPost]
[Authorize(Roles = "Admin")]
[ValidateAntiForgeryToken]
public IActionResult ExpireStale()
{
    var n = _reservations.ExpireOlderPending(TimeSpan.FromMinutes(15));
    TempData["Toast"] = $"Expired {n} old pending reservations.";
    return RedirectToAction(nameof(Index));
}

}
