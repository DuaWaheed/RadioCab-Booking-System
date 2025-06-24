using Microsoft.AspNetCore.Mvc;
using E_project.Models;

namespace E_project.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly mycontext _context;

        public FeedbackController(IWebHostEnvironment webHostEnvironment, mycontext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET
        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Create the feedback folder if it doesn't exist
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "feedback");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Generate a unique filename
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    string newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(uploadDir, newFileName);

                    // Save the file to wwwroot/feedback
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Store the relative path in the database
                    model.ImagePath = "/feedback/" + newFileName;
                }

                _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thank you for your feedback!";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


    }
}
