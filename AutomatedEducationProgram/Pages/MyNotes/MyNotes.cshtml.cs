using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class MyNotesModel : PageModel
    {
        public IEnumerable<Note> UserNotes { get; set; }
        public IEnumerable<DocumentText> UserDocs { get; set; }

        public List<Following> PendingFollowRequests { get; set; }
        public List<string> FollowRequesterNames {  get; set; }

        public List<AEPUser> Followers { get; set; }
        public List<AEPUser> Followees {  get; set; }

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
            UserNotes = _context.Notes.Where(note => note.UserId == user).ToList();
            UserDocs = _context.DocumentTexts.Where(dtext => dtext.parentNote.UserId == user).ToList();
            PendingFollowRequests = _context.Followings.Where(f => f.Followed == user && f.Pending).Take(50).ToList();
            FollowRequesterNames = new List<string>();
            Followers = new List<AEPUser>();
            Followees = new List<AEPUser>();
            foreach (var f in PendingFollowRequests)
            {
                FollowRequesterNames.Add(_userManager.Users.Where(user => user.Id == f.Follower).First().UserID);
            }
            IEnumerable<Following> FollowerPairs = _context.Followings.Where(f => f.Followed == user && !f.Pending).Take(50).ToList();
            foreach(var f in FollowerPairs)
            {
                Followers.Add(_userManager.Users.Where(user => user.Id == f.Follower).First());
            }
            IEnumerable<Following> FolloweePairs = _context.Followings.Where(f => f.Follower == user && !f.Pending).Take(50).ToList();
            foreach (var f in FolloweePairs)
            {
                Followees.Add(_userManager.Users.Where(user => user.Id == f.Followed).First());
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
            } else if (action == "Unfollow")
            {
                string idOfUserToUnfollow = inputs["followId"];
                Following followingToRemove = _context.Followings.Where(f => f.Follower == user && f.Followed == idOfUserToUnfollow).FirstOrDefault();
                _context.Followings.Remove(followingToRemove);
            } else if (action == "Remove Follower")
            {
                string idOfFollowerToRemove = inputs["followId"];
                Following followingToRemove = _context.Followings.Where(f => f.Followed == user && f.Follower == idOfFollowerToRemove).FirstOrDefault();
                _context.Followings.Remove(followingToRemove);
            }
            _context.SaveChanges();
            return Redirect("MyNotes");
        }
    }
}
