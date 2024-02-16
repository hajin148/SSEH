using AutomatedEducationProgram.Areas.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutomatedEducationProgram.Models
{
    /// <summary>
    /// Represents an exam question, which is a component of an overarching Note
    /// </summary>
    public class ExamQuestion
    {
        private string v1;
        private string v2;

        /// <summary>
        /// This question's unique ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The note to which this question belongs
        /// </summary>
        public Note ParentNote { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Explanation { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public int QuestionType { get; set; }
        public DocumentText RelevantDoc { get; set; }

        public static int TF_QUESTION = 0;
        public static int SHORT_ANSWER_QUESTION = 1;
        public static int MULTIPLE_CHOICE_QUESTION = 2;

        /// <summary>
        /// default constructor
        /// </summary>
        public ExamQuestion(Note pn, string q, string e, string aa, string ab, string ac, string ad)
        {
            ParentNote = pn;
            Question = q;
            Explanation = e;
            AnswerA = aa;
            AnswerB = ab;
            AnswerC = ac;
            AnswerD = ad;
        }

        public ExamQuestion()
        {

        }

        public ExamQuestion(string question, string answer)
        {
            this.Question = question;
            this.Answer = answer;
        }

        public ExamQuestion(string question, string answer, int type)
        {
            this.Question = question;
            this.Answer = answer;
            this.QuestionType = type;
        }

        public ExamQuestion(string question, string answer, int type, DocumentText dt)
        {
            this.Question = question;
            this.Answer = answer;
            this.QuestionType = type;
            this.RelevantDoc = dt;
        }



    }
}