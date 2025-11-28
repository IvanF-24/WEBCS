using System.Security.Claims;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    [Authorize(Roles = "Student")] // ¡Solo estudiantes entran aquí!
    public class StudentPortalController : Controller
    {
        private readonly IJsonStore _store;

        public StudentPortalController(IJsonStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Averiguar quién está conectado (por su email)
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            var students = await _store.GetStudentsAsync();
            var me = students.FirstOrDefault(s => s.Email == userEmail);

            if (me == null) return RedirectToAction("Login", "Account");

            // 2. Buscar sus cursos y horarios
            var allCourses = await _store.GetCoursesAsync();
            var allSchedules = await _store.GetSchedulesAsync();
            var allPrograms = await _store.GetProgramsAsync();

            // Filtrar mis cursos
            var myCourses = allCourses.Where(c => me.EnrolledCourseIds.Contains(c.Id)).ToList();
            
            // Enviar datos a la vista usando un ViewModel dinámico o ViewBag
            ViewBag.StudentName = me.FirstName;
            ViewBag.ProgramName = allPrograms.FirstOrDefault(p => p.Id == me.ProgramId)?.Name ?? "Sin Programa";
            ViewBag.Schedules = allSchedules; // Para buscar el horario de cada curso

            return View(myCourses);
        }
    }
}