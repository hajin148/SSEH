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
        /// This unique, auto-generated identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The note to which this question belongs
        /// </summary>
        public Note ParentNote { get; set; }
        /// <summary>
        /// The question itself
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// The correct answer to the question. In the case of a multiple-choice question, this is the same as AnswerA
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// An explanation for why the correct answer is what it is
        /// </summary>
        public string Explanation { get; set; }
        /// <summary>
        /// The correct answer in an multiple-choice question
        /// </summary>
        public string AnswerA { get; set; }
        /// <summary>
        /// An incorrect answer to a multiple-choice question
        /// </summary>
        public string AnswerB { get; set; }
        /// <summary>
        /// An incorrect answer to a multiple-choice question
        /// </summary>
        public string AnswerC { get; set; }
        /// <summary>
        /// An incorrect answer to a multiple-choice question
        /// </summary>
        public string AnswerD { get; set; }
        /// <summary>
        /// The type of question (TF_QUESTION, SHORT_ANSWER_QUESTION, or MULTIPLE_CHOICE_QUESTION
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// The document from which this question was generated. Questions that are dynamically added 
        /// to a Note are automatically assigned the relevant document of one of the existing contents of the Note.
        /// </summary>
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

        internal void ShuffleAnswers()
        {
            List<string> answers = new List<string>();
            answers.Add(AnswerA); 
            answers.Add(AnswerB); 
            answers.Add(AnswerC); 
            answers.Add(AnswerD);
            Random rng = new Random();
            int n = answers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = answers[k];
                answers[k] = answers[n];
                answers[n] = value;
            }
            AnswerA = answers[0];
            AnswerB = answers[1];
            AnswerC = answers[2];
            AnswerD = answers[3];
        }
    }
}