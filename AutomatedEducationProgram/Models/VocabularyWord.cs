using AutomatedEducationProgram.Models;

namespace EduApp
{
    /// <summary>
    /// Represents a vocabulary term and its definition. Comprises an overarching Note.
    /// </summary>
    public class VocabularyWord
    {

        public string Term { get; set; }
        public string Definition { get; set; }
        public Note ParentNote { get; set; }
        
        public VocabularyWord(string term, string definition)
        {
            this.Term = term;
            this.Definition = definition;
        }
    }
}
