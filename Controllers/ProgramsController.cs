using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    public class ProgramsController : Controller
    {
        private readonly IJsonStore _store;

        public ProgramsController(IJsonStore store)
        {
            _store = store;
        }

        // Listar Programas
        public async Task<IActionResult> Index()
        {
            var programs = await _store.GetProgramsAsync();
            return View(programs);
        }

        // Formulario de Creación
        public IActionResult Create()
        {
            return View();
        }

        // Guardar Programa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicProgram model) // <--- AQUÍ ESTABA EL ERROR (Debe ser AcademicProgram)
        {
            if (ModelState.IsValid)
            {
                await _store.SaveProgramAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}