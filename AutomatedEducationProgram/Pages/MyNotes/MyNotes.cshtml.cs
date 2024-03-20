using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class MyNotesModel : PageModel
    {
        public IEnumerable<Note> UserNotes { get; set; }
        public IEnumerable<DocumentText> UserDocs { get; set; }

        public List<Following> PendingFollowRequests { get; set; }
        public List<string> FollowRequesterNames {  get; set; }

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public MyNotesModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            string userId = _userManager.GetUserId(User);
            UserNotes = _context.Notes.Where(note => note.UserId == userId).ToList();
            UserDocs = _context.DocumentTexts.Where(dtext => dtext.parentNote.UserId == userId).ToList();
            PendingFollowRequests = _context.Followings.Where(f => f.Followed == user && f.Pending).Take(50).ToList();
            FollowRequesterNames = new List<string>();
            foreach (var f in PendingFollowRequests)
            {
                FollowRequesterNames.Add(_userManager.Users.Where(user => user.Id == f.Follower).First().UserID);
            }
            return Page();
        }

        public IActionResult OnPost(IFormCollection inputs)
        {
            string action = inputs["action"];
            string user = _userManager.GetUserId(User);
            if (action == "Approve")
            {
                int followingId = int.Parse(inputs["followId"]);
                Following following = _context.Followings.Where(f => f.Id == followingId).FirstOrDefault();
                following.Pending = false;
                _context.Followings.Update(following);
            } else if (action == "Delete")
            {
                int followingId = int.Parse(inputs["followId"]);
                Following following = _context.Followings.Where(f => f.Id == followingId).FirstOrDefault();
                _context.Followings.Remove(following);
            } else if (action == "Approve All")
            {
                IEnumerable<Following> toApprove = _context.Followings.Where(f => f.Followed == user);
                foreach (Following following in toApprove)
                {
                    following.Pending = false;
                }
                _context.Followings.UpdateRange(toApprove);
            } else if (action == "Delete All")
            {
                IEnumerable<Following> toDelete = _context.Followings.Where(f => f.Followed == user);
                _context.Followings.RemoveRange(toDelete);
            }
            _context.SaveChanges();
            return Redirect("MyNotes");
        }
    }
}
