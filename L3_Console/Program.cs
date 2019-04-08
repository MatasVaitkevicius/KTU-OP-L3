using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L3_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var utilitiesList = ReadUtilitiesData("U16a.txt");
            var residentsList = ReadResidentsData("U16b.txt");
            const string chosenMonth = "Balandis";
            const string chosenUtility = "Elektra";

            CalculateMoneySpent(utilitiesList, residentsList);
            var filteredResidentsList = FilterResidentsByMoneySpent(residentsList);
            filteredResidentsList.Sort();
            var cheapestMonth = CalculateCheapestMonth(residentsList, utilitiesList);
            RemoveResidents(filteredResidentsList, utilitiesList, chosenMonth, chosenUtility);

            Console.WriteLine("hello");
            Console.ReadKey();
        }

        static List<Residents> ReadResidentsData(string file)
        {
            var residentsList = new List<Residents>();

            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var values = line.Split(' ');
                    var surname = values[0];
                    var name = values[1];
                    var address = values[2];
                    var month = values[3];
                    var utilityCode = values[4];
                    var serviceCount = int.Parse(values[5]);
                    residentsList.AddData(new Residents(surname, name, address, month, utilityCode, serviceCount));
                    line = reader.ReadLine();
                }
            }

            return residentsList;
        }

        static List<Utilities> ReadUtilitiesData(string file)
        {
            var utilitiesList = new List<Utilities>();
            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var values = line.Split(' ');
                    var serviceCode = values[0];
                    var serviceName = values[1];
                    var serviceUnitPrice = double.Parse(values[2]);
                    utilitiesList.AddData(new Utilities(serviceCode, serviceName, serviceUnitPrice));
                    line = reader.ReadLine();
                }
            }

            return utilitiesList;
        }

        static void CalculateMoneySpent(List<Utilities> utilitiesList, List<Residents> residentsList)
        {
            for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            {
                for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
                {
                    if (residentsList.GetData().UtilityCode == utilitiesList.GetData().ServiceCode)
                    {
                        residentsList.GetData().MoneySpent = utilitiesList.GetData().ServiceUnitPrice * residentsList.GetData().ServiceCount;
                    }
                }
            }
        }
        static double CalculateResidentsAllMoneySpent(List<Residents> residentsList)
        {
            var sum = 0.0;

            for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            {
                sum += residentsList.GetData().MoneySpent;
            }

            return sum;
        }

        static double CalculateResidentsAverageMoneySpent(List<Residents> residentsList)
        {
            var count = 0;

            for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            {
                count++;
            }

            return CalculateResidentsAllMoneySpent(residentsList) / count;
        }

        static List<Residents> FilterResidentsByMoneySpent(List<Residents> residentsList)
        {
            var filteredResidents = new List<Residents>();
            var averageMoneySpent = CalculateResidentsAverageMoneySpent(residentsList);

            for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            {
                if (averageMoneySpent > residentsList.GetData().MoneySpent)
                {
                    filteredResidents.AddData(residentsList.GetData());
                }
            }

            return filteredResidents;
        }

        static double FindUtilityPrice(List<Utilities> utilitiesList, string residentUtilityCode)
        {
            var utilityPrice = 0.0;
            for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
            {

                if (residentUtilityCode == utilitiesList.GetData().ServiceCode)
                {
                    utilityPrice = utilitiesList.GetData().ServiceUnitPrice;
                    break;
                }
            }
            return utilityPrice;
        }

        static string CalculateCheapestMonth(List<Residents> residentsList, List<Utilities> utilitiesList)
        {
            var cheapestMonth = new Dictionary<string, Dictionary<string, double>>();
            var cheapestMonthList = new List<CheapestMonth>();
            var temp = residentsList;
            for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            {
                if (cheapestMonth.ContainsKey(residentsList.GetData().Month))
                {
                    if (cheapestMonth[residentsList.GetData().Month].ContainsKey(residentsList.GetData().UtilityCode))
                    {

                        cheapestMonth[residentsList.GetData().Month][residentsList.GetData().UtilityCode] +=
                            FindUtilityPrice(utilitiesList, residentsList.GetData().UtilityCode) * residentsList.GetData().ServiceCount;
                    }
                    else
                    {
                        cheapestMonth[residentsList.GetData().Month].Add(
                       residentsList.GetData().UtilityCode,
                       FindUtilityPrice(utilitiesList, residentsList.GetData().UtilityCode) * residentsList.GetData().ServiceCount
                       );
                    }
                }
                else
                {
                    cheapestMonth.Add(residentsList.GetData().Month, new Dictionary<string, double>());
                    cheapestMonth[residentsList.GetData().Month].Add(
                        residentsList.GetData().UtilityCode,
                        FindUtilityPrice(utilitiesList, residentsList.GetData().UtilityCode) * residentsList.GetData().ServiceCount
                        );
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, double>> item in cheapestMonth)
            {
                foreach (KeyValuePair<string, double> utility in cheapestMonth[item.Key])
                {
                    cheapestMonthList.AddData(new CheapestMonth(item.Key, utility.Key, utility.Value));
                }
            }

            var cheapestMonthUtility = cheapestMonthList.OrderBy(x => x.Price).FirstOrDefault();

            for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
            {
                if (cheapestMonthUtility.UtilityCode == utilitiesList.GetData().ServiceCode)
                {
                    cheapestMonthUtility.UtilityCode = utilitiesList.GetData().ServiceName;
                }
            }
            if (cheapestMonthUtility != null)
            {
                return $"{cheapestMonthUtility.Month} mėnesį {cheapestMonthUtility.UtilityCode} kainavo pigiausiai";
            }
            return "Nerasta";
        }

        static void RemoveResidents(List<Residents> residentsList, List<Utilities> utilitiesList, string chosenMonth, string chosenUtility)
        {
            var index = 0;
            var utilityCode = "";

            for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
            {
                if (utilitiesList.GetData().ServiceName == chosenUtility)
                {
                    utilityCode = utilitiesList.GetData().ServiceCode;
                    break;
                }
            }
            //for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
            //{

            //    if (residentsList.GetData().Month != chosenMonth || residentsList.GetData().ServiceCount <= 0 || residentsList.GetData().UtilityCode != utilityCode)
            //    {
            //        index++;
            //    }
            //}
            //for (int i = 0; i < index; i++)
            //{
                for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
                {
                    if (residentsList.GetData().Month != chosenMonth || residentsList.GetData().ServiceCount <= 0 || residentsList.GetData().UtilityCode != utilityCode)
                    {
                        residentsList.RemoveNode(residentsList.GetNode());
                    }
                }
            //}
        }

        //static void PrintResultsTable(List<Utilities> utilitiesList, List<Residents> residentsList, string cheapestMonth, List<Residents> filteredResidentsList, string file)
        //{
        //    using (var writer = new StreamWriter(file, false, Encoding.UTF8))
        //    {
        //        writer.WriteLine("Pradiniai Duomenys");
        //        writer.WriteLine();
        //        writer.WriteLine("Komunalines Paslaugos");
        //        writer.WriteLine(new string('-', 80));
        //        writer.WriteLine("| {0, 15} | {1,25} | {2,30} |", "Paslaugos kodas", "Paslaugos pavadinimas", "Vieno menesio vieneto kaina");
        //        writer.WriteLine(new string('-', 80));
        //        for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
        //        {
        //            writer.WriteLine(utilitiesList.GetData().UtilitiesPrintToTable());
        //        }
        //        writer.WriteLine(new string('-', 80));
        //        writer.WriteLine();
        //        writer.WriteLine("Gyventoju duomenys");
        //        writer.WriteLine(new string('-', 119));
        //        writer.WriteLine("| {0,15} | {1,15} | {2,15} | {3,15} | {4,15} | {5,25} |", "Pavardė", "Vardas", "Adresas", "Mėnuo", "Paslaugos kodas", "Sunaudotų vienetų kiekis");
        //        writer.WriteLine(new string('-', 119));
        //        for (residentsList.StartingNode(); residentsList.Contains(); residentsList.RightNode())
        //        {
        //            writer.WriteLine(residentsList.GetData().ResidentsPrintToTable());
        //        }
        //        writer.WriteLine(new string('-', 119));

        //        writer.WriteLine("Rezultatai");
        //        writer.WriteLine();
        //        writer.WriteLine("Gyventojų sąrašas, kurie už komunalines paslaugas per metus mokėjo sumą, mažesnę už vidutinę, ir surikiuoti pagal pavardę ir vardą abėcėlės tvarka");
        //        writer.WriteLine(new string('-', 119));
        //        writer.WriteLine("| {0,15} | {1,15} | {2,15} | {3,15} | {4,15} | {5,25} |", "Pavardė", "Vardas", "Adresas", "Mėnuo", "Paslaugos kodas", "Sunaudotų vienetų kiekis");
        //        writer.WriteLine(new string('-', 119));
        //        for (filteredResidentsList.StartingNode(); filteredResidentsList.Contains(); filteredResidentsList.RightNode())
        //        {
        //            writer.WriteLine(filteredResidentsList.GetData().ResidentsPrintToTable());
        //        }
        //        writer.WriteLine(new string('-', 119));
        //        writer.WriteLine("Nustatytas menesis ir komunalines paslaugos, kainavusios pigiausiai");
        //        writer.WriteLine(cheapestMonth);
        //    }
        //}
    }
}
