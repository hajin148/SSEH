

namespace AutomatedEducationProgram.Models
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
        public void ParseTermsAndDefsPhilos()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "]: ", ", [");
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

        [TestMethod]
        public void ParseTermsAndDefsMath()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput2.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "[[[", "]]]");
            Assert.AreEqual("Function:", words[0].Term);
            Assert.AreEqual("Domain:", words[1].Term);
            Assert.AreEqual("Range:", words[2].Term);
            Assert.AreEqual("Symmetry:", words[3].Term);
            Assert.AreEqual("Vertical Line Test:", words[4].Term);
            Assert.IsTrue(words[0].Definition.Contains("A function is a special type of relation between two sets, A and B, where each element in the first"));
            Assert.IsTrue(words[1].Definition.Contains("of all possible values for the independent variable."));
            Assert.IsTrue(words[2].Definition.Contains("consisting of all possible values for the dependent variable. It represents"));
            Assert.IsTrue(words[3].Definition.Contains("Symmetry properties of a function describe its behavior in relation to certain transformations."));
            Assert.IsTrue(words[4].Definition.Contains("It states that every vertical line drawn should intersect the graph"));
            Assert.IsTrue(words.Count == 5);
        }

        [TestMethod]
        public void ParseTermsAndDefsEnglish()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput3.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "]: ", ", [");
            Assert.AreEqual("Sociohistorical Context:", words[0].Term);
            Assert.AreEqual("Marvel Cinematic Universe (MCU):", words[1].Term);
            Assert.AreEqual("Offensive Language:", words[2].Term);
            Assert.AreEqual("#OscarsSoWhite:", words[3].Term);
            Assert.AreEqual("Diversity in Hollywood:", words[4].Term);
            Assert.IsTrue(words[0].Definition.Contains("The cultural and historical background that influences"));
            Assert.IsTrue(words[1].Definition.Contains("franchise and shared universe that includes a series of superhero films"));
            Assert.IsTrue(words[2].Definition.Contains("inappropriate, or harmful, particularly in the context of artistic works"));
            Assert.IsTrue(words[3].Definition.Contains("media hashtag and movement that emerged to protest the lack of diversity, particularly the underrepresentation"));
            Assert.IsTrue(words[4].Definition.Contains("a variety of voices, perspectives, and representations, particularly concerning race,"));
            Assert.IsTrue(words.Count == 5);
        }

        [TestMethod]
        public void ParseTermsAndDefsHistory()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput4.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "]: ", ", [");
            Assert.AreEqual("Middle Ages:", words[0].Term);
            Assert.AreEqual("Black Death:", words[1].Term);
            Assert.AreEqual("Feudal Society:", words[2].Term);
            Assert.AreEqual("Great Schism:", words[3].Term);
            Assert.AreEqual("Reconquista:", words[4].Term);
            Assert.IsTrue(words[0].Definition.Contains("The historical period between the fall of the Roman Empire"));
            Assert.IsTrue(words[1].Definition.Contains("A devastating pandemic that struck Europe in the 1340s, caused by the bubonic plague"));
            Assert.IsTrue(words[2].Definition.Contains("A social and economic system in medieval Europe where lords owned land, knights provided military service"));
            Assert.IsTrue(words[3].Definition.Contains("The split in the Christian Church in 1054, dividing it into the western Roman Catholic Church"));
            Assert.IsTrue(words[4].Definition.Contains("Christian-Muslim conflicts during the Crusades for control of the Holy Land."));
            Assert.IsTrue(words.Count == 5);
        }

        [TestMethod]
        public void ParseTermsAndDefsScience()
        {
            string chatOutput = File.ReadAllText("..\\..\\..\\TestFiles\\SampleChatGPTOutput5.txt");
            List<VocabularyWord> words = VocabularyReader.ParseTermsAndDefs(chatOutput, "]: ", ", [");
            Assert.AreEqual("Dalton's Atomic Theory:", words[0].Term);
            Assert.AreEqual("Atomos:", words[1].Term);
            Assert.AreEqual("Law of Definite Proportions:", words[2].Term);
            Assert.AreEqual("Law of Multiple Proportions:", words[3].Term);
            Assert.AreEqual("Isooctane:", words[4].Term);
            Assert.IsTrue(words[0].Definition.Contains("A scientific theory proposed by John Dalton in 1807, consisting of the following postulates:"));
            Assert.IsTrue(words[1].Definition.Contains("A term used by ancient Greek philosophers Leucippus and Democritus in the fifth century"));
            Assert.IsTrue(words[2].Definition.Contains("it states that all samples"));
            Assert.IsTrue(words[3].Definition.Contains("States that when two elements react to form more than one compound, a fixed mass of one element"));
            Assert.IsTrue(words[4].Definition.Contains("mentioned in the text as an example illustrating the constant composition of compounds. It has a carbon-to-hydrogen mass ratio of 5.33:1."));
            Assert.IsTrue(words.Count == 5);
        }

        [TestMethod]
        public void ParseTermsAndDefs2TrueFalse()
        {
            string msg = "[Groundwater mainly consists of meteoric water that circulates as part of the water cycle.]: True \r\n\r\n[Porosity and permeability are the same concepts when measuring water flow through a material.]: False";
            List<ExamQuestion> result = VocabularyReader.ParseTermsAndDefs2(msg, "]: ", ", [");
            string q1 = "Groundwater mainly consists of meteoric water that circulates as part of the water cycle.";
            string q2 = "Porosity and permeability are the same concepts when measuring water flow through a material.";
            string a1 = "True";
            string a2 = "False";
            Assert.AreEqual(q1.Trim(), result[0].Question.Trim());
            Assert.AreEqual(a1.Trim(), result[0].Answer.Trim());
            Assert.AreEqual(q2.Trim(), result[1].Question.Trim());
            Assert.AreEqual(a2.Trim(), result[1].Answer.Trim());
        }

        [TestMethod]
        public void ParseTermsAndDefs2ShortAnswer()
        {
            string msg = "[What is groundwater and how is it formed?]: Groundwater is water that saturates the ground, filling all available spaces. It is primarily meteoric water that circulates as part of the water cycle, soaking into the ground from precipitation, lakes, and streams.\r\n\r\n[What is the difference between porosity and permeability in rocks and sediments?]: Porosity refers to the proportion of empty space in a rock or sediment, while permeability measures the ease with which water can flow through a material and depends on the sizes of the individual cavities and crevices linking them.\r\n";
            List<ExamQuestion> result = VocabularyReader.ParseTermsAndDefs2(msg, "]: ", ", [");
            string q1 = "What is groundwater and how is it formed?\r\n";
            string q2 = "What is the difference between porosity and permeability in rocks and sediments?\r\n";
            string a1 = "Groundwater is water that saturates the ground, filling all available spaces. It is primarily meteoric water that circulates as part of the water cycle, soaking into the ground from precipitation, lakes, and streams.\r\n";
            string a2 = "Porosity refers to the proportion of empty space in a rock or sediment, while permeability measures the ease with which water can flow through a material and depends on the sizes of the individual cavities and crevices linking them.\r\n";
            Assert.AreEqual(q1.Trim(), result[0].Question.Trim());
            Assert.AreEqual(a1.Trim(), result[0].Answer.Trim());
            Assert.AreEqual(q1.Trim(), result[0].Question.Trim());
            Assert.AreEqual(a1.Trim(), result[0].Answer.Trim());
        }

        [TestMethod]
        public void ParseTermsAndDefs2MultipleChoice()
        {
            string msg = "[Question 1]: \r\n\r\nWhat is the most abundant type of groundwater mentioned in the passage? \r\n\r\na) Saltwater  \r\nb) Groundwater  \r\nc) Meteoric water  \r\nd) Hot water  \r\n\r\n[Question 2]: \r\n\r\nWhere can one find common spaces for groundwater among particles like sand grains and tiny pebbles? \r\n\r\na) In the air  \r\nb) in solid rock formations  \r\nc) In the atmosphere  \r\nd) In loose, unconsolidated sand and gravel  \r\n\r\n[Question 3]: \r\n\r\nWhat is the difference between porosity and permeability of rocks mentioned in the passage? \r\n\r\na) Porosity measures the ease of water flow, and permeability measures the total volume of empty space  \r\nb) Porosity measures the total volume of empty space, and permeability measures the ease of water flow  \r\nc) Porosity measures the ease of water flow, and permeability measures the empty space in rocks  \r\nd) Porosity measures the empty space in rocks, and permeability measures the ease of water flow \r\n";
            List<ExamQuestion> result = VocabularyReader.ParseTermsAndDefs2(msg, "]: ", ", [");
            string q1 = "Question 1 \r\n\r\nWhat is the most abundant type of groundwater mentioned in the passage?\r\n";
            string q2 = "Question 2 \r\n\r\nWhere can one find common spaces for groundwater among particles like sand grains and tiny pebbles?\r\n";
            string q3 = "Question 3 \r\n\r\nWhat is the difference between porosity and permeability of rocks mentioned in the passage?\r\n";
            string a1 = "a) Saltwater  \r\nb) Groundwater  \r\nc) Meteoric water  \r\nd) Hot water\r\n";
            string a2 = "a) In the air  \r\nb) in solid rock formations  \r\nc) In the atmosphere  \r\nd) In loose, unconsolidated sand and gravel\r\n";
            string a3 = "a) Porosity measures the ease of water flow, and permeability measures the total volume of empty space  \r\nb) Porosity measures the total volume of empty space, and permeability measures the ease of water flow  \r\nc) Porosity measures the ease of water flow, and permeability measures the empty space in rocks  \r\nd) Porosity measures the empty space in rocks, and permeability measures the ease of water flow\r\n";
            Assert.AreEqual(q1.Trim(), result[0].Question.Trim());
            Assert.AreEqual(a1.Trim(), result[0].Answer.Trim());
            Assert.AreEqual(q2.Trim(), result[1].Question.Trim());
            Assert.AreEqual(a2.Trim(), result[1].Answer.Trim());
            Assert.AreEqual(q3.Trim(), result[2].Question.Trim());
            Assert.AreEqual(a3.Trim(), result[2].Answer.Trim());
        }

        [TestMethod]
        public void ParseOptions()
        {
            string msg = "a) Saltwater  \r\nb) Groundwater  \r\nc) Meteoric water  \r\nd) Hot water\r\n";
            List<string> result = VocabularyReader.ParseOptions(msg);
            Assert.AreEqual(result[0], "Saltwater");
            Assert.AreEqual(result[1], "Groundwater");
            Assert.AreEqual(result[2], "Meteoric water");
            Assert.AreEqual(result[3], "Hot water");
        }

        [TestMethod]
        public void ParseOptionsCaps()
        {
            string msg = "A) Saltwater  \r\nB) Groundwater  \r\nC) Meteoric water  \r\nD) Hot water\r\n";
            List<string> result = VocabularyReader.ParseOptions(msg);
            Assert.AreEqual(result[0], "Saltwater");
            Assert.AreEqual(result[1], "Groundwater");
            Assert.AreEqual(result[2], "Meteoric water");
            Assert.AreEqual(result[3], "Hot water");
        }

        [TestMethod]
        public void ParseOptionsIrregular()
        {
            string msg = "   A) 1  \r\nB) 2  \r\nC) 3  \r\nD) 4\r\n";
            List<string> result = VocabularyReader.ParseOptions(msg);
            for (int i = 0; i < 0; i++)
            {
                Assert.AreEqual(result[i + 1], "" + i + 1);
            }
        }
    }
}
