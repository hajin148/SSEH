using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ExamDetailModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public int? noteId { get; set; }
        [BindProperty]
        public string? noteIdString { get; set; }
        [BindProperty]
        public string? numberMCQString { get; set; }
        [BindProperty]
        public string? numberShortString { get; set; }
        [BindProperty]
        public string? numberTFString { get; set; }
        [BindProperty]
        public string? currentQString { get; set; }
        [BindProperty]
        public string UserAnswerString { get; set; }
        [BindProperty]
        public string questionString { get; set; }
        public Note CurrentNote { get; set; }
        public List<ExamQuestion> Questions { get; set; }
        [BindProperty]
        public DocumentText doc { get; set; }
        public string answerA { get; set; }
        public string answerB { get; set; }
        public string answerC { get; set; }
        public string answerD { get; set; }
        public string correctAnswer { get; set; }
        public string nextAnswer { get; set; }
        public string prevAnswer { get; set; }
        public string nextQuestion { get; set; }
        public string prevQuestion { get; set; }

        public int numberMCQ { get; set; }
        public int numberShort { get; set; }
        public int numberTF { get; set; }
        public int currentQ { get; set; }
        public Dictionary<int, string> UserAnswersObj { get; set; }
        [BindProperty]
        public String[] UserAnswers { get; set; }


        public ExamDetailModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _config = configuration;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(240);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            numberMCQString = JsonConvert.DeserializeObject<string>(numberMCQString);
            numberShortString = JsonConvert.DeserializeObject<string>(numberShortString);
            numberTFString = JsonConvert.DeserializeObject<string>(numberTFString);
            currentQString = JsonConvert.DeserializeObject<string>(currentQString);
            noteIdString = JsonConvert.DeserializeObject<string>(noteIdString);
            //UserAnswers = JsonConvert.DeserializeObject<string>(UserAnswers);
            noteId = int.Parse(noteIdString);


            numberMCQ = int.Parse(numberMCQString);
            numberShort = int.Parse(numberShortString);
            numberTF = int.Parse(numberTFString);
            currentQ = int.Parse(currentQString);


            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            CurrentNote = _context.Notes.FirstOrDefault(note => note.Id == noteId);
            Questions = _context.ExamQuestions.Where(q => q.ParentNote.Id == noteId).ToList();
            doc = _context.DocumentTexts.FirstOrDefault(dtext => dtext.parentNote.Id == noteId);


            if (currentQ < numberMCQ)
            {
                answerA = Questions[currentQ].AnswerA;
                answerB = Questions[currentQ].AnswerB;
                answerC = Questions[currentQ].AnswerC;
                answerD = Questions[currentQ].AnswerD;
                correctAnswer = Questions[currentQ].Answer;
            }

            if (currentQ < numberMCQ + numberTF + numberShort - 1)
            {
                nextQuestion = Questions[currentQ + 1].Question;
            }

            if (currentQ > 0)
            {
                prevQuestion = Questions[currentQ - 1].Question;

            }
            return Page();
        }

    }
}
