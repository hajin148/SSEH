using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            RelatedUsers = new List<AEPUser>();
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Retrieve notes that match the search input in either title or description,
                // including the related user data
                SearchResults = _context.Notes
                    .Where(note => EF.Functions.Like(note.Title, $"%{SearchString}%") || EF.Functions.Like(note.Description, $"%{SearchString}%")) // Include related user data
                    .ToList();
                foreach (var note in SearchResults)
                {
                    AEPUser relatedUser = await _userManager.FindByIdAsync(note.UserId);
                    RelatedUsers.Add(relatedUser);
                }
            }

            return Page();
        }
    }
}