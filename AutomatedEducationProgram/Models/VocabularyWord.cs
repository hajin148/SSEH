using AutomatedEducationProgram.Models;
using System.ComponentModel.DataAnnotations;

namespace AutomatedEducationProgram.Models
{
    /// <summary>
    /// Represents a vocabulary term and its definition. Comprises an overarching Note.
    /// </summary>
    public class VocabularyWord
    {
        /// <summary>
        /// The unique, auto-generated identifier of the vocabulary word in the database
        /// </summary>
        [Key]
        public int ID {  get; set; }
        /// <summary>
        /// The word or phrase being defined
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// The meaning of the term
        /// </summary>
        public string Definition { get; set; }
        /// <summary>
        /// The note to which this vocabulary word belongs
        /// </summary>
        public Note ParentNote { get; set; }
        /// <summary>
        /// The document from which this note was generated. Terms that are dynamically added 
        /// to a Note are automatically assigned the relevant document of one of the existing contents of the Note.
        /// </summary>
        public DocumentText RelevantDoc { get; set; }
        
        public VocabularyWord(string term, string definition)
        {
            this.Term = term;
            this.Definition = definition;
        }

        public VocabularyWord(string term, string definition, DocumentText dt)
        {
            this.Term = term;
            this.Definition = definition;
            this.RelevantDoc = dt;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public VocabularyWord()
        {

        }
    }
}
