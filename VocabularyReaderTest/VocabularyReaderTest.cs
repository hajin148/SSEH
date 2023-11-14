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
    }
}
