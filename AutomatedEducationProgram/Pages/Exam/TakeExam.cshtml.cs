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

namespace AutomatedEducationProgram.Pages.Exam
{
    public class TakeExamModel : PageModel
    {
        public List<String> GeneratedQuestionsMCQ { get; set; }
        public List<String> GeneratedQuestionsShort { get; set; }
        public List<String> GeneratedQuestionsTF { get; set; }

        public List<String> GeneratedAnswersMCQ { get; set; }
        public List<String> GeneratedCorrectAnswersMCQ { get; set; }
        public List<String> GeneratedAnswersShort { get; set; }
        public List<String> GeneratedAnswersTF { get; set; }
        public Note CurrentNote { get; set; }
        public List<ExamQuestion> Questions { get; set; }
        [BindProperty]
        public DocumentText doc { get; set; }

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public int? noteId { get; set; }
        [BindProperty]
        public int currIndex { get; set; }
        [BindProperty]
        public int totalNumberQuestions { get; set; }
        public TakeExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            doc = new DocumentText();

        }

        public IActionResult OnGet(int? noteId)
        {
            GeneratedQuestionsMCQ = new List<String>();
            GeneratedQuestionsShort = new List<String>();
            GeneratedQuestionsTF = new List<String>();
            GeneratedAnswersMCQ = new List<String>();
            GeneratedAnswersShort = new List<String>();
            GeneratedAnswersTF = new List<String>();
            GeneratedCorrectAnswersMCQ = new List<String>();

            this.noteId = noteId;

            currIndex = 1;

            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            CurrentNote = _context.Notes.Where(note => note.Id == noteId).FirstOrDefault();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();
            doc = _context.DocumentTexts.Where(dtext => dtext.parentNote.Id == noteId).FirstOrDefault();
            totalNumberQuestions = Questions.Count;





            foreach (ExamQuestion question in Questions)
            {
                if (question.QuestionType == 0 && question != null)
                {
                    GeneratedQuestionsTF.Add(question.Question);
                    GeneratedAnswersTF.Add(question.Answer);
                }
                else if (question.QuestionType == 1 && question != null)
                {
                    GeneratedQuestionsShort.Add(question.Question);
                    GeneratedAnswersShort.Add(question.Answer);
                }
                else if (question.QuestionType == 2 && question != null)
                {
                    GeneratedQuestionsMCQ.Add(question.Question);
                    GeneratedAnswersMCQ.Add(question.AnswerA);
                    GeneratedAnswersMCQ.Add(question.AnswerB);
                    GeneratedAnswersMCQ.Add(question.AnswerC);
                    GeneratedAnswersMCQ.Add(question.AnswerD);
                    GeneratedCorrectAnswersMCQ.Add(question.Answer);
                }
                else
                {

                }
            }

            return Page();

        }

    }
}