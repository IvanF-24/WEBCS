using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentApp.Controllers
{
    [Authorize] // Protegemos el dashboard
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}