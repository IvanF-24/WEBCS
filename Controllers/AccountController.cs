using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using EnrollmentApp.Models;
using EnrollmentApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnrollmentApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IJsonStore _store;

        public AccountController(IJsonStore store)
        {
            _store = store;
        }

        // --- LOGIN ---
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            ClaimsIdentity identity = null;
            bool isAdmin = false;

            // 1. LOGIN DE ADMINISTRADOR (Usuario: admin / Clave: admin123)
            if (model.Username == "admin" && model.Password == "admin123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Administrador"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                isAdmin = true;
            }
            // 2. LOGIN DE ESTUDIANTE (Busca en la base de datos JSON)
            else
            {
                var students = await _store.GetStudentsAsync();
                
                // Buscamos por Email o Nombre
                var student = students.FirstOrDefault(s => s.Email == model.Username || s.FirstName == model.Username);

                // Verificación de contraseña simple (igual a la guardada)
                if (student != null && model.Password == model.Password) 
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, student.FirstName),
                        new Claim(ClaimTypes.Email, student.Email),
                        new Claim(ClaimTypes.Role, "Student")
                    };
                    identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }

            if (identity != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                
                // REDIRECCIÓN INTELIGENTE SEGÚN ROL
                if (isAdmin) return RedirectToAction("Index", "Dashboard"); // Admin -> Panel de Gestión
                else return RedirectToAction("Index", "StudentPortal");     // Estudiante -> Su Portal
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(model);
        }

        // --- REGISTRO DE ESTUDIANTE ---
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Cargar la lista de programas para el menú desplegable
            ViewBag.Programs = await _store.GetProgramsAsync(); 
            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Programs = await _store.GetProgramsAsync(); // Si falla, recargar programas
                return View(model);
            }

            // Crear el objeto Estudiante con TODOS los datos del formulario
            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                ProgramId = model.ProgramId,
                
                // Datos nuevos agregados:
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                
                CreatedBy = "Self-Registration"
            };

            // Guardar en el archivo JSON
            await _store.SaveStudentAsync(student);

            // Iniciar sesión automáticamente después de registrarse
            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.Name, student.FirstName),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Role, "Student")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // Redirigir al portal del estudiante
            return RedirectToAction("Index", "StudentPortal");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // --- MODELOS DE DATOS (ViewModels) ---

        public class LoginModel
        {
            [Required(ErrorMessage = "El usuario es obligatorio")] 
            public string Username { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "La contraseña es obligatoria")] 
            [DataType(DataType.Password)] 
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio")] 
            public string FirstName { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "El apellido es obligatorio")] 
            public string LastName { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "El correo es obligatorio")] 
            [EmailAddress] 
            public string Email { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "El teléfono es obligatorio")]
            [Phone]
            public string Phone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Debes seleccionar un programa")]
            public Guid ProgramId { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")] 
            [DataType(DataType.Password)] 
            [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
