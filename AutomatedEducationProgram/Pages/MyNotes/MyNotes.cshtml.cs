using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.MyNotes
{
    public class MyNotesModel : PageModel
    {
        public IEnumerable<Note> UserNotes { get; set; }

        public void OnGet(IEnumerable<Note> _userNotes)
        {
            UserNotes = _userNotes;
        }
    }
}
