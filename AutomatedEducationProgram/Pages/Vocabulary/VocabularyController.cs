using AutomatedEducationProgram.Models;
using EduApp;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AutomatedEducationProgram.Pages.Vocabulary
{
    public class VocabularyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveTheNote(IFormCollection inputs)
        {
            List<VocabularyWord> wordsToSave = new List<VocabularyWord>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("vocabTerm"))
                {
                    string term = inputs[key];
                    string defKey = key.Replace("Term", "Def");
                    string def = inputs[defKey];
                    wordsToSave.Add(new VocabularyWord(term, def)); 
                }
            }
            Note noteToSave = new Note();
            noteToSave.Title = inputs["title"];
            noteToSave.Description = inputs["description"];
            noteToSave.VocabularyWords = wordsToSave;
            return View();
        }
    }
}
