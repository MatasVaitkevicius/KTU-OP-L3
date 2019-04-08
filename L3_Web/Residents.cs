using System;

namespace L3_Web
{
    public class Residents : IComparable<Residents>, IEquatable<Residents>
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Month { get; set; }
        public string UtilityCode { get; set; }
        public int ServiceCount { get; set; }
        public double MoneySpent { get; set; }

        public Residents() { }

        public Residents(string surname, string name, string address, string month, string utilityCode, int serviceCount)
        {
            Surname = surname;
            Name = name;
            Address = address;
            Month = month;
            UtilityCode = utilityCode;
            ServiceCount = serviceCount;
        }

        public override string ToString()
        {
            return $"| {Surname,-15} | {Name,-15} | {Address,-15} | {Month,-15} | {UtilityCode,15} | {ServiceCount,25} |";
        }

        public int CompareTo(Residents nextResident)
        {
            if (nextResident == null)
            {
                return 1;
            }
            if (Surname == nextResident.Surname)
            {
                return Name.CompareTo(nextResident.Name);
            }
            else
            {
                return Surname.CompareTo(nextResident.Surname);
            }
        }

        public bool Equals(Residents nextResidents)
        {
            if (nextResidents == null)
            {
                return false;
            }
            if (Surname == nextResidents.Surname && Name == nextResidents.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
