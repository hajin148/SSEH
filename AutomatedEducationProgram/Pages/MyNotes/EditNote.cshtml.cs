using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class EditNoteModel : PageModel
    {
        public Note CurrentNote;
        public IEnumerable<VocabularyWord> Vocab;
        public IEnumerable<ExamQuestion> MCQuestions;
        public IEnumerable<ExamQuestion> SAQuestions;
        public IEnumerable<ExamQuestion> TFQuestions;
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
            MCQuestions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId && q.QuestionType == ExamQuestion.MULTIPLE_CHOICE_QUESTION).ToList();
            SAQuestions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId && q.QuestionType == ExamQuestion.SHORT_ANSWER_QUESTION).ToList();
            TFQuestions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId && q.QuestionType == ExamQuestion.TF_QUESTION).ToList();
            return Page();

        }

        public IActionResult OnPost(IFormCollection inputs)
        {
            int noteId = int.Parse(inputs["noteId"]);
            Note editedNote = (_context.Notes.Where(note => note.Id == noteId).FirstOrDefault());
            DocumentText defaultDoc = _context.DocumentTexts.Where(dt => dt.parentNote.Id == noteId).FirstOrDefault();
            Dictionary<int, bool> foundTermIds = new Dictionary<int, bool>();
            Dictionary<int, bool> foundQIds = new Dictionary<int, bool>();
            IEnumerable<VocabularyWord> existingVocab = _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToList();
            IEnumerable<ExamQuestion> existingQ = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();

            // I think these can be removed
            //editedNote.VocabularyWords = new List<VocabularyWord>();
            //editedNote.ExamQuestions = new List<ExamQuestion>();

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
                    foundTermIds.Add(id, true);
                    VocabularyWord word = _context.VocabularyWords.Where(w => w.ID == id).FirstOrDefault();
                    word.Term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    word.Definition = inputs[defKey];
                    _context.VocabularyWords.Update(word);
                }
                else if (key.StartsWith("newVocabTerm"))
                {
                    VocabularyWord newTerm = new VocabularyWord();
                    newTerm.Term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    newTerm.Definition = inputs[defKey];
                    newTerm.ParentNote = editedNote;
                    newTerm.RelevantDoc = defaultDoc;
                    _context.VocabularyWords.Add(newTerm);
                }
                else if (key.StartsWith("question"))
                {
                    int id = int.Parse(key.Split(" ")[1]);
                    foundQIds.Add(id, true);
                    ExamQuestion q = _context.ExamQuestions.Where(question => question.Id == id).FirstOrDefault();
                    q.Question = inputs[key];
                    if (q.QuestionType == ExamQuestion.MULTIPLE_CHOICE_QUESTION)
                    {
                        string aKey = key.Replace("question", "ansA");
                        q.AnswerA = inputs[aKey];
                        q.Answer = inputs[aKey];
                        string bKey = key.Replace("question", "ansB");
                        q.AnswerB = inputs[bKey];
                        string cKey = key.Replace("question", "ansC");
                        q.AnswerC = inputs[cKey];
                        string dKey = key.Replace("question", "ansD");
                        q.AnswerD = inputs[dKey];
                    }
                    else
                    {
                        string genericKey = key.Replace("question", "genericAns");
                        q.Answer = inputs[genericKey];
                    }
                    _context.ExamQuestions.Update(q);
                }
                else if (key.StartsWith("newQuestion") && !key.StartsWith("newQuestionType"))
                {
                    int identifier = int.Parse(key.Split(" ")[1]);
                    ExamQuestion newQ = new ExamQuestion();
                    newQ.Question = inputs[key];
                    if (inputs["newQuestionType " + identifier] == ExamQuestion.MULTIPLE_CHOICE_QUESTION)
                    {
                        string aKey = key.Replace("question", "ansA");
                        newQ.AnswerA = inputs[aKey];
                        newQ.Answer = inputs[aKey];
                        string bKey = key.Replace("question", "ansB");
                        newQ.AnswerB = inputs[bKey];
                        string cKey = key.Replace("question", "ansC");
                        newQ.AnswerC = inputs[cKey];
                        string dKey = key.Replace("question", "ansD");
                        newQ.AnswerD = inputs[dKey];
                    }
                    else
                    {
                        string ansKey = key.Replace("Question", "GenericAns");
                        newQ.Answer = inputs[ansKey];
                    }
                    newQ.QuestionType = int.Parse(inputs["newQuestionType " + identifier]);
                    newQ.ParentNote = editedNote;
                    newQ.RelevantDoc = defaultDoc;
                    _context.ExamQuestions.Add(newQ);
                }
            }
            // Delete any content that didn't get passed by the form
            foreach (VocabularyWord word in existingVocab)
            {
                if (!foundTermIds.ContainsKey(word.ID))
                {
                    _context.VocabularyWords.Remove(word);
                }
            }
            foreach (ExamQuestion q in existingQ)
            {
                if (!foundQIds.ContainsKey(q.Id))
                {
                    _context.ExamQuestions.Remove(q);
                }
            }

            editedNote.CreatedDate = DateTime.Now;
            string isPublic = inputs["publicity"];
            if (isPublic != null)
            {
                editedNote.IsPublic = true;
            } else
            {
                editedNote.IsPublic = false;
            }
            _context.Notes.Update(editedNote);
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
