using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class StudyNoteModel : PageModel
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        public Note CurrentNote { get; set; }
        public List<VocabularyWord> Vocabulary { get; set; }
        public List<ExamQuestion> Questions { get; set; }

        public StudyNoteModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
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
            Vocabulary = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();

            return Page();
        }
    }
}
