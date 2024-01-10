using EduApp;
using Microsoft.AspNetCore.Mvc;

namespace AutomatedEducationProgram.Controllers
{
    public class VocabularyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SaveVocabulary(List<VocabularyWord> words)
        {
            return View(words);
        }
    }
}
