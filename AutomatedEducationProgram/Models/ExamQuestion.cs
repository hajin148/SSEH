using AutomatedEducationProgram.Areas.Data;

namespace AutomatedEducationProgram.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public AEPUser User { get; set; }
        public string Question { get; set; }
        public string Explanation { get; set; }
        public List<string> answers { get; set; }
    }
}