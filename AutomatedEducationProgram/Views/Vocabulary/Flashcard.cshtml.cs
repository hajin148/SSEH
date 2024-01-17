using EduApp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class FlashcardModel : PageModel
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
