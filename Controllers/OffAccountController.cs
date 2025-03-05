using FixItFinderDemo.Data;
using FixItFinderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItFinderDemo.Controllers
{
    public class OffAccountController(FIFContext context) : Controller
    {
        private readonly FIFContext _context = context;

        
        private List<Post> FetchCustomerPosts(string category)
        {
            return _context.Posts
                .Include(p => p.User)
                .ThenInclude(u => u.Customer_Profile)
                .Where(p => p.User != null
                         && p.User.Role == "Customer"
                         && p.User.Customer_Profile != null
                         && p.User.Customer_Profile.Category == category)
                .ToList();
        }

        public IActionResult OfferService()
        {
            return View();
        }

        public IActionResult ServiceOfferPage(string category)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("UserRole");

            var customerPosts = FetchCustomerPosts(category);

            ViewBag.UserId = userId;
            ViewBag.Role = role;

            return View(customerPosts);
        }
