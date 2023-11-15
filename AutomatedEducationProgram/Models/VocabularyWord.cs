namespace EduApp
{
    public class VocabularyWord
    {

        public string Term { get; set; }
        public string Definition { get; set; }
        
        public VocabularyWord(string term, string definition)
        {
            this.Term = term;
            this.Definition = definition;
        }
    }
}
