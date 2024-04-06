using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class FlashcardModel : PageModel
    {
        public List<VocabularyWord> ProcessedVocabulary { get; set; }
        public string text { get; set; }

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public FlashcardModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult OnGet(int? noteId)
        {
            string user = _userManager.GetUserId(User);
            Note CurrentNote = _context.Notes.Where(note => note.Id == noteId).FirstOrDefault();
            if (CurrentNote.UserId != user)
            {
                return RedirectToPage("/Error");
            }
            ProcessedVocabulary = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            return Page();

        }

        public IActionResult OnPostAsync()
        {
            var vocabularyJson = JsonConvert.SerializeObject(ProcessedVocabulary);
            HttpContext.Session.SetString("ProcessedVocabulary", vocabularyJson);

            var textJson = JsonConvert.SerializeObject(text);
            HttpContext.Session.SetString("Text", textJson);

            return Page();
        }
    }
}

