using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using E_project.Models;
using Microsoft.EntityFrameworkCore;

namespace E_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly mycontext _context;
        private readonly ILogger<HomeController> _logger;

        // Constructor: Dono _context aur _logger inject karo
        public HomeController(mycontext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var feedbacks = _context.Feedbacks.ToList(); // Get all feedbacks from DB
            return View(feedbacks); // Pass to view
        }

        public IActionResult TopCabCompanies()
        {
            var ads = _context.Advertisers.ToList(); // Retrieve all advertisements from the database
            return View("TopCabCompanies", ads);

        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        // --- Driver Login (GET)
        public IActionResult LoginDriver()
        {
            return View();
        }

        // --- Driver Register (GET)
        public IActionResult RegisterDriver()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult OurSocialImpact()
        {
            return View();
        }
        public IActionResult WhyChooseUs()
        {
            return View();
        }
        public IActionResult AboutUS()
        {
            return View();
        }
    }
}
