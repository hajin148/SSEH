using AutomatedEducationProgram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Views.MyNotes
{
    public class MyNotesModel : PageModel
    {
        public IEnumerable<Note> UserNotes { get; set; }

        public MyNotesModel(IEnumerable<Note> userNotes)
        {
            UserNotes = userNotes;
        }

        public void OnGet(IEnumerable<Note> _userNotes)
        {
            UserNotes = _userNotes;
        }
    }
}
