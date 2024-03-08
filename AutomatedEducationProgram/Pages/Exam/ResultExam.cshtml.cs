using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class ResultExamModel : PageModel
    {
        [BindProperty]
        public string UserAnswers { get; set; }
        public Dictionary<int, string> UserAnswersObj { get; set; }

        public void OnPost()
        {
            if (!string.IsNullOrWhiteSpace(UserAnswers))
            {
                UserAnswersObj = JsonConvert.DeserializeObject<Dictionary<int, string>>(UserAnswers);
            }
            else
            {
                UserAnswersObj = new Dictionary<int, string>();
            }
        }


    }
}
