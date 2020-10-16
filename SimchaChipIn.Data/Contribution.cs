using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaChipIn.Data
{
    public class Contribution
    {
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
    }
}
