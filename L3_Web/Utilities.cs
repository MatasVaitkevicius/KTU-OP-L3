using System;
using System.Collections.Generic;

namespace L3_Web
{
    public class Utilities : IComparable<Utilities>, IEquatable<Utilities>
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public double ServiceUnitPrice { get; set; }

        public Utilities(string serviceCode, string serviceName, double serviceUnitPrice)
        {
            ServiceCode = serviceCode;
            ServiceName = serviceName;
            ServiceUnitPrice = serviceUnitPrice;
        }

        public override string ToString()
        {
            return $"| {ServiceCode,15} | {ServiceName,-25} | {ServiceUnitPrice,30} |";
        }

        public IEnumerator<Utilities> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Utilities other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Utilities other)
        {
            throw new NotImplementedException();
        }
    }
}
