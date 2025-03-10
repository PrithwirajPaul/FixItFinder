using FixItFinderDemo.Data;
using FixItFinderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItFinderDemo.Controllers
{
    public class PostEngagementController(FIFContext context) : Controller
    {
        private readonly FIFContext _context = context;

        public async Task<IActionResult> Apply(int postId)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var newEngagement = new Post_Engagement
            {
                PostId = postId,
                EngagedUserId = currentUserId,
                Status = 2 // Pending
            };

            _context.Post_Engagements.Add(newEngagement);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssemblyPage", "Account");
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new List<object>());

            var notifications = new List<object>();

            var applications = _context.Post_Engagements
                .Include(pe => pe.EngagedUser)
                .Include(pe => pe.Post)
                .ThenInclude(p => p.User)
                .Where(pe => pe.Post.UserId == userId && pe.Status == 2)
                .Select(pe => new
                {
                    EngagedUserName = pe.EngagedUser.Name ?? "Unknown",
                    PostId = pe.Post.PId,
                    PostTitle = pe.Post.Description ?? "Service",
                    PostEngagementId = pe.Id,
                    HasActions = true
                }).ToList();

            notifications.AddRange(applications);

            var acceptances = _context.Post_Engagements
                .Include(pe => pe.Post)
                .ThenInclude(p => p.User)
                .Where(pe => pe.EngagedUserId == userId && pe.Status == 3)
                .Select(pe => new
                {
                    ServiceProviderName = pe.Post.User.Name ?? "Unknown",
                    PostId = pe.Post.PId,
                    PostTitle = pe.Post.Description ?? "Service",
                    PostEngagementId = pe.Id,
                    HasActions = false
                }).ToList();

            notifications.AddRange(acceptances);

            return Json(notifications);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AcceptApplication(int id)
        {
            var engagement = _context.Post_Engagements.FirstOrDefault(pe => pe.Id == id);
            if (engagement == null)
                return NotFound();

            engagement.Status = 3; // Accepted

            var otherApplicants = _context.Post_Engagements
                .Where(pe => pe.PostId == engagement.PostId && pe.Id != id)
                .ToList();
            foreach (var applicant in otherApplicants)
            {
                applicant.Status = 4; // Rejected
            }

            var post = _context.Posts.FirstOrDefault(p => p.PId == engagement.PostId);
            if (post != null)
                post.Post_Status = 2;

            _context.SaveChanges();
            return Ok(new { success = true });
        }

        [HttpPost]
        public IActionResult RejectApplication(int id)
        {
            var engagement = _context.Post_Engagements.FirstOrDefault(pe => pe.Id == id);
            if (engagement == null)
                return NotFound();

            engagement.Status = 4; // Rejected

            _context.SaveChanges();
            return Ok(new { success = true });
        }
    }
}
