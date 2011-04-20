using System.IO;

namespace StreamsAndFiles
{
    public class StreamFiller
    {
        private readonly int _numberOfLines;

        public StreamFiller(int numberOfLines)
        {
            _numberOfLines = numberOfLines;
        }

        public void Fill(TextWriter writer)
        {
            for (int i = 1; i <= _numberOfLines; i++)
            {
                writer.WriteLine("This is line {0}.", i);
            }
            writer.Flush();
        }
    }
}