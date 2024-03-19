using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomatedEducationProgram.Pages.SearchNote
{
    public class SearchNoteModel : PageModel
    {
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;

        public SearchNoteModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public string SearchString { get; set; }

        public List<Note> SearchResults { get; set; }
        public List<AEPUser> RelatedUsers { get; set; }
        public List<DateTime> PostDates { get; set; } // Add a property to hold the post dates
        public List<AEPUser> UserSearchResults { get; set; }
        public List<int> PostCounts { get; set; }
        public List<int> FollowerCounts { get; set; }
        public bool SearchedForNotes { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost(IFormCollection inputs)
        {
            RelatedUsers = new List<AEPUser>();
            PostDates = new List<DateTime>(); // Initialize PostDates list
            string user = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToLower();
                SearchedForNotes = inputs["searchOption"] == "notes";
                if (SearchedForNotes)
                {

                    // Retrieve notes that match the search input in either title or description,
                    // including the related user data
                    SearchResults = _context.Notes
                        .Where(note => (note.Title.Contains(SearchString) || note.Description.Contains(SearchString)) && note.IsPublic && note.UserId != user) // Include related user data
                        .Take(50).ToList();

                    foreach (var note in SearchResults)
                    {
                        AEPUser relatedUser = await _userManager.FindByIdAsync(note.UserId);
                        RelatedUsers.Add(relatedUser);
                        PostDates.Add(note.CreatedDate); // Add the post date to the list
                    }
                } else
                {
                    PostCounts = new List<int>();
                    FollowerCounts = new List<int>();
                    UserSearchResults = _userManager.Users.Where(u => u.UserID.Contains(SearchString) && u.Id != user).Take(50).ToList();
                    foreach (AEPUser u in UserSearchResults)
                    {
                        int postCount = _context.Notes.Where(note => note.UserId == u.Id).Count();
                        PostCounts.Add(postCount);
                        int followerCount = _context.Followings.Where(f => f.Followed == u.Id && !f.Pending).Count();
                        FollowerCounts.Add(followerCount);
                    }
                    
                }
            }

            return Page();
        }
    }
}
