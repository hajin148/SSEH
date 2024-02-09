using System.Reflection;

namespace AutomatedEducationProgram.Models
{
    public class DocumentText
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
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
