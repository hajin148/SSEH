using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class FlashcardModel : PageModel
    {
        public List<VocabularyWord> ProcessedVocabulary { get; set; }
        public string text { get; set; }

        public void OnGet()
        {
            var vocabularyJson = HttpContext.Session.GetString("ProcessedVocabulary");
            var textJson = HttpContext.Session.GetString("Text");
            if (!string.IsNullOrEmpty(vocabularyJson))
            {
                ProcessedVocabulary = JsonConvert.DeserializeObject<List<VocabularyWord>>(vocabularyJson);
                text = JsonConvert.DeserializeObject<string>(textJson);
            }

            // ProcessedVocabulary should be feed up with the received Vocab from the db.
        }

        public IActionResult OnPostAsync()
        {
            var vocabularyJson = JsonConvert.SerializeObject(ProcessedVocabulary);
            HttpContext.Session.SetString("ProcessedVocabulary", vocabularyJson);

            var textJson = JsonConvert.SerializeObject(text);
            HttpContext.Session.SetString("Text", textJson);

            return Page();
        }
    }
}

