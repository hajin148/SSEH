using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.SearchNote
{
    public class PostsModel : PageModel
    {

        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;
        public string Username {  get; set; }
        public List<Note> Notes { get; set; }
        public string Major { get; set; }
        

        public PostsModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task OnGetAsync(string? userId)
        {
            AEPUser currentUser = await _userManager.FindByIdAsync(userId);
            Notes = _context.Notes.Where(note => note.UserId == userId).ToList();
            Username = currentUser.UserID;
            Major = currentUser.Major;

        }
    }
}
