using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class VocabularyList : PageModel
    {
        private readonly HttpClient _httpClient;
        public string Message { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

        public VocabularyList(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync(string message)
        {
            Messages.Add(message);

            // Replace with your API key and adjust the API request as needed
            var apiKey = "Your_API_Key";
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var jsonContent = new
            {
                prompt = message,
                model = "text-davinci-003",
                max_tokens = 1000
            };

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

            Messages.Add(data.choices[0].text.ToString());
            return Page();
        }
    }
}
