using Microsoft.AspNetCore.Identity;  // Required for PasswordHasher
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using E_project.Models;
using E_project.Services;
using System.ComponentModel.Design;

public class AdvertiserController : Controller
{
    private mycontext _context;
    private IWebHostEnvironment _env;
    private EmailService _emailService;

    public AdvertiserController(mycontext context, IWebHostEnvironment env, EmailService emailService)
    {
        _context = context;
        _env = env;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        var advertiserId = HttpContext.Session.GetString("advertiser_session");
        var row = _context.Advertisers.FirstOrDefault(c => c.AdvertiserId == int.Parse(advertiserId));

        if (advertiserId != null)
        {
            return View(row);
        }
        else
        {
            return RedirectToAction("LoginAdvertiser");
        }
    }

    [HttpGet]
    public IActionResult LoginAdvertiser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LoginAdvertiser(string companyname, string password, string activitystate)
    {
        Console.WriteLine("Login attempt for Advertiser: " + companyname);

        // Try to find the advertiser in the database
        // Find company by username only (don't compare password here!)
        var advertiser = _context.Advertisers
            .FirstOrDefault(c => c.CompanyName == companyname);
        if (advertiser == null)
        {
            Console.WriteLine("Invalid login attempt for: " + companyname);
            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // Password verification using PasswordHasher
        var hasher = new PasswordHasher<Advertiser>();
        var result = hasher.VerifyHashedPassword(advertiser, advertiser.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // Check if the account is disabled
        if (advertiser.ActivityState == "Disable")
        {
            Console.WriteLine("Account disabled for: " + companyname);
            ViewBag.Error = "Your account is disabled due to unpaid fees or suspicious activity. Please contact support.";
            return View();
        }

        HttpContext.Session.SetString("advertiser_session", advertiser.AdvertiserId.ToString());
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult RegisterAdvertiser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAdvertiser(Advertiser advertiser)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(advertiser);
        }

        // Check if email already exists before saving
        if (_context.Advertisers.Any(a => a.Email == advertiser.Email))
        {
            ModelState.AddModelError("Email", "Email already exists.");
            return View(advertiser);
        }

        // Check if company name already exists before saving
        if (_context.Advertisers.Any(a => a.CompanyName == advertiser.CompanyName))
        {
            ModelState.AddModelError("CompanyName", "Company name already exists.");
            return View(advertiser);
        }

        // Password hashing
        var hasher = new PasswordHasher<Advertiser>();
        advertiser.Password = hasher.HashPassword(advertiser, advertiser.Password);

        _context.Advertisers.Add(advertiser);
        await _context.SaveChangesAsync();

        var subject = "Welcome to radiocab.pk!";
        var message = $@"
        Hello {advertiser.CompanyName},<br><br>
        Thanks for registering with us.<br><br>
        To activate your account, please click on the link below and log in:<br>
        <a href='https://localhost:7054/Advertiser/LoginAdvertiser'>https://localhost:7054/Advertiser/LoginAdvertiser</a><br><br>
        Best regards,<br>
        The <a href='https://localhost:7054/'>radiocab.pk </a> Team
    ";

        await _emailService.SendEmailAsync(advertiser.Email, subject, message);

        return RedirectToAction("RegisterationSuccess");
    }

    public IActionResult RegisterationSuccess()
    {
        return View();
    }
    public IActionResult LogoutAdvertiser()
    {
        HttpContext.Session.Remove("advertiser_session");
        return RedirectToAction("LoginAdvertiser");
    }


    public IActionResult Advertise()
    {
        var ads = _context.Advertisers
            .Where(a => a.Status == "Paid" && a.ActivityState == "Active")
            .ToList();

        return View(ads);
    }

}
