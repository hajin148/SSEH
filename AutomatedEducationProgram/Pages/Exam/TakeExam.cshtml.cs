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
        public List<ExamQuestion> GeneratedQuestionsMCQ { get; set; }
        public List<ExamQuestion> GeneratedQuestionsShort { get; set; }
        public List<ExamQuestion> GeneratedQuestionsTF { get; set; }
        public List<Note> ExistingNotes { get; set; }
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public int currIndex { get; set; }
        [BindProperty]
        public int totalNumberQuestions { get; set; }
        public TakeExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            
        }

        public IActionResult OnGet()
        {
            currIndex = 1;
            totalNumberQuestions = 10;


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
                    //TotalNumberQuestions++;
                    q.QuestionType = ExamQuestion.MULTIPLE_CHOICE_QUESTION;
                }
            }

            if (!string.IsNullOrEmpty(shortJson))
            {
                GeneratedQuestionsShort = JsonConvert.DeserializeObject<List<ExamQuestion>>(shortJson);
                foreach (ExamQuestion q in GeneratedQuestionsShort)
                {
                    //TotalNumberQuestions++;
                    q.QuestionType = ExamQuestion.SHORT_ANSWER_QUESTION;
                }
            }

            if (!string.IsNullOrEmpty(tfJson))
            {
                GeneratedQuestionsTF = JsonConvert.DeserializeObject<List<ExamQuestion>>(tfJson);
                foreach (ExamQuestion q in GeneratedQuestionsTF)
                {
                    //TotalNumberQuestions++;
                    q.QuestionType = ExamQuestion.TF_QUESTION;
                }
            }

            ExistingNotes = _context.Notes.Where(note => note.UserId == user).ToList();

            return Page();

        }

        public async Task<IActionResult> OnPostSaveIndexAsync([FromBody] IndexModel model)
        {
            TempData["currIndex"] = currIndex;

            return new JsonResult(new { success = true });
        }


    }
}