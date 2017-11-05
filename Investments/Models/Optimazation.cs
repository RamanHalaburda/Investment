using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Investments.Models
{
    public class Optimazation
    {
        public int ID { get; set; }
        public DateTime date { get; set; }
        public int dividentsA { get; set; }
        public int dividentsB { get; set; }
        public int sum { get; set; }
        public int limitA { get; set; }
        public int limitB { get; set; }
        public int investmentA { get; set; }
        public int investmentB { get; set; }
        public int result { get; set; }
    }
}
