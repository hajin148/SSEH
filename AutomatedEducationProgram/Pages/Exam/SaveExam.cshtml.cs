using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class SaveExamModel : PageModel
    {
        public List<ExamQuestion> GeneratedQuestionsMCQ { get; set; }
        public List<ExamQuestion> GeneratedQuestionsShort {  get; set; }
        public List<ExamQuestion> GeneratedQuestionsTF {  get; set; }
        public List<Note> ExistingNotes { get; set; }
        public string Text { get; set; }
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public SaveExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
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

            var mcqJson = HttpContext.Session.GetString("MCQQuestion");
            var shortJson = HttpContext.Session.GetString("ShortQuestion");
            var tfJson = HttpContext.Session.GetString("TFQuestion");

            if (!string.IsNullOrEmpty(mcqJson))
            {
                GeneratedQuestionsMCQ = JsonConvert.DeserializeObject<List<ExamQuestion>>(mcqJson);
                foreach (ExamQuestion q in GeneratedQuestionsMCQ)
                {
                    q.QuestionType = ExamQuestion.MULTIPLE_CHOICE_QUESTION;
                }
            }

            if (!string.IsNullOrEmpty(shortJson))
            {
                GeneratedQuestionsShort = JsonConvert.DeserializeObject<List<ExamQuestion>>(shortJson);
                foreach (ExamQuestion q in GeneratedQuestionsShort)
                {
                    q.QuestionType = ExamQuestion.SHORT_ANSWER_QUESTION;
                }
            }

            if (!string.IsNullOrEmpty(tfJson))
            {
                GeneratedQuestionsTF = JsonConvert.DeserializeObject<List<ExamQuestion>>(tfJson);
                foreach (ExamQuestion q in GeneratedQuestionsTF)
                {
                    q.QuestionType = ExamQuestion.TF_QUESTION;
                }
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
            List<ExamQuestion> qsToSave = new List<ExamQuestion>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("examQ") || key.StartsWith("newExamQ"))
                {
                    string q = inputs[key];
                    string ansKey = key.Replace("Q", "A");
                    string ans = inputs[ansKey];
                    string typeKey = key.Replace("Q", "T");
                    int type = int.Parse(inputs[typeKey]);
                    qsToSave.Add(new ExamQuestion(q, ans, type, documentText));
                }
                if (key.StartsWith("mcqExamQ") || key.StartsWith("newMcqExamQ"))
                {
                    string q = inputs[key];
                    string ansKey = key.Replace("Q", "A");
                    string ans = inputs[ansKey];
                    string typeKey = key.Replace("Q", "T");
                    int type = int.Parse(inputs[typeKey]);
                    ExamQuestion newQuestion = new ExamQuestion(q, ans, type, documentText);
                    newQuestion.Answer = ans;
                    List<string> answers = new List<string>();
                    newQuestion.AnswerA = inputs[key.Replace("Q", "A")];
                    newQuestion.AnswerB = inputs[key.Replace("Q", "B")];
                    newQuestion.AnswerC = inputs[key.Replace("Q", "C")];
                    newQuestion.AnswerD = inputs[key.Replace("Q", "D")];
                    newQuestion.ShuffleAnswers();
                    qsToSave.Add(newQuestion);
                }
            }
            // If merging with existing note
            if (buttonClicked == "Merge To Existing Note")
            {
                int noteToUpdateId = int.Parse(inputs["existingNotes"]);
                Note noteToUpdate = _context.Notes.Where(note => note.Id == noteToUpdateId).FirstOrDefault();
                foreach (ExamQuestion q in qsToSave)
                {
                    q.ParentNote = noteToUpdate;
                    _context.ExamQuestions.Add(q);
                }
                documentText.parentNote = noteToUpdate;
                _context.DocumentTexts.Add(documentText);
            }
            // If creating new note
            else
            {
                Note noteToSave = new Note();
                noteToSave.Title = inputs["title"];
                noteToSave.Description = inputs["description"];
                noteToSave.ExamQuestions = qsToSave;
                noteToSave.UserId = user;
                noteToSave.CreatedDate = DateTime.Now;
                string isPublic = inputs["publicity"];
                if (isPublic != null)
                {
                    noteToSave.IsPublic = true;
                }
                else
                {
                    noteToSave.IsPublic = false;
                }
                _context.Notes.Add(noteToSave);
                foreach (var q in qsToSave)
                {
                    q.ParentNote = noteToSave;
                    _context.ExamQuestions.Add(q);
                }
                documentText.parentNote = noteToSave;
                _context.DocumentTexts.Add(documentText);
            }
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
