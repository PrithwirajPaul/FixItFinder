using FixItFinderDemo.Data;
using FixItFinderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItFinderDemo.Controllers
{
    public class OffAccountController(FIFContext context) : Controller
    {
        private readonly FIFContext _context = context;

        public IActionResult OfferService()
        {
            return View();
        }

        public IActionResult ServiceOfferPage(string category)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("UserRole");

            var customerPosts = _context.Posts
                .Include(p => p.User)
                .ThenInclude(u => u.Customer_Profile)
                .Include(p => p.PostEngagements)
                .Where(p => p.User != null
                         && p.User.Role == "Customer")
                .ToList();

            ViewBag.UserId = userId;
            ViewBag.Role = role;
            ViewBag.Category = category;
            if (role == "Service Provider")
            {
                ViewBag.UserCategory = HttpContext.Session.GetString("UserCategory");
            }

            return View(customerPosts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("OffAccount/CreatePost")]
        public async Task<IActionResult> CreatePost([Bind("Description,UserId,Price")] Post post)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Valid");
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    ModelState.AddModelError("", "You must be logged in to create a post.");
                    return RedirectToAction("Login", "Account");
                }

                post.UserId = userId;
                post.Post_Status = 1;
                post.Views = 0;

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpGet]
        public IActionResult AssemblyOPage() => ServiceOfferPage("Assembler");

        [HttpGet]
        public IActionResult MountingOPage() => ServiceOfferPage("Mounter");

        [HttpGet]
        public IActionResult CleaningOPage() => ServiceOfferPage("Cleaner");

        [HttpGet]
        public IActionResult HouseRepairOPage() => ServiceOfferPage("Repairer");

        [HttpGet]
        public IActionResult ElectritiansOPage() => ServiceOfferPage("Electrician");

        [HttpGet]
        public IActionResult PlumbingOPage() => ServiceOfferPage("Plumber");

        [HttpGet]
        public IActionResult PainterOPage() => ServiceOfferPage("Painter");

        [HttpGet]
        public IActionResult CarpenterOPage() => ServiceOfferPage("Carpenter");
    }
}
