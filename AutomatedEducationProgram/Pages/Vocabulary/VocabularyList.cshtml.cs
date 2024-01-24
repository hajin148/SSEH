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
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Areas.Data;
using Microsoft.AspNetCore.Identity;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class VocabularyList : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        public string text { get; set; }
        public string Message { get; set; }
        public string prompt = "Please read the following passage of text and give me the 5 most important vocabulary terms from the text and their definitions. Each vocabularies should be surrounded by square brackets and : at the end of square brackets followed by definition e.g. [vocabs]: definitions . Please don't add any additional formatting or text.\n\n";
        public List<string> Messages { get; set; } = new List<string>();
        public List<VocabularyWord> ProcessedVocabulary { get; set; } = new List<VocabularyWord>();
        public List<DocumentText> UsersTexts { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; }

        public VocabularyList(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration config, HttpClient httpClient)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _httpClient = httpClient;
        }


        public void OnGet()
        {
            string userId = _userManager.GetUserId(User);
            UsersTexts = _context.DocumentTexts.Where(dtext => dtext.UserId == userId).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string docId)
        {
            if (Upload != null || docId != null)
            {
                if (docId != null)
                {
                    text = _context.DocumentTexts.Where(dtext => dtext.Id == int.Parse(docId)).FirstOrDefault().Text;

                    await SendMessage($"{prompt} {text}");
                }
                else
                {
                    var fileExtension = Path.GetExtension(Upload.FileName).ToLowerInvariant();

                    var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + fileExtension);

                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await Upload.CopyToAsync(stream);
                    }

                    text = VocabularyReader.ReadWordsFromFile(tempFilePath);

                    await SendMessage($"{prompt} {text}");

                    System.IO.File.Delete(tempFilePath);

                    if (text != null)
                    {
                        string userId = _userManager.GetUserId(User);
                        DocumentText newText = new DocumentText(userId, text);
                        _context.DocumentTexts.Add(newText);
                        _context.SaveChanges();
                    }
                }

            }

            var vocabularyJson = JsonConvert.SerializeObject(ProcessedVocabulary);
            HttpContext.Session.SetString("ProcessedVocabulary", vocabularyJson);

            return Page();
        }

        private async Task<IActionResult> SendMessage(string message)
        {
            // Replace with your actual API key
            var apiKey = _config["apiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                prompt = message,
                model = "gpt-3.5-turbo-instruct",
                max_tokens = 2000
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            Messages.Add(data.choices[0].text.ToString());
            foreach (var msg in Messages)
            {
                var parsedTerms = VocabularyReader.ParseTermsAndDefs(msg, "]: ", ", [");
                ProcessedVocabulary.AddRange(parsedTerms);
                // pushing ProcessedVocabulary to database + text

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

            return Page();
        }
    }
}
