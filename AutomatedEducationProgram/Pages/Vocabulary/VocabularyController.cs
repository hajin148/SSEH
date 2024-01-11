using EduApp;
using Microsoft.AspNetCore.Mvc;

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
            int i = 0;
            i += 1;
            i += 2;
            i += 3;
            return View();
        }
    }
}
