using FixItFinderDemo.Data;
using FixItFinderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FixItFinderDemo.Controllers
{
    public class AccountController(FIFContext context) : Controller
    {
        private readonly FIFContext _context = context;

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexStringLower(hashedBytes);
        }

        private List<Post> FetchPosts(string category)
        {
            return _context.Posts
            .Include(p => p.User)
            .ThenInclude(u => u.Worker_Profile)
            .Include(p => p.PostEngagements)
            .Where(p => p.User != null
                     && p.User.Role == "Service Provider"
                     && p.User.Worker_Profile != null
                     && p.User.Worker_Profile.Category == category)
            .ToList();
        }

        public IActionResult ProfsFetchPosts(string category)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("UserRole");
            var PostsToShow = FetchPosts(category);

            ViewBag.UserId = userId;
            ViewBag.Role = role;
            ViewBag.Category = category;
            if (role == "Service Provider")
            {
                ViewBag.UserCategory = HttpContext.Session.GetString("UserCategory");
            }

            return View(PostsToShow);
        }

        public IActionResult FindAPro()
        {
            return View();
        }
        [HttpGet]
        [Route("Account/AssemblyPage")]
        public IActionResult AssemblyPage() { return ProfsFetchPosts("Assembler"); }

        public IActionResult MountingPage() { return ProfsFetchPosts("Mounter"); }

        public IActionResult CleaningPage() { return ProfsFetchPosts("Cleaner"); }

        public IActionResult HouseRepairs() { return ProfsFetchPosts("Repairer"); }

        public IActionResult ElectritiansPage() { return ProfsFetchPosts("Electrician"); }

        public IActionResult PlumbingPage() { return ProfsFetchPosts("Plumber"); }

        public IActionResult PainterPage() { return ProfsFetchPosts("Painter"); }

        public IActionResult CarpenterPage() { return ProfsFetchPosts("Carpenter"); }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password,Role")] User loginUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Worker_Profile).Where(u=>u.Worker_Profile!=null) 
                    .FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.Role == loginUser.Role);
                if (user == null)
                {
                    ModelState.AddModelError("", "No account found with this email and role.");
                    return View(loginUser);
                }
                string hashedInputPassword = HashPassword(loginUser.Password);
                if (user.Password != hashedInputPassword)
                {
                    ModelState.AddModelError("", "Invalid password.");
                    return View(loginUser);
                }
                HttpContext.Session.SetInt32("UserId", user.UId);
                HttpContext.Session.SetString("UserName", user.Name ?? "Unknown");
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("LoggedIn", "True");
                HttpContext.Session.SetString("Image","/Customer.jpeg");
                if (user.Role.Equals("Service Provider"))
                {
                    HttpContext.Session.SetString("UserCategory", user.Worker_Profile.Category);
                    HttpContext.Session.SetString("Image","/ServiceProvider.jpeg");
                }

                return RedirectToAction("FindAPro", "Account");
            }
            return View(loginUser);
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            //Console.WriteLine("At start");
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Role == model.Role);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    City = model.City,
                    ZipCode = model.ZipCode,
                    Role = model.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (model.Role == "Customer")
                {
                    var customerProfile = new Customer_Profile
                    {
                        UserId = user.UId,

                    };
                    _context.Customer_Profiles.Add(customerProfile);
                }
                else if (model.Role == "Service Provider")
                {
                    var workerProfile = new Worker_Profile
                    {
                        UserId = user.UId,
                        Category = model.Category ?? "",
                        Experience = model.Experience ?? 0
                    };
                    _context.Worker_Profiles.Add(workerProfile);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            Console.WriteLine("Here");
            return View(model);
        }


        [HttpPost]
        [Route("Account/CreatePost")]
        [ValidateAntiForgeryToken]
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
    }
}
