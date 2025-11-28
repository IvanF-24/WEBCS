using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IJsonStore _store;

        public StudentsController(IJsonStore store) => _store = store;

        public async Task<IActionResult> Index()
        {
            var students = await _store.GetStudentsAsync();
            return View(students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var s = await _store.GetStudentAsync(id);
            if (s == null) return NotFound();
            var programs = (await _store.GetProgramsAsync()).ToDictionary(p => p.Id, p => p.Name);
            var courses = (await _store.GetCoursesAsync()).ToDictionary(c => c.Id, c => c.Name);
            ViewBag.Programs = programs;
            ViewBag.Courses = courses;
            return View(s);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Programs = await _store.GetProgramsAsync();
            ViewBag.Courses = await _store.GetCoursesAsync();
            return View(new Student { DateOfBirth = DateTime.UtcNow.AddYears(-18) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model, Guid[]? selectedCourses)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = await _store.GetProgramsAsync();
                ViewBag.Courses = await _store.GetCoursesAsync();
                return View(model);
            }

            model.EnrolledCourseIds = selectedCourses?.ToList() ?? new List<Guid>();
            model.CreatedBy = User.Identity?.Name;
            await _store.SaveStudentAsync(model);
            TempData["Success"] = "Estudiante creado correctamente.";
            return RedirectToAction("Index");
        }
    }
}