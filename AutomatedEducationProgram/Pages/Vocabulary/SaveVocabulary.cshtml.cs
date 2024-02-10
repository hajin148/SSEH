using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class SaveVocabularyModel : PageModel
    {
        public List<VocabularyWord> ProcessedVocabulary { get; set; }
        public List<Note> ExistingNotes { get; set; }
        public string Text { get; set; }

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public SaveVocabularyModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
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

            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
            }

            ExistingNotes = _context.Notes.Where(note => note.UserId == user).ToList();

            return Page();
        }

        public IActionResult OnPostAsync(IFormCollection inputs)
        {
            string user = _userManager.GetUserId(User);
            var textJson = HttpContext.Session.GetString("Text");
            Text = JsonConvert.DeserializeObject<string>(textJson);
            DocumentText documentText = new DocumentText();
            documentText.Text = Text;
            string buttonClicked = HttpContext.Request.Form["submitButton"];
            List<VocabularyWord> wordsToSave = new List<VocabularyWord>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("vocabTerm") || key.StartsWith("newVocabTerm"))
                {
                    string term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    string def = inputs[defKey];
                    wordsToSave.Add(new VocabularyWord(term, def, documentText));
                }
            }
            // If merging with existing Note
            if (buttonClicked == "Merge To Existing Note")
            {
                int noteToUpdateId = int.Parse(inputs["existingNotes"]);
                Note noteToUpdate = _context.Notes.Where(note => note.Id == noteToUpdateId).FirstOrDefault();
                foreach (VocabularyWord word in wordsToSave)
                {
                    word.ParentNote = noteToUpdate;
                    _context.VocabularyWords.Add(word);
                }
                documentText.parentNote = noteToUpdate;
                _context.DocumentTexts.Add(documentText);
            }
            // If creating new Note
            else
            {
                Note noteToSave = new Note();
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
                documentText.parentNote = noteToSave;
                _context.DocumentTexts.Add(documentText);
            }
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
