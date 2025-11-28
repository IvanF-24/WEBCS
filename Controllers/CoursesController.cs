using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IJsonStore _store;
        public CoursesController(IJsonStore store) => _store = store;

        public async Task<IActionResult> Index()
        {
            var list = await _store.GetCoursesAsync();
            var programs = (await _store.GetProgramsAsync()).ToDictionary(p => p.Id, p => p.Name);
            ViewBag.Programs = programs;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Programs = await _store.GetProgramsAsync();
            return View(new Course());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = await _store.GetProgramsAsync();
                return View(model);
            }
            await _store.SaveCourseAsync(model);
            TempData["Success"] = "Curso creado.";
            return RedirectToAction("Index");
        }
    }
}