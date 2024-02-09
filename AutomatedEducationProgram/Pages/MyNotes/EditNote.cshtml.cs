using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class EditNoteModel : PageModel
    {
        public Note CurrentNote;
        public IEnumerable<VocabularyWord> Vocab;
        public IEnumerable<ExamQuestion> Questions;
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public EditNoteModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
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
            Vocab = _context.VocabularyWords.Where(word => word.ParentNote.Id ==  noteId).ToList();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();

            return Page();

        }

        public IActionResult OnPost(IFormCollection inputs)
        {
            int noteId = int.Parse(inputs["noteId"]);
            Note editedNote = (_context.Notes.Where(note => note.Id == noteId).FirstOrDefault());
            editedNote.VocabularyWords = new List<VocabularyWord>();
            editedNote.ExamQuestions = new List<ExamQuestion>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("title"))
                {
                    editedNote.Title = inputs[key];
                }
                else if (key.StartsWith("description"))
                {
                    editedNote.Description = inputs[key];
                }
                else if (key.StartsWith("vocabTerm"))
                {
                    int id = int.Parse(key.Split(" ")[1]);
                    VocabularyWord word = _context.VocabularyWords.Where(w => w.ID == id).FirstOrDefault();
                    word.Term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    word.Definition = inputs[defKey];
                    _context.VocabularyWords.Update(word);
                }
                else if (key.StartsWith("question"))
                {
                    int id = int.Parse(key.Split(" ")[1]);
                    ExamQuestion q = _context.ExamQuestions.Where(question => question.Id == id).FirstOrDefault();
                    q.Question = inputs[key];
                    /*string aKey = key.Replace("question", "ansA");
                    q.AnswerA = inputs[aKey];
                    string bKey = key.Replace("question", "ansB");
                    q.AnswerB = inputs[bKey];
                    string cKey = key.Replace("question", "ansC");
                    q.AnswerC = inputs[cKey];
                    string dKey = key.Replace("question", "ansD");
                    q.AnswerD = inputs[dKey];*/
                    string genericKey = key.Replace("question", "genericAns");
                    q.Answer = inputs[genericKey];
                    /*string eKey = key.Replace("question", "explanation");
                    q.Explanation = inputs[eKey];
                    string typeKey = key.Replace("question", "qType");
                    q.QuestionType = int.Parse(inputs[typeKey]);
                    string docKey = key.Replace("question", "qDocId");
                    int docId = int.Parse(inputs[docKey]);
                    q.RelevantDoc = _context.DocumentTexts.Where(dt => dt.Id == docId).FirstOrDefault();*/
                    _context.ExamQuestions.Update(q);
                }
            }
            editedNote.CreatedDate = DateTime.Now;
            _context.Notes.Update(editedNote);
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
