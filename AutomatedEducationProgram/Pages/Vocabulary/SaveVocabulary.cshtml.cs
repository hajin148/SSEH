using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class SaveVocabularyModel : PageModel
    {
        public List<VocabularyWord> ProcessedVocabulary { get; set; }
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public SaveVocabularyModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public void OnGet()
        {
            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
            }
        }

        public IActionResult OnPostAsync(IFormCollection inputs)
        {
            List<VocabularyWord> wordsToSave = new List<VocabularyWord>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("vocabTerm"))
                {
                    string term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    string def = inputs[defKey];
                    wordsToSave.Add(new VocabularyWord(term, def));
                }
            }
            Note noteToSave = new Note();
            string user = _userManager.GetUserId(User);
            noteToSave.Title = inputs["title"];
            noteToSave.Description = inputs["description"];
            noteToSave.VocabularyWords = wordsToSave;
            noteToSave.UserId = user;
            noteToSave.CreatedDate = DateTime.Now;
            _context.Notes.Add(noteToSave);
            foreach (var word in wordsToSave)
            {
                word.ParentNote = noteToSave;
                _context.VocabularyWords.Add(word);
            }
            _context.SaveChanges();
            return RedirectToPage("MyNotes");

        }
    }
}
