using AutomatedEducationProgram.Models;
using System.ComponentModel.DataAnnotations;

namespace EduApp
{
    /// <summary>
    /// Represents a vocabulary term and its definition. Comprises an overarching Note.
    /// </summary>
    public class VocabularyWord
    {
        [Key]
        public int ID {  get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public Note ParentNote { get; set; }
        
        public VocabularyWord(string term, string definition)
        {
            this.Term = term;
            this.Definition = definition;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public VocabularyWord()
        {

        }
    }
}
