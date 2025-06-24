using Microsoft.AspNetCore.Identity;  // Required for PasswordHasher
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using E_project.Models; 
using E_project.Services;

public class CompanyController : Controller
{
    private mycontext _context;
    private IWebHostEnvironment _env;
 private  EmailService _emailService;

    public CompanyController(mycontext context, IWebHostEnvironment env, EmailService emailService)

    {
        _context = context;
        _env = env;
      _emailService = emailService;
    }

    public IActionResult Index()
    {
        // Try to find the company in the database
        var CompanyId = HttpContext.Session.GetString("company_session");
        var row = _context.Companies.FirstOrDefault(c => c.CompanyId == int.Parse(CompanyId));



        string company_session = HttpContext.Session.GetString("company_session");

        if (company_session != null)
        {
            return View(row);
        }
        else
        {
            return RedirectToAction("LoginCompany");
        }
    }

   
    [HttpGet]
    public IActionResult LoginCompany()
    {
        return View();
    }

    [HttpPost]
    public IActionResult LoginCompany(string companyname, string password, string activitystate)
    {


        // Find company by username only (don't compare password here!)
        var company = _context.Companies
            .FirstOrDefault(c => c.CompanyName == companyname);

        if (company == null)
        {
            // Debugging: If no company found, log the error
            Console.WriteLine("Invalid login attempt for: " + companyname);

            // Display an error message on the page
            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // Password verification using PasswordHasher
        var hasher = new PasswordHasher<Company>();
        var result = hasher.VerifyHashedPassword(company, company.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            // If the password verification fails
            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // Check if the account is disabled (based on activitystate from DB, not from parameter)
        if (company.ActivityState == "Disable")
        {
            Console.WriteLine("Account disabled for: " + companyname);
            ViewBag.Error = "Your account is disabled due to unpaid fees or suspicious activity. Please contact support.";
            return View();
        }

        HttpContext.Session.SetString("company_session", company.CompanyId.ToString());
        // If login is successful, redirect to the company list
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult RegisterCompany()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> RegisterCompany(Company company)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(company);
        }

        // Check if email already exists BEFORE saving
        if (_context.Companies.Any(c => c.Email == company.Email))
        {
            ModelState.AddModelError("Email", "Email already exists.");
            return View(company);  // ← IMPORTANT: return here so user can fix the form
        }
        // Check if company name already exists BEFORE saving
        if (_context.Companies.Any(c => c.CompanyName == company.CompanyName))
        {
            ModelState.AddModelError("CompanyName", "Company name already exists.");
            return View(company);  // ← IMPORTANT: return here so user can fix the form
        }

        // Handle file upload (uncomment and adjust if needed)
        // string imageName = Path.GetFileName(company.CompanyLogo.FileName);
        // string imagePath = Path.Combine(_env.WebRootPath, "images", "Company", "CompanyLogo", imageName);
        // using (FileStream fs = new FileStream(imagePath, FileMode.Create))
        // {
        //     company.CompanyLogo.CopyTo(fs);
        // }
        // company.CompanyLogo = imageName;

        // Password hashing
        var hasher = new PasswordHasher<Company>();
        company.Password = hasher.HashPassword(company, company.Password); // Hash the password

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var subject = "Welcome to radiocab.pk!";
        var message = $@"
        Hello {company.CompanyName},<br><br>
        Thanks for registering with us.<br><br>
        To activate your account, please click on the link below and log in:<br>
        <a href='https://localhost:7054/Company/LoginCompany'>https://localhost:7054/Company/LoginCompany</a><br><br>
        Best regards,<br>
        The <a href='https://localhost:7054/'>radiocab.pk </a> Team
    ";

        await _emailService.SendEmailAsync(company.Email, subject, message);

        return RedirectToAction("RegisterationSuccess");
    }


    public IActionResult RegisterationSuccess()
    {
     return View();
       }
    public IActionResult LogoutCompany()
    {
        //HttpContext.Session.Clear();  // Clears all session data
        HttpContext.Session.Remove("company_session"); // Removes specific session key

        return RedirectToAction("LoginCompany"); // Redirects to login page
    }
    [HttpGet]
    public IActionResult CompanyList()
    {
        // Fetch companies with the specified conditions
        var companies = _context.Companies
            .Where(c => c.Status == "Paid" && c.VisibleMode == "Online" && c.ActivityState == "Active")
            .ToList();

        return View(companies);
    }
    public IActionResult Subscription()
    {
        var companyId = HttpContext.Session.GetString("company_session");

        // Assuming you're storing the company ID in the session. Adjust as necessary.
        if (string.IsNullOrEmpty(companyId))
        {
            // If the company session doesn't exist, you might want to redirect the user to a login page
            return RedirectToAction("LoginCompany");
        }

        var company = _context.Companies.FirstOrDefault(c => c.CompanyId == int.Parse(companyId));

        if (company == null)
        {
            // If no company is found, redirect or show an error message
            return NotFound("Company not found.");
        }

        return View(company);
    }
    public IActionResult QuaterlyPaymentConfirmationn()
    {
        // Get company ID from session (assuming you stored it in session at login)
        var companyId = HttpContext.Session.GetString("company_session");

        if (companyId == null)
        {
            return RedirectToAction("LoginCompany");
        }

        // Find the company in the database
        var company = _context.Companies.FirstOrDefault(c => c.CompanyId.ToString() == companyId);

        if (company == null)
        {
            return RedirectToAction("LoginCompany");
        }

        // Update status to Paid
        company.Status = "Paid";
        _context.Update(company);
        _context.SaveChanges();

        // Optionally, set the payment amount (you can skip if already set in DB)
        company.PaymentAmount = "$40.00";

        return View(company);  // Pass the updated Company object to the view
    }

    public IActionResult MonthlyPaymentConfirmationn()
    {
        // Get company ID from session (assuming you stored it in session at login)
        var companyId = HttpContext.Session.GetString("company_session");

        if (companyId == null)
        {
            return RedirectToAction("LoginCompany");
        }

        // Find the company in the database
        var company = _context.Companies.FirstOrDefault(c => c.CompanyId.ToString() == companyId);

        if (company == null)
        {
            return RedirectToAction("LoginCompany");
        }

        // Update status to Paid
        company.Status = "Paid";
        _context.Update(company);
        _context.SaveChanges();

        // Optionally, set the payment amount (you can skip if already set in DB)
        company.PaymentAmount = "$15.00";

        return View(company);  // Pass the updated Company object to the view
    }
    public IActionResult Profile()
    {
        var companyId = HttpContext.Session.GetString("company_session");

        if (string.IsNullOrEmpty(companyId))
        {
            return RedirectToAction("LoginCompany");
        }

        var company = _context.Companies.FirstOrDefault(c => c.CompanyId == int.Parse(companyId));

        if (company == null)
        {
            return NotFound("Company not found.");
        }

        return View(company);
    }


}
