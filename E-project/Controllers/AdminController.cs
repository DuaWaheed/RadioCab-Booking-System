using Microsoft.AspNetCore.Mvc;

namespace E_project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
