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

        //[TestMethod]
        public void ReadDocxFile()
        {
            Assert.AreEqual("This is a sample text file that I am using", VocabularyReader.ReadWordsFromFile("..\\..\\..\\TestFiles\\Sample.docx"));
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
    }
}
