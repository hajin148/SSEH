using System.Reflection;

namespace AutomatedEducationProgram.Models
{
    /// <summary>
    /// Represents a document that the user uploads in order to generate study materials
    /// </summary>
    public class DocumentText
    {
        /// <summary>
        /// The auto-generated identifier of the document
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the document
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The contents of the document
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The note to which this document pertains
        /// </summary>
        public Note parentNote { get; set; }

        public DocumentText() { }

        public DocumentText(string text)
        {
            Text = text;
        }


        public DocumentText(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }
}
