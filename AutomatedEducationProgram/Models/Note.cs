using AutomatedEducationProgram.Areas.Data;
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
        /// The auto-generated ID of the user to which this Note belongs
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// The vocabulary terms and definitions that belong to this Note
        /// </summary>
        public List<VocabularyWord> VocabularyWords { get; set; }
        /// <summary>
        /// The exam questions and answers that belong to this note
        /// </summary>
        public List<ExamQuestion> ExamQuestions { get; set; }
        /// <summary>
        /// The user-given title of the Note
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The user-given description of the Note
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The timestamp of when the Note was last edited
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Signifies whether the Note is searchable by other users.
        /// </summary>
        public bool IsPublic { get; set; }

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
