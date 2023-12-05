using AutomatedEducationProgram.Areas.Data;
using EduApp;

namespace AutomatedEducationProgram.Models
{
    /// <summary>
    /// A Note is the basic, shareable unit of Autoedu. A Note is a themed package vocabulary words and exam questions
    /// </summary>
    public class Note
    {
        /// <summary>
        /// This Note's unique ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The user to which this note belongs
        /// </summary>
        public AEPUser User { get; set; }
        public List<VocabularyWord> VocabularyWords { get; set; }
        public List<ExamQuestion> ExamQuestions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
