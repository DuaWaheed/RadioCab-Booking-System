using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using E_project.Models;
using E_project.Services;

public class DriverController : Controller
{
    private readonly mycontext _context;
    private readonly IWebHostEnvironment _env;
    private readonly EmailService _emailService;

    public DriverController(mycontext context, IWebHostEnvironment env, EmailService emailService)
    {
        _context = context;
        _env = env;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        var driverId = HttpContext.Session.GetString("driver_session");
        if (string.IsNullOrEmpty(driverId))
        {
            return RedirectToAction("LoginDriver");
        }

        var row = _context.Drivers.FirstOrDefault(c => c.DriverId == int.Parse(driverId));
        return View(row);
    }

    [HttpGet]
    public IActionResult LoginDriver()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LoginDriver(string email, string password)
    {
        var driver = _context.Drivers.FirstOrDefault(d => d.Email == email);

        if (driver == null)
        {
            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        var hasher = new PasswordHasher<Driver>();
        var result = hasher.VerifyHashedPassword(driver, driver.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        if (driver.ActivityState == "Disable")
        {
            ViewBag.Error = "Your account is disabled. Please contact support.";
            return View();
        }

        HttpContext.Session.SetString("driver_session", driver.DriverId.ToString());
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult RegisterDriver()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterDriver(Driver driver)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(driver);
        }

        if (_context.Drivers.Any(d => d.Email == driver.Email))
        {
            ModelState.AddModelError("Email", "Email already exists.");
            return View(driver);
        }

        var hasher = new PasswordHasher<Driver>();
        driver.Password = hasher.HashPassword(driver, driver.Password);

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        var subject = "Welcome to radiocab.pk!";
        var message = $@"
            Hello {driver.DriverName},<br><br>
            Thank you for registering as a driver.<br><br>
            To activate your account, please login here:<br>
            <a href='https://localhost:7054/Home/LoginDriver'>https://localhost:7054/Home/LoginDriver</a><br><br>
            Regards,<br>
            <a href='https://localhost:7054/'>RadioCabs.pk</a> Team
        ";

        await _emailService.SendEmailAsync(driver.Email, subject, message);

        return RedirectToAction("RegistrationSuccess");
    }

    public IActionResult RegisterationSuccess()
    {
        return View();
    }
    public IActionResult LogoutDriver()
    {
        HttpContext.Session.Remove("driver_session");
        return RedirectToAction("LoginDriver");
    }

    [HttpGet]
    public IActionResult DriverList()
    {
        var drivers = _context.Drivers.ToList();
        return View(drivers);
    }

    public IActionResult Profile()
    {
        var driverId = HttpContext.Session.GetString("driver_session");
        if (string.IsNullOrEmpty(driverId))
        {
            return RedirectToAction("LoginDriver");
        }

        var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == int.Parse(driverId));
        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        return View(driver);
    }
}
