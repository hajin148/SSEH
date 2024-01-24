using System.Reflection;

namespace AutomatedEducationProgram.Models
{
    public class DocumentText
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }

        public DocumentText() { }

        public DocumentText(string text)
        {
            Text = text;
        }

        public DocumentText(string userId, string text)
        {
            UserId = userId;
            Text = text;
        }

        public DocumentText(string userId, string title, string text)
        {
            UserId = userId;
            Title = title;
            Text = text;
        }
    }
}
