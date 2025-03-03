using FixItFinderDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItFinderDemo.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FIFContext _context;

        public ProfileController(FIFContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? userId)
        {
            if (userId == null)
            {
                userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            }

            var user = await _context.Users
                .Include(u => u.Customer_Profile)
                .Include(u => u.Worker_Profile)
                .FirstOrDefaultAsync(u => u.UId == userId);

            if (user == null) return NotFound();

            if (user.Customer_Profile != null)
            {
                var customer = user.Customer_Profile;

                var servicesLookingFor = await _context.Posts
                    .Where(p => p.UserId == customer.UserId)
                    .Include(p => p.User)
                    .ThenInclude(u => u.Worker_Profile)
                    .ToListAsync();

                var servicesTaken = await _context.Service_Histories
                    .Where(sh => sh.CustomerId == customer.UserId)
                    .Include(sh => sh.Worker)
                    .ThenInclude(w => w.User)
                    .ToListAsync();

                ViewData["ServicesLookingFor"] = servicesLookingFor;
                ViewData["ServicesTaken"] = servicesTaken;

                return View("CustomerProfile", customer);
            }
            else if (user.Worker_Profile != null)
            {
                var worker = user.Worker_Profile;

                var ratings = await _context.Service_Histories
                    .Where(sh => sh.WorkerId == worker.Id)
                    .Select(sh => sh.Rating)
                    .ToListAsync();

                worker.Rating = ratings.Any() ? ratings.Average() : 0;

                _context.Worker_Profiles.Update(worker);
                await _context.SaveChangesAsync();

                var serviceOffering = await _context.Posts
                    .Where(p => p.UserId == worker.UserId)
                    .Include(p => p.User)
                    .ToListAsync();

                var serviceProvided = await _context.Service_Histories
                    .Where(sh => sh.WorkerId == worker.UserId)
                    .Include(sh => sh.Customer)
                    .ThenInclude(c => c.User)
                    .ToListAsync();

                ViewData["ServiceOffering"] = serviceOffering;
                ViewData["ServiceProvided"] = serviceProvided;

                return View("WorkerProfile", worker);
            }

            return RedirectToAction("Login", "Account");
        }

    }
}
