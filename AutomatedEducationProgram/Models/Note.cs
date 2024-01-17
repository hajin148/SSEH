using AutomatedEducationProgram.Areas.Data;
using EduApp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The user to which this note belongs
        /// </summary>
        public string UserId { get; set; }
        public List<VocabularyWord> VocabularyWords { get; set; }
        public List<ExamQuestion> ExamQuestions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public Note(int _id, string _userid, List<VocabularyWord> _vocabwords, List<ExamQuestion> _examqs, string _title, string _description, DateTime _created)
        {
            Id = _id;
            UserId = _userid;
            VocabularyWords = _vocabwords;
            ExamQuestions = _examqs;
            Title = _title;
            Description = _description;
            CreatedDate = _created;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Note()
        {
            int i = 0;
            i++;
        }
    }
}
