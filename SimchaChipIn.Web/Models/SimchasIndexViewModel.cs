using SimchaChipIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaChipIn.Web.Models
{
    public class SimchasIndexViewModel
    {
        public int TotalContributorCount { get; set; }
        public IEnumerable<Simcha> Simchas { get; set; }
    }
}
