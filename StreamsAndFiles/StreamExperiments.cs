using System;
using System.IO;
using NUnit.Framework;

namespace StreamsAndFiles
{
    public abstract class StreamExperiments
    {
        private StreamFiller _streamFiller;

        [TestFixture]
        public class Writing_into_a_stream : StreamExperiments
        {
            [Test]
            public void Retrieving_the_results_as_bytes()
            {
                _streamFiller = new StreamFiller(10);
                byte[] results;
                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        _streamFiller.Fill(writer);
                        results = stream.GetBuffer();
                    }
                }
                Assert.That(results, Is.Not.Null);

                var encoding = System.Text.Encoding.UTF8;
                string encodedResult = encoding.GetString(results);
                Assert.That(encodedResult, Is.StringStarting("This is line 1."));
            }

            [Test]
            public void Retrieving_the_results_with_a_reader()
            {
                const int numberOfLines = 3;
                string[] results = FillStreamAndRetrieveWithAStreamReader(numberOfLines, StringSplitOptions.None);
                Assert.That(results, Is.Not.Null);
                Assert.That(results.Length, Is.EqualTo(numberOfLines + 1), "Because each line is added with WriteLine(), which appends a line terminator, the collection ends with a blank line.");
                Assert.That(results[2], Is.StringMatching("This is line 3."));
                Assert.That(results[numberOfLines], Is.StringMatching(""));
            }

            [Test]
            public void Retrieving_the_results_with_a_reader_and_excluding_the_final_blank_line()
            {
                const int numberOfLines = 3;
                string[] results = FillStreamAndRetrieveWithAStreamReader(numberOfLines, StringSplitOptions.RemoveEmptyEntries);
                Assert.That(results, Is.Not.Null);
                Assert.That(results.Length, Is.EqualTo(numberOfLines), "Using StringSplitOptions.RemoveEmptyEntries trims off the final blank-line entry when creating the array.");
                Assert.That(results[2], Is.StringMatching("This is line 3."));
            }

            [Test]
            public void Retrieving_the_results_with_a_reader_but_not_resetting_the_stream()
            {
                const int numberOfLines = 3;
                string[] results = FillStreamAndRetrieveWithAStreamReaderWithoutResettingTheStreamPosition(numberOfLines);
                Assert.That(results, Is.Not.Null);
                Assert.That(results.Length, Is.EqualTo(1), "The stream stays on the final position, so the only item to read from it is the final blank line.");
                Assert.That(results[0], Is.StringMatching(""));
            }

            private string[] FillStreamAndRetrieveWithAStreamReader(int numberOfLines, StringSplitOptions stringSplitOptions, bool resetStream = true)
            {
                _streamFiller = new StreamFiller(numberOfLines);
                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        _streamFiller.Fill(writer);

                        if (resetStream)
                        {
                            stream.Position = 0;
                        }
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd().Split(new[] { "\r\n" }, stringSplitOptions);
                        }
                    }
                }
            }

            private string[] FillStreamAndRetrieveWithAStreamReaderWithoutResettingTheStreamPosition(int numberOfLines)
            {
                return FillStreamAndRetrieveWithAStreamReader(numberOfLines, StringSplitOptions.None, false);
            }
        }

        [TestFixture]
        public class Converting_between_FileStream_and_FileInfo : StreamExperiments
        {
            
        }
    }
}
