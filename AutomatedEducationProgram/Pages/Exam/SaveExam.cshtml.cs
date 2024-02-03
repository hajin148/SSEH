using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Data;
using AutomatedEducationProgram.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace AutomatedEducationProgram.Pages.Exam
{
    public class SaveExamModel : PageModel
    {
        public List<ExamQuestion> GeneratedQuestionsMCQ { get; set; }
        public List<ExamQuestion> GeneratedQuestionsShort {  get; set; }
        public List<ExamQuestion> GeneratedQuestionsTF {  get; set; }
        public List<Note> ExistingNotes { get; set; }
        private readonly AutomatedEducationProgramContext _context;
        private readonly UserManager<AEPUser> _userManager;
        private readonly IConfiguration _configuration;

        public SaveExamModel(AutomatedEducationProgramContext context, UserManager<AEPUser> userManager, IConfiguration configuration)
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

            var mcqJson = HttpContext.Session.GetString("MCQQuestion");
            var shortJson = HttpContext.Session.GetString("ShortQuestion");
            var tfJson = HttpContext.Session.GetString("TFQuestion");

            if (!string.IsNullOrEmpty(mcqJson))
            {
                GeneratedQuestionsMCQ = JsonConvert.DeserializeObject<List<ExamQuestion>>(mcqJson);
            }

            if (!string.IsNullOrEmpty(shortJson))
            {
                GeneratedQuestionsShort = JsonConvert.DeserializeObject<List<ExamQuestion>>(shortJson);
            }

            if (!string.IsNullOrEmpty(tfJson))
            {
                GeneratedQuestionsTF = JsonConvert.DeserializeObject<List<ExamQuestion>>(tfJson);
            }

            ExistingNotes = _context.Notes.Where(note => note.UserId == user).ToList();

            return Page();

        }

        public IActionResult OnPostAsync(IFormCollection inputs)
        {
            string buttonClicked = HttpContext.Request.Form["submitButton"];
            List<ExamQuestion> qsToSave = new List<ExamQuestion>();
            foreach (var key in inputs.Keys)
            {
                if (key.StartsWith("examQ"))
                {
                    string q = inputs[key];
                    string ansKey = key.Replace("Q", "A");
                    string ans = inputs[ansKey];
                    qsToSave.Add(new ExamQuestion(q, ans));
                }
            }
            // If merging with existing note
            if (buttonClicked == "Merge To Existing Note")
            {
                int noteToUpdateId = int.Parse(inputs["existingNotes"]);
                Note noteToUpdate = _context.Notes.Where(note => note.Id == noteToUpdateId).FirstOrDefault();
                foreach (ExamQuestion q in qsToSave)
                {
                    q.ParentNote = noteToUpdate;
                    _context.ExamQuestions.Add(q);
                }
            }
            // If creating new note
            else
            {
                Note noteToSave = new Note();
                string user = _userManager.GetUserId(User);
                noteToSave.Title = inputs["title"];
                noteToSave.Description = inputs["description"];
                noteToSave.ExamQuestions = qsToSave;
                noteToSave.UserId = user;
                noteToSave.CreatedDate = DateTime.Now;
                _context.Notes.Add(noteToSave);
                foreach (var q in qsToSave)
                {
                    q.ParentNote = noteToSave;
                    _context.ExamQuestions.Add(q);
                }
            }
            _context.SaveChanges();
            return RedirectToPage("MyNotes");
        }
    }
}
