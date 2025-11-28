using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IJsonStore _store;

        public EnrollmentController(IJsonStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? studentId)
        {
            var students = await _store.GetStudentsAsync();
            var courses = await _store.GetCoursesAsync();
            var schedules = await _store.GetSchedulesAsync(); // <-- Traemos los horarios también

            // 1. AVISO EN LUGAR DE REDIRECCIÓN:
            // Si faltan datos, solo avisamos con un mensaje en la vista, pero no te expulsamos.
            if (!students.Any()) ViewBag.Warning = "⚠ No hay estudiantes creados. Ve a 'Estudiantes' y crea uno primero.";
            if (!courses.Any()) ViewBag.Warning = "⚠ No hay cursos creados. Ve a 'Cursos' y crea uno primero.";

            // 2. LISTA DE ESTUDIANTES (Nombre + Apellido)
            var studentList = students.Select(s => new {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}"
            });

            // 3. LISTA DE CURSOS CON HORARIO (¡Lo que pediste!)
            // Unimos el Curso con su Horario para mostrarlo en el dropdown
            var courseList = courses.Select(c => {
                // Buscamos si este curso tiene horario asignado
                var schedule = schedules.FirstOrDefault(s => s.CourseId == c.Id);
                
                // Formateamos el texto: "NombreCurso (Día HoraInicio - HoraFin)"
                var scheduleText = schedule != null 
                    ? $"({schedule.Day} {schedule.Start} - {schedule.End})" 
                    : "(Sin horario asignado)";
                
                return new {
                    Id = c.Id,
                    DisplayText = $"{c.Name} {scheduleText}"
                };
            });

            ViewBag.StudentId = new SelectList(studentList, "Id", "FullName", studentId);
            ViewBag.Courses = new SelectList(courseList, "Id", "DisplayText");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid StudentId, Guid CourseId)
        {
            var students = await _store.GetStudentsAsync();
            var student = students.FirstOrDefault(s => s.Id == StudentId);

            if (student != null)
            {
                if (student.EnrolledCourseIds == null) student.EnrolledCourseIds = new List<Guid>();

                // Evitar duplicados
                if (!student.EnrolledCourseIds.Contains(CourseId))
                {
                    student.EnrolledCourseIds.Add(CourseId);
                    await _store.UpdateStudentAsync(student);
                    
                    TempData["Success"] = "¡Matrícula exitosa!";
                    return RedirectToAction("Index", "Students");
                }
                else
                {
                    TempData["Error"] = "El estudiante ya está inscrito en este curso.";
                }
            }
            
            // Si algo falla, recargamos la vista (Reutilizamos la lógica del GET)
            return await Create(StudentId); 
        }
    }
}