using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using AutomatedEducationProgram.Views.MyNotes;
using EduApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Controllers
{
    public class MyNotesController : Controller
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public MyNotesController(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> MyNotes()
        {
            string userId = _userManager.GetUserId(User);
            IEnumerable<AutomatedEducationProgram.Models.Note> usersNotes = await _context.Notes.Where(note => note.UserId == userId).ToListAsync();
            return View(usersNotes);
        }

        public async Task<IActionResult> StudyNote(int noteId)
        {
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            return View(vocab);
        }

        public async Task<IActionResult> EditNote(int noteId)
        {
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            return View(vocab);

        }

        public async Task<IActionResult> DeleteNote(int noteId)
        {
            IEnumerable<VocabularyWord> vocab = await _context.VocabularyWords.Where(word => word.ParentNote.Id == noteId).ToListAsync();
            return View(vocab);

        }
    }
}
