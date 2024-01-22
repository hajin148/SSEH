using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class SaveVocabularyModel : PageModel
    {
        public List<VocabularyWord> ProcessedVocabulary { get; set; }

        public void OnGet()
        {
            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
            }
        }
    }
}
