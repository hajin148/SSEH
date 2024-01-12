using AutomatedEducationProgram.Areas.Data;

namespace AutomatedEducationProgram.Models
{
    /// <summary>
    /// Represents an exam question, which is a component of an overarching Note
    /// </summary>
    public class ExamQuestion
    {
        /// <summary>
        /// This question's unique ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The note to which this question belongs
        /// </summary>
        public Note ParentNote { get; set; }
        public string Question { get; set; }
        public string Explanation { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }

    }
}