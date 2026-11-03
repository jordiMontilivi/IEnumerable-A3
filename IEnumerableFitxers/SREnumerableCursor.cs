using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IEnumerableFitxers
{

    public class SREnumerableCursor : IEnumerable<string>
    {
        private readonly string file;
        public SREnumerableCursor(string fitxer)
        {
            file = fitxer;
        }
        public IEnumerator<string> GetEnumerator()
        {
            return new SREnumerator(file);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    internal class SREnumerator : IEnumerator<string>
    {
        private StreamReader sR;
        private string current;

        public SREnumerator(string fitxer)
        {
            sR = new StreamReader(fitxer);
        }
        public string Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException(
                        "No hi ha cap element actual vàlid.");
                }

                return current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            sR.Close();
            sR.Dispose();
        }

        public bool MoveNext()
        {
            current = sR.ReadLine();

            return current != null;
        }

        public void Reset()
        {
            sR.DiscardBufferedData();
            sR.BaseStream.Seek(0, SeekOrigin.Begin);
            current = null;
        }
    }
}
