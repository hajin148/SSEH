using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using AutomatedEducationProgram.Models;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class VocabularyList : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public string text { get; set; }
        [BindProperty]
        public int numQuestion { get; set; }
        public string Message { get; set; }
        public string prompt { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public List<VocabularyWord> ProcessedVocabulary { get; set; } = new List<VocabularyWord>();
        public List<DocumentText> UsersTexts { get; set; }


        [BindProperty]
        public IFormFile Upload { get; set; }

        public VocabularyList(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }


        public void OnGet()
        {

            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            var textJson = HttpContext.Session.GetString("Text");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
                text = JsonConvert.DeserializeObject<string>(textJson);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var fileExtension = Path.GetExtension(Upload.FileName).ToLowerInvariant();
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + fileExtension);

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await Upload.CopyToAsync(stream);
            }
            if (numQuestion <= 0)
            {
                Message = "Please enter a valid number of vocabulary words.";
                return Page();
            }

            text = VocabularyReader.ReadWordsFromFile(tempFilePath);

            prompt = $"Please read the following passage of text and give me {numQuestion} most important vocabulary terms from the text and their definitions. Each vocabulary should be surrounded by square brackets and : at the end of square brackets followed by definition e.g. \"[Term]: Definition\". Please don't add any additional formatting other than \"[Term]: Definition\" in your response. \n\n";

            await SendMessage($"{prompt} {text}");

            System.IO.File.Delete(tempFilePath);

            var vocabularyJson = JsonConvert.SerializeObject(ProcessedVocabulary);
            HttpContext.Session.SetString("ProcessedVocabulary", vocabularyJson);

            var textJson = JsonConvert.SerializeObject(text);
            HttpContext.Session.SetString("Text", textJson);

            return RedirectToPage();
        }

        private async Task<IActionResult> SendMessage(string message)
        {
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "system", content = "You are a helpful assistant to generate important vocabualires from text." }, new { role = "user", content = message } }
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            Messages.Add(data.choices[0].message.content.ToString());
            foreach (var msg in Messages)
            {
                var parsedTerms = VocabularyReader.ParseTermsAndDefs(msg, "]: ", ", [");
                ProcessedVocabulary.AddRange(parsedTerms);
                // Pushing processed data to database or handling it as required
            }


            if (!string.IsNullOrEmpty(text))
            {
                foreach (var vocab in ProcessedVocabulary)
                {
                    var term = vocab.Term;
                    var replacement = $"<span style=\"font-weight: bold; text-decoration: underline;\">{term}</span>";
                    text = Regex.Replace(text, Regex.Escape(term), replacement, RegexOptions.IgnoreCase);
                }
            }

            return RedirectToPage();
        }
    }
}