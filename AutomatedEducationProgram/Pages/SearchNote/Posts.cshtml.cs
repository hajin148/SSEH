using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.SearchNote
{
    public class PostsModel : PageModel
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        public string Username {  get; set; }
        public List<Note> Notes { get; set; }
        public string Major { get; set; }
        public bool FollowingThisUser {  get; set; }
        public string UserId { get; set; }

        public PostsModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task OnGetAsync(string? userId)
        {
            AEPUser currentUser = await _userManager.FindByIdAsync(userId);
            string idOfLoggedInUser = _userManager.GetUserId(User);
            FollowingThisUser = _context.Followings.Where(pair => pair.Follower == idOfLoggedInUser && pair.Followed == userId && !pair.Pending).Any();
            Notes = _context.Notes.Where(note => note.UserId == userId && (note.IsPublic || FollowingThisUser)).ToList();
            Username = currentUser.UserID;
            Major = currentUser.Major;
            UserId = currentUser.Id;

        }

        public async Task<RedirectToPageResult> OnPostAsync(IFormCollection inputs)
        {
            string followerId = _userManager.GetUserId(User);
            string followedId = inputs["idOfFollowed"];
            bool requestALreadyExists = _context.Followings.Where(pair => pair.Follower == followerId && pair.Followed == followedId).Any();
            if (!requestALreadyExists)
            {
                Following request = new Following();
                request.Follower = followerId;
                request.Followed = followedId; 
                request.Pending = true;
                _context.Followings.Add(request);
                _context.SaveChanges();
            }  
            return RedirectToPage("FollowRequestSent");
        }
    }
}
