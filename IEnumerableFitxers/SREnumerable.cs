using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IEnumerableFitxers
{
    public class SREnumerable:IEnumerable<string>
    {
        private readonly string fileName;

        public SREnumerable(string file)
        {
            fileName = file;
        }

        public IEnumerator<string> GetEnumerator()
        {
            //Podem fer qualsevol dels dos mètodes.
            using (StreamReader sR = new StreamReader(fileName))
            {
                // Consultem següent 'caracter' sense llegir-lo
                // en cas d'existir caracter retorna tota la linea
                while (sR.Peek() != -1)
                    yield return sR.ReadLine();
            }

            //string? linia;
            //while ((linia = sr.ReadLine()) != null)
            //    yield return linia;
        }

        // Recordeu que un metode genèric ens obliga a retornar també el no generic.
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}