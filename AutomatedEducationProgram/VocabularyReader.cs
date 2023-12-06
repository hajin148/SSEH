using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Reflection.Metadata;
using System.Text;

namespace EduApp
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
    }
}
