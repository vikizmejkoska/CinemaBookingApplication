// CinemaBookingApplication.Web/Controllers/HallsController.cs
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingApplication.Web.Controllers;

public class HallsController : Controller
{
    private readonly IHallService _halls;

    public HallsController(IHallService halls) => _halls = halls;

    public IActionResult Index() => View(_halls.All());

    public IActionResult Details(Guid? id)
    {
        if (id is null) return NotFound();
        var h = _halls.Get(id.Value);
        if (h is null) return NotFound();
        return View(h);
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Name,Capacity,Id")] Hall hall)
    {
        if (!ModelState.IsValid) return View(hall);
        hall.Id = Guid.NewGuid();
        _halls.Create(hall);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Guid? id)
    {
        if (id is null) return NotFound();
        var h = _halls.Get(id.Value);
        if (h is null) return NotFound();
        return View(h);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(Guid id, [Bind("Name,Capacity,Id")] Hall hall)
    {
        if (id != hall.Id) return NotFound();
        if (!ModelState.IsValid) return View(hall);
        _halls.Update(hall);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid? id)
    {
        if (id is null) return NotFound();
        var h = _halls.Get(id.Value);
        if (h is null) return NotFound();
        return View(h);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _halls.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
