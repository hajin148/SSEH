using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class DeleteDocModel : PageModel
    {
        public DocumentText CurrentDoc;
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public DeleteDocModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public void OnGet(int? docId)
        {
            CurrentDoc = _context.DocumentTexts.Where(dtext => dtext.Id == docId).FirstOrDefault();

        }

        public IActionResult OnPost(int? docId, IFormCollection inputs)
        {
            DocumentText toDelete = _context.DocumentTexts.Where(dtext => dtext.Id == docId).FirstOrDefault();
            _context.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
