using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaChipIn.Data
{
    public class ContributionInclusion
    {
        public bool Include { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }
}
