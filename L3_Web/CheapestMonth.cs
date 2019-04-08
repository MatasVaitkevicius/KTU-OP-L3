using System;

namespace L3_Web
{
    class CheapestMonth : IComparable<CheapestMonth>, IEquatable<CheapestMonth>
    {
        public string Month { get; set; }
        public string UtilityCode { get; set; }
        public double Price { get; set; }

        public CheapestMonth(string month, string utilityCode, double price)
        {
            Month = month;
            UtilityCode = utilityCode;
            Price = price;
        }

        public int CompareTo(CheapestMonth other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(CheapestMonth other)
        {
            throw new NotImplementedException();
        }
    }
}
