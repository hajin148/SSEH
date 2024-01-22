using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using AutomatedEducationProgram.Views.MyNotes;
using EduApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;

namespace AutomatedEducationProgram.Controllers
{
    public class MyNotesController : Controller
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public MyNotesController(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> MyNotes()
        {
            string userId = _userManager.GetUserId(User);
            IEnumerable<AutomatedEducationProgram.Models.Note> usersNotes = await _context.Notes.Where(note => note.UserId == userId).ToListAsync();
            return View(usersNotes);
        }

        public async Task<IActionResult> StudyNote(int noteId)
        {
            Note note = (await _context.Notes.Where(note => note.Id == noteId).ToListAsync())[0];
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            IEnumerable<ExamQuestion> examQuestions = await _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToListAsync();

            return View(new Tuple<Note, IEnumerable<VocabularyWord>, IEnumerable<ExamQuestion>>(note, vocab, examQuestions));

        }

        public async Task<IActionResult> EditNote(int noteId)
        {
            Note note = (await _context.Notes.Where(note => note.Id == noteId).ToListAsync())[0];
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            IEnumerable<ExamQuestion> examQuestions = await _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToListAsync();

            return View(new Tuple<Note, IEnumerable<VocabularyWord>, IEnumerable<ExamQuestion>>(note, vocab, examQuestions));

        }

        public async Task<IActionResult> DeleteNote(int noteId)
        {
            Note note = (await _context.Notes.Where(note => note.Id == noteId).ToListAsync())[0];
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            IEnumerable<ExamQuestion> examQuestions = await _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToListAsync();

            return View(new Tuple<Note, IEnumerable<VocabularyWord>, IEnumerable<ExamQuestion>>(note, vocab, examQuestions));

        }

        [HttpPost]
        public async Task<IActionResult> EditNote(IFormCollection inputs)
        {
            int noteId = int.Parse(inputs["noteId"]);
            List<VocabularyWord> existingVocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            foreach(var word in existingVocab)
            {
                _context.VocabularyWords.Remove(word);
            }
            List<ExamQuestion> existingQs = await _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToListAsync();
            foreach (var q in existingQs)
            {
                _context.ExamQuestions.Remove(q);
            }
            Note toDelete = (await _context.Notes.Where(note => note.Id == noteId).ToListAsync())[0];
            _context.Remove(toDelete);

            Note editedNote = new Note();
            editedNote.VocabularyWords = new List<VocabularyWord>();
            editedNote.ExamQuestions = new List<ExamQuestion>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("title"))
                {
                    editedNote.Title = inputs[key];
                } else if (key.StartsWith("description"))
                {
                    editedNote.Description = inputs[key];
                } else if (key.StartsWith("userId"))
                {
                    editedNote.UserId = inputs[key];
                } else if (key.StartsWith("vocabTerm"))
                {
                    VocabularyWord word = new VocabularyWord();
                    word.ParentNote = editedNote;
                    word.Term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    word.Definition = inputs[defKey];
                    editedNote.VocabularyWords.Add(word);
                } else if (key.StartsWith("question"))
                {
                    ExamQuestion q = new ExamQuestion();
                    q.ParentNote = editedNote;
                    q.Question = inputs[key];
                    string aKey = key.Replace("question", "ansA");
                    q.AnswerA = inputs[aKey];
                    string bKey = key.Replace("question", "ansB");
                    q.AnswerB = inputs[bKey];
                    string cKey = key.Replace("question", "ansC");
                    q.AnswerC = inputs[cKey];
                    string dKey = key.Replace("question", "ansD");
                    q.AnswerD = inputs[dKey];
                    string eKey = key.Replace("question", "explanation");
                    q.Explanation = inputs[eKey];
                    editedNote.ExamQuestions.Add(q);
                }
            }
            editedNote.CreatedDate = DateTime.Now;
            _context.Notes.Add(editedNote);
            _context.SaveChanges();
            return View();
        }
    }
}
