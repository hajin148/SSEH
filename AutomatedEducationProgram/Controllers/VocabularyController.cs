using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using EduApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AutomatedEducationProgram.Controllers
{
    public class VocabularyController : Controller
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public VocabularyController(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }


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
            AEPUser user = await _userManager.GetUserAsync(User);
            noteToSave.Title = inputs["title"];
            noteToSave.Description = inputs["description"];
            noteToSave.VocabularyWords = wordsToSave;
            noteToSave.UserId = user.Id;
            noteToSave.CreatedDate = DateTime.Now;
            _context.Notes.Add(noteToSave);
            foreach (var word in wordsToSave)
            {
                word.ParentNote = noteToSave;
                _context.VocabularyWords.Add(word);
            }
            _context.SaveChanges();
            return View("~/Pages/Index.cshtml");
        }
    }
}
