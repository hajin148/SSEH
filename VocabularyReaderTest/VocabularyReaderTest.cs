namespace EduApp
{
    [TestClass]
    public class VocabularyReaderTest
    {

        [TestMethod]
        public void ReadTxtFile()
        {
            Assert.AreEqual("This is a sample text file that I am using", VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample.txt"));
        }

        [TestMethod]
        public void ReadPdfFile()
        {
            Assert.AreEqual("This is a sample text ﬁle that I am using", VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample.pdf"));
        }

        [TestMethod]
        public void ParseTermsAndDefs()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "[[[", "]]]");
            Assert.AreEqual("Socratic Method:", words[0].Term);
            Assert.AreEqual("Harm Principle:", words[1].Term);
            Assert.AreEqual("Ahimsa:", words[2].Term);
            Assert.AreEqual("Oracle:", words[3].Term);
            Assert.AreEqual("Self-Examination:", words[4].Term);
            Assert.IsTrue(words[0].Definition.Contains("questioning, known as the Socratic method"));
            Assert.IsTrue(words[1].Definition.Contains("of one's knowledge to avoid causing harm."));
            Assert.IsTrue(words[2].Definition.Contains("emphasizing the absence of causing injury or harm."));
            Assert.IsTrue(words[3].Definition.Contains("Pythian prophetess at Delphi, a shrine dedicated to the god Apollo."));
            Assert.IsTrue(words[4].Definition.Contains("Self-examination is a key aspect of Socrates's philosophy,"));
            Assert.IsTrue(words.Count == 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadInvalidFile()
        {
            Assert.AreEqual("This is a sample text file that I am using", VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample.docx"));
        }

        [TestMethod]
        public void ReadMultilineTxtFile()
        {
            string expected = "This is a text file.\r\nThis is still a text file.\r\nThis will always be a text file.";
            Assert.AreEqual(expected, VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample2.txt"));
        }

        [TestMethod]
        public void ReadMultilinePdfFile()
        {
            string expected = "This is a text file. \nThis is still a text file. \nThis will always be a text file.";
            string actual = VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample2.pdf");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadContinuousPdfFile()
        {
            string expected = "abcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabc\nabcabcabcabcabcabcabcabcabcabcabcabcabc";
            string actual = VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample3.pdf");
            Assert.AreEqual(expected, actual);
        }
    }
}
