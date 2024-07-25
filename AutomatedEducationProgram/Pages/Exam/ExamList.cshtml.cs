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
using Microsoft.AspNetCore.Identity;
using AutomatedEducationProgram.Areas.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ExamListModel : PageModel
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
        private readonly ILogger<ExamListModel> _logger;

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

        public ExamListModel(IConfiguration config, IHttpClientFactory httpClientFactory, UserManager<AEPUser> userManager, IOptions<GeminiOptions> geminiOptions, ILogger<ExamListModel> logger)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
            _userManager = userManager;
            _geminiOptions = geminiOptions.Value;
            _logger = logger;
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

            text = VocabularyReader.ReadWordsFromFile(tempFilePath);

            List<Task> tasks = new List<Task>();

            if (numMCQ > 0)
            {
                string mcqPrompt = $"Please read the following passage of text and give me {numMCQ} multiple-choice questions with exactly 4 answer choices each. Ensure the first answer choice is correct and the remaining three are incorrect. Each Question should be surrounded by square brackets and : at the end of square brackets followed by answer choices e.g. \"[question]: answer choices\". Please don't add any additional formatting other than \"[question]: 4 answer choices\" in your response. \n\n";
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
                },
                generationConfig = new
                {
                    temperature = 0,
                    top_k = 1,
                    top_p = 1,
                    max_output_tokens = 2048,
                    stop_sequences = new List<object>()
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
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


                MessagesMCQ.Add(generatedText);
                var parsedTermsMCQ = ParseTermsAndDefsMCQ(generatedText, "]: ", ", [");

                GeneratedQuestionsMCQ.AddRange(parsedTermsMCQ);

                List<String> MCQAnswers = new List<String>();
                foreach (var answers in parsedTermsMCQ)
                {
                    MCQAnswers.AddRange(VocabularyReader.ParseOptions(answers.Answer));
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message to Gemini API");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return Page();
            }
        }

        public static List<ExamQuestion> ParseTermsAndDefsMCQ(string text, string termDelimiter, string optionDelimiter)
        {
            var questions = new List<ExamQuestion>();
            var parts = text.Split(new[] { "\n[" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) continue;

                var questionParts = part.Split(new[] { "]:\n" }, StringSplitOptions.None);
                if (questionParts.Length == 2)
                {
                    var questionText = questionParts[0].Trim('[', ']');
                    var answerOptions = questionParts[1].Split(new[] { "\n- " }, StringSplitOptions.None);

                    if (answerOptions.Length == 4)
                    {
                        // Remove the hyphen from the first answer option
                        answerOptions[0] = answerOptions[0].TrimStart('-').Trim();

                        var question = new ExamQuestion
                        {
                            Question = questionText,
                            AnswerA = answerOptions[0].Trim(),
                            AnswerB = answerOptions[1].Trim(),
                            AnswerC = answerOptions[2].Trim(),
                            AnswerD = answerOptions[3].Trim(),
                            Answer = answerOptions[0].Trim() // Assuming the first option is the correct answer for simplicity
                        };
                        questions.Add(question);
                    }
                }
            }

            return questions;
        }

        private async Task<IActionResult> SendMessage2(string message)
        {
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
                },
                generationConfig = new
                {
                    temperature = 0,
                    top_k = 1,
                    top_p = 1,
                    max_output_tokens = 2048,
                    stop_sequences = new List<object>()
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
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

                MessagesShort.Add(generatedText);
                var parsedTermsShort = VocabularyReader.ParseTermsAndDefs2(generatedText, "]: ", ", [");
                GeneratedQuestionsShort.AddRange(parsedTermsShort);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message to Gemini API");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return Page();
            }
        }

        private async Task<IActionResult> SendMessage3(string message)
        {
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
                },
                generationConfig = new
                {
                    temperature = 0,
                    top_k = 1,
                    top_p = 1,
                    max_output_tokens = 2048,
                    stop_sequences = new List<object>()
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
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

                MessagesTF.Add(generatedText);
                var parsedTermsTF = VocabularyReader.ParseTermsAndDefs3(generatedText, "]: ", ", [");
                GeneratedQuestionsTF.AddRange(parsedTermsTF);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message to Gemini API");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return Page();
            }
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
