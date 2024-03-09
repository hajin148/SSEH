using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ResultExamModel : PageModel
    {
        [BindProperty]
        public string UserAnswers { get; set; }
        public Dictionary<int, string> UserAnswersObj { get; set; }
        public Note CurrentNote { get; set; }
        public List<ExamQuestion> Questions { get; set; }
        public DocumentText doc { get; set; }
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public string? noteIdString { get; set; }
        [BindProperty]
        public string? numberMCQString { get; set; }
        [BindProperty]
        public string? numberShortString { get; set; }
        [BindProperty]
        public string? numberTFString { get; set; }
        [BindProperty]
        public int? noteId { get; set; }
        [BindProperty]
        public int? numberMCQ { get; set; }
        [BindProperty]
        public int? numberShort { get; set; }
        [BindProperty]
        public int? numberTF { get; set; }

        public int? totalNumberQuestion { get; set; }
        public int correctAnswers = 0;
        public string userId { get; set; }

        public ResultExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            
        }

        public IActionResult OnPost()
        {
            noteIdString = JsonConvert.DeserializeObject<string>(noteIdString);
            noteId = int.Parse(noteIdString);

            numberMCQString = JsonConvert.DeserializeObject<string>(numberMCQString);
            numberShortString = JsonConvert.DeserializeObject<string>(numberShortString);
            numberTFString = JsonConvert.DeserializeObject<string>(numberTFString);

            numberMCQ = int.Parse(numberMCQString);
            numberShort = int.Parse(numberShortString);
            numberTF = int.Parse(numberTFString);

            totalNumberQuestion = numberMCQ + numberShort + numberTF;

            if (!string.IsNullOrWhiteSpace(UserAnswers))
            {
                UserAnswersObj = JsonConvert.DeserializeObject<Dictionary<int, string>>(UserAnswers);
                
            }
            else
            {
                UserAnswersObj = new Dictionary<int, string>();
            }

           

           

            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            CurrentNote = _context.Notes.Where(note => note.Id == noteId).FirstOrDefault();
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();
            userId = _userManager.GetUserId(User);
            var firstQuestion = Questions.FirstOrDefault();
            int firstQNum = firstQuestion.Id;
            int index = 0;
            foreach (var answer in UserAnswersObj)
            {
                if(index > numberMCQ)
                {
                    break;
                }
                var questionNum = answer.Key;
                var userAnswer = answer.Value;

                CheckMCQAnswers(Questions, questionNum, userAnswer, firstQNum);
                index++;
            }


            return Page();
        }

        public void CheckMCQAnswers(List<ExamQuestion> eq, int questionNum, string userAnswer, int firstQNum)
        {
            var question = eq.FirstOrDefault(q => q.Id == questionNum + firstQNum - 1);
            if (question != null && questionNum <= numberMCQ)
            {
                string chosenAnswer = ConvertUserAnswerToActualAnswer(question, userAnswer);

                if (chosenAnswer != null && chosenAnswer.Equals(question.Answer, StringComparison.OrdinalIgnoreCase))
                {
                    correctAnswers++;
                }
            }
        }


        private string ConvertUserAnswerToActualAnswer(ExamQuestion question, string userAnswer)
        {
            switch (userAnswer)
            {
                case "1":
                    return question.AnswerA;
                case "2":
                    return question.AnswerB;
                case "3":
                    return question.AnswerC;
                case "4":
                    return question.AnswerD;
                default:
                    return null; 
            }
        }

    }

    

}


