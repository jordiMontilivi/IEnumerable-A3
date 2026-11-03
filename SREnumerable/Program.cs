namespace SREnumerable
{
    internal class Program
    {
        static void Main(string[] args)
        {

            const string FILE = "LogIncidencies.csv";

            if (!File.Exists(FILE))
            {
                var dades = new StreamReaderEnumerable(FILE);

                // Es pressuposa que la primera línia és la capçalera.
                // Cada consulta torna a enumerar el fitxer.
                var incidencies = dades.Skip(1);

                // Camps: 0=Id, 1=Data, 2=Departament,
                //        3=Prioritat, 4=Estat, 5=Descripcio
                // string[] Camps(string linia) => linia.Split(';');

                    // 1. Comptar les incidències
                    Console.WriteLine("\n1. TOTAL D'INCIDÈNCIES");

                Console.WriteLine(incidencies.Count());


                // 2. Cercar incidències relacionades amb servidors
                Console.WriteLine("\n2. INCIDÈNCIES SOBRE SERVIDORS");

                var servidors = incidencies
                    .Where(linia => linia.Contains(
                        "servidor",
                        StringComparison.OrdinalIgnoreCase));

                foreach (string linia in servidors)
                    Console.WriteLine(linia);


                // 3. Incidències amb prioritat crítica
                Console.WriteLine("\n3. PRIORITAT CRÍTICA");

                var critiques = incidencies
                    .Where(linia =>
                        Camps(linia)[3].Trim()
                            .Equals("Crítica",
                                StringComparison.OrdinalIgnoreCase));

                foreach (string linia in critiques)
                    Console.WriteLine(linia);


                // 4. Incidències pendents
                Console.WriteLine("\n4. INCIDÈNCIES PENDENTS");

                var pendents = incidencies
                    .Where(linia =>
                        Camps(linia)[4].Trim()
                            .Equals("Pendent",
                                StringComparison.OrdinalIgnoreCase));

                foreach (string linia in pendents)
                    Console.WriteLine(linia);


                // 5. Ordenar per departament i després per data
                Console.WriteLine("\n5. ORDRE PER DEPARTAMENT I DATA");

                var ordenades = incidencies
                    .OrderBy(linia => Camps(linia)[2].Trim())
                    .ThenBy(linia => Camps(linia)[1].Trim());

                foreach (string linia in ordenades)
                    Console.WriteLine(linia);


                // 6. Incidències de la tercera a la cinquena posició,
                //    transformades a minúscules
                Console.WriteLine("\n6. POSICIONS 3 A 5 EN MINÚSCULES");

                var fragment = incidencies
                    .Skip(2)
                    .Take(3)
                    .Select(linia => linia.ToLowerInvariant());

                foreach (string linia in fragment)
                    Console.WriteLine(linia);


                // 7. Departaments sense repeticions i ordenats
                Console.WriteLine("\n7. DEPARTAMENTS ÚNICS");

                var departaments = incidencies
                    .Select(linia => Camps(linia)[2].Trim())
                    .Distinct()
                    .OrderBy(departament => departament);

                foreach (string departament in departaments)
                    Console.WriteLine(departament);


                // 8. Comptar incidències per estat
                Console.WriteLine("\n8. INCIDÈNCIES PER ESTAT");

                var perEstat = incidencies
                    .GroupBy(linia => Camps(linia)[4].Trim())
                    .Select(grup => new
                    {
                        Estat = grup.Key,
                        Quantitat = grup.Count()
                    })
                    .OrderBy(resultat => resultat.Estat);

                foreach (var resultat in perEstat)
                {
                    Console.WriteLine(
                        $"{resultat.Estat}: {resultat.Quantitat}");
                }


                // 9. Departament amb més incidències
                Console.WriteLine("\n9. DEPARTAMENT AMB MÉS INCIDÈNCIES");

                var departamentMaxim = incidencies
                    .GroupBy(linia => Camps(linia)[2].Trim())
                    .Select(grup => new
                    {
                        Departament = grup.Key,
                        Quantitat = grup.Count()
                    })
                    .OrderByDescending(resultat => resultat.Quantitat)
                    .ThenBy(resultat => resultat.Departament)
                    .First();

                Console.WriteLine(
                    $"{departamentMaxim.Departament}: " +
                    $"{departamentMaxim.Quantitat} incidències");


                // 10. Incidències crítiques i pendents,
                //     ordenades cronològicament
                Console.WriteLine("\n10. CRÍTIQUES PENDENTS");

                var critiquesPendents = incidencies
                    .Where(linia =>
                    {
                        string[] camps = Camps(linia);

                        return camps[3].Trim().Equals(
                                   "Crítica",
                                   StringComparison.OrdinalIgnoreCase)
                               &&
                               camps[4].Trim().Equals(
                                   "Pendent",
                                   StringComparison.OrdinalIgnoreCase);
                    })
                    .OrderBy(linia => Camps(linia)[1].Trim())
                    .Select(linia =>
                    {
                        string[] camps = Camps(linia);

                        return new
                        {
                            Id = camps[0].Trim(),
                            Data = camps[1].Trim(),
                            Descripcio = camps[5].Trim()
                        };
                    });

                foreach (var incidencia in critiquesPendents)
                {
                    Console.WriteLine(
                        $"{incidencia.Data} | {incidencia.Id} | " +
                        $"{incidencia.Descripcio}");
                }
            }
            else
            {
                Console.WriteLine($"No s'ha trobat el fitxer: {FILE}");
            }
        }
        // Camps: 0=Id, 1=Data, 2=Departament,
        //        3=Prioritat, 4=Estat, 5=Descripcio
        public static string[] Camps(string linia)
        {
            return linia.Split(';');
        }
    }
}
