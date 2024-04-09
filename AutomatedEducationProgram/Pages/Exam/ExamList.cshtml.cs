using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using AutomatedEducationProgram.Areas.Data;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ExamListModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly UserManager<AEPUser> _userManager;
        public string text { get; set; }
        [BindProperty]
        public int numShortAnswer { get; set; }
        [BindProperty]
        public int numMCQ { get; set; }
        [BindProperty]
        public int numTrueFalse { get; set; }
        public string Message { get; set; }
        public string prompt { get; set; }
        public List<string> MessagesMCQ { get; set; } = new List<string>();
        public List<string> MessagesShort { get; set; } = new List<string>();
        public List<string> MessagesTF { get; set; } = new List<string>();
        public List<ExamQuestion> GeneratedQuestionsMCQ { get; set; } = new List<ExamQuestion>();
        public List<ExamQuestion> GeneratedQuestionsShort { get; set; } = new List<ExamQuestion>();
        public List<ExamQuestion> GeneratedQuestionsTF { get; set; } = new List<ExamQuestion>();
        [BindProperty]
        public IFormFile Upload { get; set; }

        public ExamListModel(IConfiguration config, HttpClient httpClient, UserManager<AEPUser> userManager)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(240);
            _userManager = userManager;
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

            var textJson = HttpContext.Session.GetString("Text");
            if (!string.IsNullOrEmpty(mcqJson) || !string.IsNullOrEmpty(shortJson) || !string.IsNullOrEmpty(tfJson))
            {
                text = JsonConvert.DeserializeObject<string>(textJson);
            }

            if (!string.IsNullOrEmpty(mcqJson))
            {
                GeneratedQuestionsMCQ = JsonConvert.DeserializeObject<List<ExamQuestion>>(mcqJson);
            }

            if (!string.IsNullOrEmpty(shortJson))
            {
                GeneratedQuestionsShort = JsonConvert.DeserializeObject<List<ExamQuestion>>(shortJson);
            }

            if (!string.IsNullOrEmpty(tfJson))
            {
                GeneratedQuestionsTF = JsonConvert.DeserializeObject<List<ExamQuestion>>(tfJson);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // if fileExtension is not null need to be included
            var fileExtension = Path.GetExtension(Upload.FileName).ToLowerInvariant();
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + fileExtension);

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await Upload.CopyToAsync(stream);
            }

            text = VocabularyReader.ReadWordsFromFile(tempFilePath);

            List<Task> tasks = new List<Task>();

            if (numMCQ > 0)
            {
                string mcqPrompt = $"Please read the following passage of text and give me {numMCQ} multiple-choice questions with exactly 4 answer choices each. Each Question should be surrounded by square brackets and : at the end of square brackets followed by answer choices e.g. \"[question]: answer choices\". Please don't add any additional formatting other than \"[question]: 4 answer choices\" in your response. \n\n";
                tasks.Add(SendMessage1(mcqPrompt + text));
            }


            if (numShortAnswer > 0)
            {
                string shortAnswerPrompt = $"Please read the following passage of text and give me {numShortAnswer} most important questions from the text and their answers. Each questions should be surrounded by square brackets and : at the end of square brackets followed by answer e.g. \"[question]: answer\". Please don't add any additional formatting other than \"[question]: answer\" in your response. \n\n";
                tasks.Add(SendMessage2(shortAnswerPrompt + text));
            }

            if (numTrueFalse > 0)
            {
                string trueFalsePrompt = $"Please read the following passage of text and give me {numTrueFalse} most important true or false questions with answer each. Each Question should be surrounded by square brackets and : at the end of square brackets followed by answer e.g. \"[question]: answer\". Please don't add any additional formatting other than \"[question]: answer\" in your response.\n\n";
                tasks.Add(SendMessage3(trueFalsePrompt + text));
            }

            await Task.WhenAll(tasks);


            System.IO.File.Delete(tempFilePath);

            var mcqJson = JsonConvert.SerializeObject(GeneratedQuestionsMCQ);
            HttpContext.Session.SetString("MCQQuestion", mcqJson);

            var shortJson = JsonConvert.SerializeObject(GeneratedQuestionsShort);
            HttpContext.Session.SetString("ShortQuestion", shortJson);

            var tfJson = JsonConvert.SerializeObject(GeneratedQuestionsTF);
            HttpContext.Session.SetString("TFQuestion", tfJson);

            var textJson = JsonConvert.SerializeObject(text);
            HttpContext.Session.SetString("Text", textJson);

            return RedirectToPage();
        }


        private async Task<IActionResult> SendMessage1(string message)
        {
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "system", content = "You are a helpful assistant to generate MCQ questions and answers." }, new { role = "user", content = message } }
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            List<String> MCQAnswers = new List<String>();

            MessagesMCQ.Add(data.choices[0].message.content.ToString());
            foreach (var msg in MessagesMCQ)
            {
                var parsedTerms = VocabularyReader.ParseTermsAndDefs2(msg, "]: ", ", [");
                GeneratedQuestionsMCQ.AddRange(parsedTerms);

                if (parsedTerms != null)
                {
                    foreach (var answers in parsedTerms)
                    {
                        MCQAnswers.AddRange(VocabularyReader.ParseOptions(answers.Answer));
                    }
                }
            }
            int index = 0;
            foreach (ExamQuestion e in GeneratedQuestionsMCQ)
            {
                e.AnswerA = MCQAnswers[index];
                index++;
                e.AnswerB = MCQAnswers[index];
                index++;
                e.AnswerC = MCQAnswers[index];
                index++;
                e.AnswerD = MCQAnswers[index];
                index++;
            }


            return Page();
        }

        private async Task<IActionResult> SendMessage2(string message)
        {
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "system", content = "You are a helpful assistant to generate Short-Answer questions and answers." }, new { role = "user", content = message } }
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            MessagesShort.Add(data.choices[0].message.content.ToString());
            foreach (var msg in MessagesShort)
            {
                var parsedTerms = VocabularyReader.ParseTermsAndDefs2(msg, "]: ", ", [");
                GeneratedQuestionsShort.AddRange(parsedTerms);
                // Pushing processed data to database or handling it as required
            }

            return Page();
        }

        private async Task<IActionResult> SendMessage3(string message)
        {
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "system", content = "You are a helpful assistant to generate True/False questions and answers." }, new { role = "user", content = message } }
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            MessagesTF.Add(data.choices[0].message.content.ToString());
            foreach (var msg in MessagesTF)
            {
                var parsedTerms = VocabularyReader.ParseTermsAndDefs3(msg, "]: ", ", [");
                GeneratedQuestionsTF.AddRange(parsedTerms);
                // Pushing processed data to database or handling it as required
            }

            return Page();
        }

        public IActionResult OnPostResetQuestion()
        {
            GeneratedQuestionsMCQ.Clear();
            GeneratedQuestionsShort.Clear();
            GeneratedQuestionsTF.Clear();

            HttpContext.Session.Remove("GeneratedQuestionsMCQ");
            HttpContext.Session.Remove("GeneratedQuestionsShort");
            HttpContext.Session.Remove("GeneratedQuestionsTF");

            HttpContext.Session.Clear();

            return new JsonResult(new { success = true });
        }

    }
}