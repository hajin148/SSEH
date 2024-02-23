using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
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

        public SearchNoteModel(AutomatedEducationProgramContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string SearchString { get; set; }

        public List<Note> SearchResults { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Retrieve notes that match the search input in either title or description
                SearchResults = _context.Notes
                    .Where(note => EF.Functions.Like(note.Title, $"%{SearchString}%") || EF.Functions.Like(note.Description, $"%{SearchString}%"))
                    .ToList();
            }

            return Page();
        }
    }
}
