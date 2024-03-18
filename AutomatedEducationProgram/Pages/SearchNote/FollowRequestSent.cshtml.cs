using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutomatedEducationProgram.Pages.SearchNote
{
    public class FollowRequestSentModel : PageModel
    {
        public void OnGet()
        {
        }

        public RedirectToPageResult OnPost()
        {
            return RedirectToPage("MyNotes");
        }
    }
}
