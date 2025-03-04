using FixItFinderDemo.Data;
using FixItFinderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FixItFinderDemo.Controllers
{
    public class PostEngagementController(FIFContext context) : Controller


    {
        private readonly FIFContext _context = context;
    
        public async Task<IActionResult> Apply (int postId)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId")??0;

            var newEngagement = new Post_Engagement
            {
                PostId = postId,
                EngagedUserId = currentUserId,
                Status = 2 
            };

            _context.Post_Engagements.Add(newEngagement);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssemblyPage", "Account");
        }
    }
}
