using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SREnumerable
{
    public class StreamReaderEnumerable:IEnumerable<string>
    {
        private readonly string fileName;

        public StreamReaderEnumerable(string file)
        {
            fileName = file;
        }

        public IEnumerator<string> GetEnumerator()
        {
            using (StreamReader sR = new StreamReader(fileName))
            {
                while (sR.Peek() != -1)
                    yield return sR.ReadLine();
            }

                //string? linia;
                //while ((linia = sr.ReadLine()) != null)
                //    yield return linia;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
