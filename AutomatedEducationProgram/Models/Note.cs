using AutomatedEducationProgram.Areas.Data;
using EduApp;

namespace AutomatedEducationProgram.Models
{
    public class Note
    {
        public int Id { get; set; }
        public AEPUser User { get; set; }
        public List<VocabularyWord> VocabularyWords { get; set; }
        public List<ExamQuestion> ExamQuestions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
