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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            RelatedUsers = new List<AEPUser>();
            PostDates = new List<DateTime>(); // Initialize PostDates list
            string user = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(SearchString))
            {
                // Retrieve notes that match the search input in either title or description,
                // including the related user data
                SearchResults = _context.Notes
                    .Where(note => (note.Title.Contains(SearchString) || note.Description.Contains(SearchString)) && note.IsPublic && note.UserId != user) // Include related user data
                    .ToList();

                foreach (var note in SearchResults)
                {
                    AEPUser relatedUser = await _userManager.FindByIdAsync(note.UserId);
                    RelatedUsers.Add(relatedUser);
                    PostDates.Add(note.CreatedDate); // Add the post date to the list
                }
            }

            return Page();
        }
    }
}
