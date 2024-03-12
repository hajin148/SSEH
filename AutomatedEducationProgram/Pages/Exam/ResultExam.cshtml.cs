using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Humanizer;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Build.Framework;
using System.Net.Http.Headers;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ResultExamModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        [BindProperty]
        public string UserAnswers { get; set; }
        public List<string> MessagesShort { get; set; } = new List<string>();
        public Dictionary<int, string> UserAnswersObj { get; set; }
        public Note CurrentNote { get; set; }
        public List<ExamQuestion> Questions { get; set; }
        public String[] questionArray { get; set; }
        public List<int> quesitonNumbers { get; set; }
        public String[] answers { get; set; }
        public String[] correctOrWrong { get; set; }
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
        public int numberMCQ { get; set; }
        [BindProperty]
        public int numberShort { get; set; }
        [BindProperty]
        public int numberTF { get; set; }
        public int totalNumberQuestion { get; set; }
        public int correctAnswers = 0;
        public string userId { get; set; }

        public ResultExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _config = configuration;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(240);

            quesitonNumbers = new List<int>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            noteIdString = JsonConvert.DeserializeObject<string>(noteIdString);
            noteId = int.Parse(noteIdString);

            numberMCQString = JsonConvert.DeserializeObject<string>(numberMCQString);
            numberShortString = JsonConvert.DeserializeObject<string>(numberShortString);
            numberTFString = JsonConvert.DeserializeObject<string>(numberTFString);

            numberMCQ = int.Parse(numberMCQString);
            numberShort = int.Parse(numberShortString);
            numberTF = int.Parse(numberTFString);
            List<Task> checkAnswerTasks = new List<Task>();

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

            correctOrWrong = new String[Questions.Count];
            questionArray = new String[Questions.Count];
            foreach (var question in Questions)
            {
                questionArray[index] = question.Question;
                index++;
            }

            index = 0;

            answers = new String[Questions.Count];

            foreach (var answer in UserAnswersObj)
            {
                var questionNum = answer.Key;
                var userAnswer = answer.Value;
                quesitonNumbers.Add(questionNum);
                //Handling Short Check
                if (index >= numberMCQ && index < numberMCQ + numberShort)
                {
                    checkAnswerTasks.Add(CheckShortAnswers(Questions, questionNum, userAnswer, firstQNum));
                }
                //Handling MCQ Check
                if (index < numberMCQ)
                {
                    answers[index] = ConvertUserAnswerToActualAnswer(Questions.FirstOrDefault(q => q.Id == questionNum + firstQNum - 1), userAnswer);
                    CheckMCQAnswers(Questions, questionNum, userAnswer, firstQNum);
                }
                //Handling TF Check
                else if (index >= numberMCQ + numberShort && index < numberMCQ + numberShort + numberTF)
                {
                    CheckTFAnswers(Questions, questionNum, userAnswer, firstQNum);
                } 
                
                
                index++;
            }

            await Task.WhenAll(checkAnswerTasks);




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
                    correctOrWrong[questionNum - 1] = "Correct";
                    correctAnswers++;
                }
                else
                {
                    correctOrWrong[questionNum - 1] = "Wrong";
                }
            }
        }

        public async Task CheckShortAnswers(List<ExamQuestion> eq, int questionNum, string userAnswer, int firstQNum)
        {
            if (userAnswer.Equals("Not Answered"))
            {
                correctOrWrong[questionNum - 1] = "Wrong";
            }
            else
            {
                var question = eq.FirstOrDefault(q => q.Id == questionNum + firstQNum - 1);
                if (question != null)
                {
                    string Q = questionArray[questionNum - 1];
                    string qnaPrompt = $"The question is \"{Q}\" and the user answer is \"{userAnswer}\". Is it Correct or Wrong? Please give response surrounded by square brackets and : at the end of square brackets followed by answer e.g. \"[Correct|Wrong]: Explanation why user answer is Correct or Wrong\".";

                    var apiResponse = await SendMessage2(qnaPrompt);
                    

                    if (apiResponse.Equals("Correct", StringComparison.OrdinalIgnoreCase))
                    {
                        correctOrWrong[questionNum - 1] = "Correct";
                        correctAnswers++;
                    }
                    else
                    {
                        correctOrWrong[questionNum - 1] = "Wrong";
                    }
                }
            }
        }

        private async Task<string> SendMessage2(string message)
        {
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
            new { role = "system", content = "You are a helpful assistant to check Short-Answer questions and user answers." },
            new { role = "user", content = message }
        }
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"OpenAI API call failed with status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            string lastMessage = data.choices[0].message.content.ToString();
            lastMessage = ParseApiResponse(lastMessage);
            return lastMessage;
        }

        public static string ParseApiResponse(string response)
        {

            var match = Regex.Match(response, @"\[(Correct|Wrong)\]:\s*(.*)");

            if (match.Success)
            {
                var status = match.Groups[1].Value; 
                var explanation = match.Groups[2].Value; 
                return status;
            }
            else
            {
                return "Unable to determine"; 
            }
        }




        public void CheckTFAnswers(List<ExamQuestion> eq, int questionNum, string userAnswer, int firstQNum)
        {
            var question = eq.FirstOrDefault(q => q.Id == questionNum + firstQNum - 1);
            if (question != null && (numberMCQ + numberShort) <= questionNum && questionNum <= (numberMCQ + numberShort + numberTF))
            {

                if (userAnswer != null && userAnswer.Equals(question.Answer, StringComparison.OrdinalIgnoreCase))
                {
                    correctOrWrong[questionNum - 1] = "Correct";
                    correctAnswers++;
                }
                else
                {
                    correctOrWrong[questionNum - 1] = "Wrong";
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


