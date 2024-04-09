
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace AutomatedEducationProgram.Models
{
    public static class VocabularyReader
    {
        /// <summary>
        /// Returns text from file of a given name as a string. Valid file types are .pdf, .doc, .docx, and .txt
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadWordsFromFile(string filename)
        {
            string[] tokens = filename.Split(".");
            switch (tokens[tokens.Length - 1])
            {
                case "txt":
                    return File.ReadAllText(filename).Trim();
                case "pdf":
                    return ReadPdfFile(filename).Trim();
                default:
                    throw new ArgumentException("Files of this type are not supported");
            }
        }

        public static string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }

        /// <summary>
        /// Returns a list of vocabulary words given a block of text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="termDefSeparator"></param>
        /// <param name="entrySeparator"></param>
        /// <returns></returns>
        public static List<VocabularyWord> ParseTermsAndDefs(string text, string termDefSeparator, string entrySeparator)
        {
            List<VocabularyWord> vocab = new List<VocabularyWord>();

            var pairs = text.Split(new[] { "[" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var cleanedPair = pair.TrimEnd(']');

                var tokens = cleanedPair.Split(new[] { "]:" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    vocab.Add(new VocabularyWord(tokens[0].Trim(), tokens[1].Trim()));
                }
            }

            return vocab;
        }


        public static List<ExamQuestion> ParseTermsAndDefs2(string text, string termDefSeparator, string entrySeparator)
        {
            List<ExamQuestion> qna = new List<ExamQuestion>();

            var pairs = text.Split(new[] { "[" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var cleanedPair = pair.TrimEnd(']');

                var tokens = cleanedPair.Split(new[] { "]:" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    if (tokens[1].Contains("?"))
                    {
                        var parts = cleanedPair.Split(new[] { "?" }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            tokens[0] = (parts[0]).Trim() + "?";
                            tokens[0] = tokens[0].Replace("]:", "");
                            tokens[1] = parts[1].Trim();
                        }
                        qna.Add(new ExamQuestion(tokens[0].Trim(), tokens[1].Trim()));
                    }
                    else
                    {
                        qna.Add(new ExamQuestion(tokens[0].Trim(), tokens[1].Trim()));
                    }

                }
            }

            return qna;
        }

        public static List<ExamQuestion> ParseTermsAndDefs3(string text, string termDefSeparator, string entrySeparator)
        {
            List<ExamQuestion> qna = new List<ExamQuestion>();

            var pairs = text.Split(new[] { "[" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var cleanedPair = pair.TrimEnd(']');

                var tokens = cleanedPair.Split(new[] { "]:" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    if (tokens[1].Contains("?"))
                    {
                        var parts = cleanedPair.Split(new[] { "?" }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            tokens[0] = (parts[0]).Trim();
                            tokens[1] = parts[1].Trim();
                            if (tokens[0].Contains("True") ||  tokens[0].Contains("true") )
                            {
                                tokens[0] = tokens[0].Replace("]: True.", "")
                                                     .Replace("]: true.", "")
                                                     .Replace("True.", "")
                                                     .Replace("true.", "")
                                                     .Trim();
                                tokens[1] = "True";

                            }
                            else if (tokens[0].Contains("False") || tokens[0].Contains("false"))
                            {
                                tokens[0] = tokens[0].Replace("]: False.", "")
                                                                .Replace("]: false.", "")
                                                                .Replace("False.", "")
                                                                .Replace("false.", "")
                                                                .Trim();
                                tokens[1] = "False";
                            }
                        }
                        qna.Add(new ExamQuestion(tokens[0].Trim(), tokens[1].Trim()));

                    }
                }
            }

            return qna;
        }

        public static List<string> ParseOptions(string text)
        {
            var options = new List<string>();
            string pattern;
            List<string> parts = new List<string>();

            if (Regex.IsMatch(text, "(.|\n)*A\\)(.|\n)*B\\)(.|\n)*C\\)(.|\n)*D\\).*"))
            {
                pattern = "A\\)|B\\)|C\\)|D\\)";
                parts = Regex.Split(text, pattern)
                                 .Where(p => !string.IsNullOrWhiteSpace(p))
                                 .ToList();
            }

            else if (Regex.IsMatch(text, "(.|\n)*a\\)(.|\n)*b\\)(.|\n)*c\\)(.|\n)*d\\)(.|\n)*"))
            {
                pattern = "a\\)|b\\)|c\\)|d\\)";
                parts = Regex.Split(text, pattern)
                                 .Where(p => !string.IsNullOrWhiteSpace(p))
                                 .ToList();
            }

            for (int i = parts.Count - 4; i < parts.Count; i++)
            {
                if (!(parts[i].Trim() == ""))
                {
                    options.Add(parts[i].Trim());
                }
            }

            return options;
        }
    }
}
