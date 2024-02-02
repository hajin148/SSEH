using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class DeleteNoteModel : PageModel
    {
        public Note CurrentNote;
        public IEnumerable<VocabularyWord> Vocab;
        public IEnumerable<ExamQuestion> Questions;
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public DeleteNoteModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult OnGet(int? noteId)
        {
            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }

            CurrentNote = _context.Notes.Where(note => note.Id == noteId).FirstOrDefault();
            Vocab = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();

            return Page();

        }

        public IActionResult OnPost(int? noteId, IFormCollection inputs)
        {
            List<VocabularyWord> existingVocab = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            foreach (var word in existingVocab)
            {
                _context.VocabularyWords.Remove(word);
            }
            List<ExamQuestion> existingQs = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();
            foreach (var q in existingQs)
            {
                _context.ExamQuestions.Remove(q);
            }
            Note toDelete = (_context.Notes.Where(note => note.Id == noteId).ToList())[0];
            _context.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
