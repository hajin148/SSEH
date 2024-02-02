using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class MyNotesModel : PageModel
    {
        public IEnumerable<Note> UserNotes { get; set; }
        public IEnumerable<DocumentText> UserDocs { get; set; }

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public MyNotesModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            string user = _userManager.GetUserId(User);
            if (user == null)
            {
                return Redirect("https://localhost:7039/Identity/Account/Login");
            }
            string userId = _userManager.GetUserId(User);
            UserNotes = _context.Notes.Where(note => note.UserId == userId).ToList();
            UserDocs = _context.DocumentTexts.Where(dtext => dtext.UserId == userId).ToList();

            return Page();
        }
    }
}
