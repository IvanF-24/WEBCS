using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly IJsonStore _store;
        public SchedulesController(IJsonStore store) => _store = store;

        public async Task<IActionResult> Index()
        {
            var list = await _store.GetSchedulesAsync();
            var courses = (await _store.GetCoursesAsync()).ToDictionary(c => c.Id, c => c.Name);
            ViewBag.Courses = courses;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _store.GetCoursesAsync();
            return View(new Schedule { Start = new TimeSpan(8,0,0), End = new TimeSpan(10,0,0), Day = "Lunes" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _store.GetCoursesAsync();
                return View(model);
            }
            await _store.SaveScheduleAsync(model);
            TempData["Success"] = "Horario creado.";
            return RedirectToAction("Index");
        }
    }
}