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
using Microsoft.AspNetCore.Identity;
using AutomatedEducationProgram.Areas.Data;
using Microsoft.Extensions.Options;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class VocabularyList : PageModel
    {

        public sealed class GeminiOptions
        {
            public string ApiKey { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }

        public sealed class GeminiRequest
        {
            public GeminiContent[] Contents { get; set; }
            public GenerationConfig GenerationConfig { get; set; }
            public SafetySetting[] SafetySettings { get; set; }
        }

        public sealed class GeminiContent
        {
            public string Role { get; set; }
            public GeminiPart[] Parts { get; set; }
        }

        public sealed class GeminiPart
        {
            public string Text { get; set; }
        }

        public sealed class GenerationConfig
        {
            public int Temperature { get; set; }
            public int TopK { get; set; }
            public int TopP { get; set; }
            public int MaxOutputTokens { get; set; }
            public List<object> StopSequences { get; set; }
        }

        public sealed class SafetySetting
        {
            public string Category { get; set; }
            public string Threshold { get; set; }
        }

        public sealed class GeminiResponse
        {
            public Candidate[] Candidates { get; set; }
            public PromptFeedback PromptFeedback { get; set; }
        }

        public sealed class PromptFeedback
        {
            public SafetyRating[] SafetyRatings { get; set; }
        }

        public sealed class Candidate
        {
            public Content Content { get; set; }
            public string FinishReason { get; set; }
            public int Index { get; set; }
            public SafetyRating[] SafetyRatings { get; set; }
        }

        public sealed class Content
        {
            public Part[] Parts { get; set; }
            public string Role { get; set; }
        }

        public sealed class Part
        {
            public string Text { get; set; }
        }

        public sealed class SafetyRating
        {
            public string Category { get; set; }
            public string Probability { get; set; }
        }


        public sealed class GeminiRequestFactory
        {
            public static GeminiRequest CreateRequest(string prompt)
            {
                return new GeminiRequest
                {
                    Contents = new[]
                    {
                new GeminiContent
                {
                    Role = "user",
                    Parts = new[]
                    {
                        new GeminiPart { Text = prompt }
                    }
                }
            },
                    GenerationConfig = new GenerationConfig
                    {
                        Temperature = 0,
                        TopK = 1,
                        TopP = 1,
                        MaxOutputTokens = 2048,
                        StopSequences = new List<object>()
                    },
                    SafetySettings = new[]
                    {
                new SafetySetting { Category = "HARM_CATEGORY_HARASSMENT", Threshold = "BLOCK_ONLY_HIGH" },
                new SafetySetting { Category = "HARM_CATEGORY_HATE_SPEECH", Threshold = "BLOCK_ONLY_HIGH" },
                new SafetySetting { Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", Threshold = "BLOCK_ONLY_HIGH" },
                new SafetySetting { Category = "HARM_CATEGORY_DANGEROUS_CONTENT", Threshold = "BLOCK_ONLY_HIGH" }
            }
                };
            }
        }




        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly UserManager<AEPUser> _userManager;
        private readonly GeminiOptions _geminiOptions;
        private readonly ILogger<VocabularyList> _logger; 

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

        public VocabularyList(IConfiguration config, IHttpClientFactory httpClientFactory, UserManager<AEPUser> userManager, IOptions<GeminiOptions> geminiOptions, ILogger<VocabularyList> logger)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
            _userManager = userManager;
            _geminiOptions = geminiOptions.Value;
            _logger = logger;

            _logger.LogInformation($"Gemini API Key: {_geminiOptions.ApiKey}");
            _logger.LogInformation($"Gemini URL: {_geminiOptions.Url}");
        }

        public IActionResult OnGet()
        {
            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }

            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            var textJson = HttpContext.Session.GetString("Text");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
                text = JsonConvert.DeserializeObject<string>(textJson);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload == null)
            {
                return Page();
            }

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
            if (_geminiOptions == null || string.IsNullOrEmpty(_geminiOptions.ApiKey) || string.IsNullOrEmpty(_geminiOptions.Url))
            {
                _logger.LogError("Gemini API configuration is missing or invalid.");
                throw new InvalidOperationException("Gemini API configuration is missing or invalid.");
            }

            if (_httpClient == null)
            {
                _logger.LogError("HttpClient is not initialized.");
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var requestUrl = $"{_geminiOptions.Url}?key={_geminiOptions.ApiKey}";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = message }
                }
            }
        }
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            _logger.LogInformation($"Request URL: {requestUrl}");
            _logger.LogInformation($"Request Payload: {JsonConvert.SerializeObject(requestBody, Formatting.Indented)}");
            _logger.LogInformation($"Using API Key: {_geminiOptions.ApiKey}");

            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Response Status Code: {response.StatusCode}");
                _logger.LogInformation($"Response Content: {responseContent}");

                response.EnsureSuccessStatusCode();

                var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseContent);
                if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Length == 0)
                {
                    _logger.LogError("Invalid response from Gemini API.");
                    throw new InvalidOperationException("Invalid response from Gemini API.");
                }

                var generatedText = geminiResponse.Candidates[0]?.Content?.Parts?[0]?.Text;
                if (string.IsNullOrEmpty(generatedText))
                {
                    _logger.LogError("Generated text is empty.");
                    throw new InvalidOperationException("Generated text is empty.");
                }

                Messages.Add(generatedText);

                foreach (var msg in Messages)
                {
                    var parsedTerms = VocabularyReader.ParseTermsAndDefs(msg, "]: ", ", [");
                    ProcessedVocabulary.AddRange(parsedTerms);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message to Gemini API");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return Page();
            }
        }




        public IActionResult OnPostResetVocabulary()
        {
            ProcessedVocabulary = new List<VocabularyWord>();
            HttpContext.Session.Remove("ProcessedVocabulary");
            return new JsonResult(new { success = true });
        }
    }
}