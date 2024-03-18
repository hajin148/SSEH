using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.SearchNote
{
    public class ViewNoteModel : PageModel
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        public Note CurrentNote { get; set; }
        public List<VocabularyWord> Vocabulary { get; set; }
        public List<ExamQuestion> Questions { get; set; }
        public int? noteNum { get; set; }
        public string CreatorUsername { get; set; }
        public string CreatorId {  get; set; }

        public ViewNoteModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int? noteId)
        {

            noteNum = noteId;
            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            CurrentNote = _context.Notes.Where(note => note.Id == noteId).FirstOrDefault();
            Vocabulary = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();
            AEPUser CreatorOfNote = await _userManager.FindByIdAsync(CurrentNote.UserId);
            CreatorUsername = CreatorOfNote.UserID;
            CreatorId = CreatorOfNote.Id; 

            return Page();
        }

        public IActionResult OnPost(IFormCollection inputs)
        {
            // Get info about the Note currently being viewed
            int noteToDownloadID = int.Parse(inputs["searchedNoteId"]);
            CurrentNote = _context.Notes.Where(note => note.Id == noteToDownloadID).FirstOrDefault();
            Vocabulary = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteToDownloadID).ToList();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteToDownloadID).ToList();

            // Make a new Note
            Note newNote = new Note();
            newNote.Title = CurrentNote.Title;
            newNote.Description = CurrentNote.Description;
            newNote.CreatedDate = DateTime.Now;
            newNote.IsPublic = false;
            newNote.UserId = _userManager.GetUserId(User);

            // Copy content
            List<VocabularyWord> copiedWords = new List<VocabularyWord>();
            foreach (VocabularyWord word in Vocabulary)
            {
                VocabularyWord copy = word.Copy();
                copy.ParentNote = newNote;
                copiedWords.Add(copy);
            }
            List<ExamQuestion> copiedQs = new List<ExamQuestion>();
            foreach (ExamQuestion q in Questions)
            {
                ExamQuestion copy = q.Copy();
                copy.ParentNote = newNote;
                copiedQs.Add(copy);
            }

            // Add changes to database and save
            _context.VocabularyWords.AddRange(copiedWords);
            _context.ExamQuestions.AddRange(copiedQs);
            _context.Notes.Add(newNote);
            _context.SaveChanges();
            return RedirectToPage("MyNotes");

        }
    }
}
