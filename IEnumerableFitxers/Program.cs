using System.Xml.Linq;

namespace IEnumerableFitxers
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Recordeu no usar el metode Camps ja que inicialment el vaig voler deixar per poder elegir no solament un camp per número sino per tipus
			// Posaré un Mètode CampsTipus, però no el faré servir, llegiu-lo vosaltres.
			const string FILE = "LogIncidencies.csv";

			if (File.Exists(FILE))
			{

				// Seleccionem la implementació
				Console.WriteLine("TRIA LA IMPLEMENTACIÓ");
				Console.WriteLine("1. SREnumerable (yield return + foreach)");
				Console.WriteLine("2. SREnumerableCursor (IEnumerator + MoveNext)");
				Console.Write("Opció: ");

				string opcio = Console.ReadLine();

				// Variable comuna per a les dues implementacions
				IEnumerable<string> dades;

				// Indica com recorrerem els resultats
				bool cursor;

				switch (opcio)
				{
					case "1":
						// Mostrarem les dades segons foreach
						dades = new SREnumerable(FILE);
						cursor = false;
						Console.WriteLine("\nUtilitzant SREnumerable");
						break;

					case "2":
						// Mostrarem les dades amb un cursor -> while (enumerador.MoveNext())...)
						dades = new SREnumerableCursor(FILE);
						cursor = true;
						Console.WriteLine("\nUtilitzant SREnumerableCursor");
						break;

					default:
						Console.WriteLine("Opció incorrecta.");
						return; // No usar aquest tipus de return per acabar l'aplicació
				}


				// Es pressuposa que la primera línia és la capçalera.
				// Cada consulta torna a enumerar el fitxer.
				var incidencies = dades.Skip(1);


				// 1. Comptar les incidències
				Console.WriteLine("\n1. TOTAL D'INCIDÈNCIES");

				Console.WriteLine(incidencies.Count());


				// 2. Cercar incidències relacionades amb servidors
				Console.WriteLine("\n2. INCIDÈNCIES SOBRE SERVIDORS");

				var servidors = incidencies
					.Where(linia => linia.Contains(
						"servidor",
						StringComparison.OrdinalIgnoreCase));

				// Crida completa al mètode, en c# podem simplificar
				// Mostrar<string>(servidors, cursor); 
				Mostrar(servidors, cursor);


				// 3. Incidències amb prioritat crítica
				Console.WriteLine("\n3. PRIORITAT CRÍTICA");

				var critiques = incidencies
					.Where(linia =>
						Camps(linia)[3].Trim()
							.Equals("Crítica",
								StringComparison.OrdinalIgnoreCase));

				Mostrar(critiques, cursor);


				// 4. Incidències pendents
				Console.WriteLine("\n4. INCIDÈNCIES PENDENTS");

				var pendents = incidencies
					.Where(linia =>
						linia.Split(';')[4].Trim()
							//Camps(linia)[4].Trim()
							.Equals("Pendent",
								StringComparison.OrdinalIgnoreCase));

				Mostrar(pendents, cursor);


				// 5. Ordenar per departament i després per data
				Console.WriteLine("\n5. ORDRE PER DEPARTAMENT I DATA");

				var ordenades = incidencies
					.OrderBy(linia => Camps(linia)[2].Trim())
					.ThenBy(linia => Camps(linia)[1].Trim());

				Mostrar(ordenades, cursor);


				// 6. Incidències de la tercera a la cinquena posició,
				//    transformades a minúscules
				Console.WriteLine("\n6. POSICIONS 3 A 5 EN MINÚSCULES");

				var fragment = incidencies
					.Skip(2)
					.Take(3)
					.Select(linia => linia.ToLowerInvariant());

				Mostrar(fragment, cursor);


				// 7. Departaments sense repeticions i ordenats
				Console.WriteLine("\n7. DEPARTAMENTS ÚNICS");

				var departaments = incidencies
					.Select(linia => Camps(linia)[2].Trim())
					.Distinct()
					.OrderBy(departament => departament);

				Mostrar(departaments, cursor);


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


				// En aquest cas la consulta LinQ ens retorna un objecte anonim (una projecció d'un objecte que no existeix) 
				// i nosaltres agafem les parts necessaries, ho podem incloure al Mostar<T> però hem de saber els atributs dels
				// objectes anonims que hem generat a la pràctica i no veig sentit fer-ho amb un mètode...
				
				if (cursor)
				{
					// Podem usar un using i ens estalviem el close en els fitxers i Dispose en els enumeradors, ja que el using s'encarrega de tot.
					using (var enumerador = perEstat.GetEnumerator())
					{
						while (enumerador.MoveNext())
						{
							Console.WriteLine(
								$"{enumerador.Current.Estat}: {enumerador.Current.Quantitat}");
						}
					}

					//var enumerador = perEstat.GetEnumerator();
					//try
					//{
					//	// Recorrem el enumerador mentre MoveNext ens deixi
					//	while (enumerador.MoveNext())
					//	{
					//		// Mostrem valor actual
					//		Console.WriteLine($"{enumerador.Current.Estat}: {enumerador.Current.Quantitat}");
					//	}
					//}
					//finally
					//{
					//	enumerador.Dispose();
					//}
				}

				else
					foreach (var resultat in perEstat)
						Console.WriteLine($"{resultat.Estat}: {resultat.Quantitat}");

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

				// Objecte anonim, no usaré el metode Mostrar
				if(cursor)
				{
					using (var enumerador = critiquesPendents.GetEnumerator())
					{
						while (enumerador.MoveNext())
						{
							Console.WriteLine(
								$"{enumerador.Current.Data} | {enumerador.Current.Id} | " +
								$"{enumerador.Current.Descripcio}");
						}
					}
				}
				else
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
		private static string[] Camps(string linia)
		{
			return linia.Split(';');
		}
		private static string CampsTipus(string linia, string camp)
		{
			//No el faré servir ja que m'agafa mandra i tampoc té sentit ara mateix,
			//però seria una forma de fer un mètode que donat una linia i un tipus de dada ens dona el valor directament
			//ID;DATA;DEPARTAMENT;PRIORITAT;ESTAT;DESCRIPCIO
			string valor;
			string[] camps = linia.Split(";");
			switch (camp.ToUpper())
			{
				case "ID":
					valor = camps[0].Trim();
					break;
				case "DATA":
					valor = camps[1].Trim();
					break;
				case "DEPARTAMENT":
					valor = camps[2].Trim();
					break;
				case "PRIORITAT":
					valor = camps[3].Trim();
					break;
				case "ESTAT":
					valor = camps[4].Trim();
					break;
				case "DESCRIPCIO":
					valor = camps[5].Trim();
					break;
				default:
					valor = "";
					break;
			}

			return valor;
		}

		//L'he fet genèric <T> però hagués pogut ser <string>
		private static void Mostrar<T>(IEnumerable<T> elements, bool cursor)
		{
			if (cursor)
			{
				// Recorregut manual amb IEnumerator
				IEnumerator<T> enumerador = elements.GetEnumerator();

				try
				{
					// Recorrem el enumerador mentre MoveNext ens deixi
					while (enumerador.MoveNext())
					{
						// Mostrem valor actual
						Console.WriteLine(enumerador.Current);
					}
					// enumerador.Reset();
				}
				finally
				{
					enumerador.Dispose();
				}
			}
			else
				// Directament enxufem el foreach
				foreach (T element in elements)
				{
					Console.WriteLine(element);
				}
		}
	}
}
