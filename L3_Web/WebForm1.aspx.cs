using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace L3_Web
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string UtilitiesData = HttpContext.Current.Request.PhysicalApplicationPath + "/App_Data/U16a.txt";
        string ResidentsData = HttpContext.Current.Request.PhysicalApplicationPath + "/App_Data/U16b.txt";
        ListNodes<Residents> residentsListNodes = new ListNodes<Residents>();
        ListNodes<Utilities> utilitiesListNodes = new ListNodes<Utilities>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Label4.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {


            if (FileUpload1.HasFile && FileUpload1.FileName.EndsWith(".txt"))
            {
                utilitiesListNodes = ReadUtilitiesData((FileUpload1.FileContent));

            }

            if (FileUpload2.HasFile && FileUpload1.FileName.EndsWith(".txt"))
            {
                residentsListNodes = ReadResidentsData((FileUpload2.FileContent));
            }

            CalculateMoneySpent(utilitiesListNodes, residentsListNodes);
            var filteredResidentsListNodes = FilterResidentsByMoneySpent(residentsListNodes);
            var cheapestMonth = CalculateCheapestMonth(residentsListNodes, utilitiesListNodes);
            filteredResidentsListNodes.Sort();

            Session["StartingUtilities"] = utilitiesListNodes;
            Session["StartingResidents"] = residentsListNodes;

            UtilitiesTable(utilitiesListNodes);
            ResidentsTable(residentsListNodes, 0, cheapestMonth);
            ResidentsTable(filteredResidentsListNodes, 1, cheapestMonth);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var utilitiesListNodes = (ListNodes<Utilities>)Session["StartingUtilities"];
            var residentsListNodes = (ListNodes<Residents>)Session["StartingResidents"];

            var filteredResidentsListNodes = FilterResidentsByMoneySpent(residentsListNodes);
            var cheapestMonth = CalculateCheapestMonth(residentsListNodes, utilitiesListNodes);
            filteredResidentsListNodes.Sort();

            UtilitiesTable(utilitiesListNodes);
            ResidentsTable(residentsListNodes, 0, cheapestMonth);
            ResidentsTable(filteredResidentsListNodes, 1, cheapestMonth);

            var chosenMonth = TextBox1.Text;
            var chosenUtility = TextBox2.Text;
            if (chosenMonth == "" || chosenUtility == "")
            {
                Label4.Visible = true;
                Label4.Text = "Iveskite menesi ir komunaline paslauga!";
            }

            var removedResidents = RemoveResidents(filteredResidentsListNodes, utilitiesListNodes, chosenMonth, chosenUtility);
            if (removedResidents.ContainsAll())
            {
                ResidentsTable(removedResidents, 2, cheapestMonth);
            }
            else
            {
                if (Label4.Visible == true)
                {

                }
                else
                {
                    Label4.Visible = true;
                    Label4.Text = "Pašalinti visi mokėtojai";
                }
            }

            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\Report.txt";

            PrintReportTable(utilitiesListNodes, 0, filePath, cheapestMonth);
            PrintReportTable(residentsListNodes, 1, filePath, cheapestMonth);
            PrintReportTable(filteredResidentsListNodes, 2, filePath, cheapestMonth);
            PrintReportTable(removedResidents, 3, filePath, cheapestMonth);        
        }

        ListNodes<Residents> ReadResidentsData(Stream file)
        {
            var residentsListNodes = new ListNodes<Residents>();

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
                    residentsListNodes.AddData(new Residents(surname, name, address, month, utilityCode, serviceCount));
                    line = reader.ReadLine();
                }
            }

            return residentsListNodes;
        }

        ListNodes<Utilities> ReadUtilitiesData(Stream file)
        {
            var utilitiesListNodes = new ListNodes<Utilities>();
            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var values = line.Split(' ');
                    var serviceCode = values[0];
                    var serviceName = values[1];
                    var serviceUnitPrice = double.Parse(values[2]);
                    utilitiesListNodes.AddData(new Utilities(serviceCode, serviceName, serviceUnitPrice));
                    line = reader.ReadLine();
                }
            }

            return utilitiesListNodes;
        }

        static void CalculateMoneySpent(ListNodes<Utilities> utilitiesListNodes, ListNodes<Residents> residentsListNodes)
        {
            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                for (utilitiesListNodes.StartingNode(); utilitiesListNodes.Contains(); utilitiesListNodes.RightNode())
                {
                    if (residentsListNodes.GetData().UtilityCode == utilitiesListNodes.GetData().ServiceCode)
                    {
                        residentsListNodes.GetData().MoneySpent = utilitiesListNodes.GetData().ServiceUnitPrice * residentsListNodes.GetData().ServiceCount;
                    }
                }
            }
        }
        static double CalculateResidentsAllMoneySpent(ListNodes<Residents> residentsListNodes)
        {
            var sum = 0.0;

            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                sum += residentsListNodes.GetData().MoneySpent;
            }

            return sum;
        }

        static double CalculateResidentsAverageMoneySpent(ListNodes<Residents> residentsListNodes)
        {
            var count = 0;

            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                count++;
            }

            return CalculateResidentsAllMoneySpent(residentsListNodes) / count;
        }

        static ListNodes<Residents> FilterResidentsByMoneySpent(ListNodes<Residents> residentsListNodes)
        {
            var filteredResidents = new ListNodes<Residents>();
            var averageMoneySpent = CalculateResidentsAverageMoneySpent(residentsListNodes);

            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                if (averageMoneySpent > residentsListNodes.GetData().MoneySpent)
                {
                    filteredResidents.AddData(residentsListNodes.GetData());
                }
            }

            return filteredResidents;
        }

        static double FindUtilityPrice(ListNodes<Utilities> utilitiesListNodes, string residentUtilityCode)
        {
            var utilityPrice = 0.0;
            for (utilitiesListNodes.StartingNode(); utilitiesListNodes.Contains(); utilitiesListNodes.RightNode())
            {

                if (residentUtilityCode == utilitiesListNodes.GetData().ServiceCode)
                {
                    utilityPrice = utilitiesListNodes.GetData().ServiceUnitPrice;
                    break;
                }
            }
            return utilityPrice;
        }

        static string CalculateCheapestMonth(ListNodes<Residents> residentsListNodes, ListNodes<Utilities> utilitiesListNodes)
        {
            var cheapestMonth = new Dictionary<string, Dictionary<string, double>>();
            var cheapestMonthListNodes = new ListNodes<CheapestMonth>();
            var temp = residentsListNodes;
            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                if (cheapestMonth.ContainsKey(residentsListNodes.GetData().Month))
                {
                    if (cheapestMonth[residentsListNodes.GetData().Month].ContainsKey(residentsListNodes.GetData().UtilityCode))
                    {

                        cheapestMonth[residentsListNodes.GetData().Month][residentsListNodes.GetData().UtilityCode] +=
                            FindUtilityPrice(utilitiesListNodes, residentsListNodes.GetData().UtilityCode) * residentsListNodes.GetData().ServiceCount;
                    }
                    else
                    {
                        cheapestMonth[residentsListNodes.GetData().Month].Add(
                       residentsListNodes.GetData().UtilityCode,
                       FindUtilityPrice(utilitiesListNodes, residentsListNodes.GetData().UtilityCode) * residentsListNodes.GetData().ServiceCount
                       );
                    }
                }
                else
                {
                    cheapestMonth.Add(residentsListNodes.GetData().Month, new Dictionary<string, double>());
                    cheapestMonth[residentsListNodes.GetData().Month].Add(
                        residentsListNodes.GetData().UtilityCode,
                        FindUtilityPrice(utilitiesListNodes, residentsListNodes.GetData().UtilityCode) * residentsListNodes.GetData().ServiceCount
                        );
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, double>> item in cheapestMonth)
            {
                foreach (KeyValuePair<string, double> utility in cheapestMonth[item.Key])
                {
                    cheapestMonthListNodes.AddData(new CheapestMonth(item.Key, utility.Key, utility.Value));
                }
            }

            var cheapestMonthUtility = cheapestMonthListNodes.OrderBy(x => x.Price).FirstOrDefault();

            for (utilitiesListNodes.StartingNode(); utilitiesListNodes.Contains(); utilitiesListNodes.RightNode())
            {
                if (cheapestMonthUtility.UtilityCode == utilitiesListNodes.GetData().ServiceCode)
                {
                    cheapestMonthUtility.UtilityCode = utilitiesListNodes.GetData().ServiceName;
                }
            }
            if (cheapestMonthUtility != null)
            {
                return $"{cheapestMonthUtility.Month} mėnesį {cheapestMonthUtility.UtilityCode} kainavo pigiausiai";
            }
            return "Nerasta";
        }

        static ListNodes<Residents> RemoveResidents(ListNodes<Residents> residentsList, ListNodes<Utilities> utilitiesList, string chosenMonth, string chosenUtility)
        {
            var utilityCode = "";
            var removedResidents = residentsList;

            for (utilitiesList.StartingNode(); utilitiesList.Contains(); utilitiesList.RightNode())
            {
                if (utilitiesList.GetData().ServiceName == chosenUtility)
                {
                    utilityCode = utilitiesList.GetData().ServiceCode;
                    break;
                }
            }

            var index = 0;

            for (removedResidents.StartingNode(); removedResidents.Contains(); removedResidents.RightNode())
            {
                if (removedResidents.GetData().Month != chosenMonth || removedResidents.GetData().ServiceCount <= 0 || removedResidents.GetData().UtilityCode != utilityCode)
                {
                    index++;                  
                }
            }

            for (int i = 0; i < index; i++)
            {
                for (removedResidents.StartingNode(); removedResidents.Contains(); removedResidents.RightNode())
                {
                    if (removedResidents.GetData().Month != chosenMonth || removedResidents.GetData().ServiceCount <= 0 || removedResidents.GetData().UtilityCode != utilityCode)
                    {
                        removedResidents.RemoveNode(removedResidents.GetNodeInterface());
                    }
                }
            }

            return removedResidents;
        }

        void PrintReportTable<Type>(IEnumerable<Type> elements, int index, string filePath, string cheapestMonth) where Type : IComparable<Type>, IEquatable<Type>
        {
            string linesElementsUtilities = new string('-', 80);
            string linesElementsResidents = new string('-', 119);

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            using (var writer = new StreamWriter(filePath, true))
            {
                switch (index)
                {
                    case 0:
                        writer.WriteLine("Pradiniai duomenys");
                        writer.WriteLine();
                        writer.WriteLine("16a.txt");
                        writer.WriteLine();
                        writer.WriteLine(linesElementsUtilities);
                        writer.WriteLine("| {0, 15} | {1,25} | {2,30} |", "Paslaugos kodas", "Paslaugos pavadinimas", "Vieno menesio vieneto kaina");
                        writer.WriteLine(linesElementsUtilities);
                        break;
                    case 1:
                        writer.WriteLine();
                        writer.WriteLine("16b.txt");
                        writer.WriteLine();
                        writer.WriteLine(linesElementsResidents);
                        writer.WriteLine("| {0,15} | {1,15} | {2,15} | {3,15} | {4,15} | {5,25} |", "Pavardė", "Vardas", "Adresas", "Mėnuo", "Paslaugos kodas", "Sunaudotų vienetų kiekis");
                        writer.WriteLine(linesElementsResidents);
                        break;
                    case 2:
                        writer.WriteLine();
                        writer.WriteLine("Gyventojų sąrašas, kurie už komunalines paslaugas per metus mokėjo sumą, mažesnę už vidutinę, ir surikiuoti pagal pavardę ir vardą abėcėlės tvarka");
                        writer.WriteLine();
                        writer.WriteLine(linesElementsResidents);
                        writer.WriteLine("| {0,15} | {1,15} | {2,15} | {3,15} | {4,15} | {5,25} |", "Pavardė", "Vardas", "Adresas", "Mėnuo", "Paslaugos kodas", "Sunaudotų vienetų kiekis");
                        writer.WriteLine(linesElementsResidents);
                        break;
                    case 3:
                        writer.WriteLine();
                        writer.WriteLine("Gyventojai po pašalinimo, pašalinti buvo tie, kurie nemokėjo už nurodyta mėnesį ir paslaugą");
                        writer.WriteLine();
                        writer.WriteLine(linesElementsResidents);
                        writer.WriteLine("| {0,15} | {1,15} | {2,15} | {3,15} | {4,15} | {5,25} |", "Pavardė", "Vardas", "Adresas", "Mėnuo", "Paslaugos kodas", "Sunaudotų vienetų kiekis");
                        writer.WriteLine(linesElementsResidents);
                        break;
                    default:
                        break;
                }

                foreach (var element in elements)
                {
                    writer.WriteLine(element.ToString());
                }

                if (index == 0) writer.WriteLine(linesElementsUtilities);

                if (index == 1 || index == 2 || index == 3) writer.WriteLine(linesElementsResidents);
                if (index == 2)
                {
                    writer.WriteLine();
                    writer.WriteLine(cheapestMonth);
                }

                writer.WriteLine();
            }
        }

        void UtilitiesTable(ListNodes<Utilities> utilitiesListNodes)
        {
            var rowTop = new TableRow();

            for (int i = 0; i < 3; i++)
            {
                var cell = new TableCell();
                cell.BorderStyle = BorderStyle.Solid;
                cell.BorderWidth = 3;
                cell.Font.Size = 15;
                cell.Height = 30;
                cell.Width = 30;

                if (i == 0) cell.Text = "Paslaugos kodas";

                if (i == 1) cell.Text = "Paslaugos pavadinimas";

                if (i == 2) cell.Text = "Paslaugos kaina vnt.";

                rowTop.Cells.Add(cell);
            }
            Table1.Rows.Add(rowTop);

            for (utilitiesListNodes.StartingNode(); utilitiesListNodes.Contains(); utilitiesListNodes.RightNode())
            {
                var row = new TableRow();

                for (int i = 0; i < 3; i++)
                {
                    var cell = new TableCell();
                    cell.BorderStyle = BorderStyle.Solid;
                    cell.BorderWidth = 3;
                    cell.Font.Size = 15;
                    cell.Height = 30;
                    cell.Width = 30;

                    if (i == 0) cell.Text = utilitiesListNodes.GetData().ServiceCode;

                    if (i == 1) cell.Text = utilitiesListNodes.GetData().ServiceName;

                    if (i == 2) cell.Text = utilitiesListNodes.GetData().ServiceUnitPrice.ToString();

                    row.Cells.Add(cell);
                }

                Table1.Rows.Add(row);
            }
        }

        void ResidentsTable(ListNodes<Residents> residentsListNodes, int residentsChooser, string cheapestMonth)
        {
            var rowTop = new TableRow();

            for (int i = 0; i < 7; i++)
            {
                var cell = new TableCell();
                cell.BorderStyle = BorderStyle.Solid;
                cell.BorderWidth = 3;
                cell.Font.Size = 15;
                cell.Height = 30;
                cell.Width = 30;

                if (i == 0) cell.Text = "Pavarde";

                if (i == 1) cell.Text = "Vardas";

                if (i == 2) cell.Text = "Adresas";

                if (i == 3) cell.Text = "Menesis";

                if (i == 4) cell.Text = "Paslaugos kodas";

                if (i == 5) cell.Text = "Kiekis";

                if (i == 6) cell.Text = "Išleisti Pinigai";

                rowTop.Cells.Add(cell);
            }

            if (residentsChooser == 0)
            {
                Table2.Rows.Add(rowTop);

            }

            if (residentsChooser == 1)
            {
                Table3.Rows.Add(rowTop);

            }

            if (residentsChooser == 2)
            {
                Table4.Rows.Add(rowTop);

            }

            for (residentsListNodes.StartingNode(); residentsListNodes.Contains(); residentsListNodes.RightNode())
            {
                var row = new TableRow();

                for (int i = 0; i < 7; i++)
                {
                    var cell = new TableCell();
                    cell.BorderStyle = BorderStyle.Solid;
                    cell.BorderWidth = 3;
                    cell.Font.Size = 15;
                    cell.Height = 30;
                    cell.Width = 30;

                    if (i == 0) cell.Text = residentsListNodes.GetData().Surname;

                    if (i == 1) cell.Text = residentsListNodes.GetData().Name;

                    if (i == 2) cell.Text = residentsListNodes.GetData().Address;

                    if (i == 3) cell.Text = residentsListNodes.GetData().Month;

                    if (i == 4) cell.Text = residentsListNodes.GetData().UtilityCode;

                    if (i == 5) cell.Text = residentsListNodes.GetData().ServiceCount.ToString();

                    if (i == 6) cell.Text = residentsListNodes.GetData().MoneySpent.ToString();

                    row.Cells.Add(cell);
                }

                if (residentsChooser == 0) Table2.Rows.Add(row);
                if (residentsChooser == 1) Table3.Rows.Add(row);
                if (residentsChooser == 2) Table4.Rows.Add(row);
            }

            if (residentsChooser == 1)
            {
                var row = new TableRow();
                var cell = new TableCell();
                cell.Font.Size = 15;
                cell.Text = cheapestMonth;
                row.Cells.Add(cell);
                Table3.Rows.Add(row);
            }
        }
    }
}