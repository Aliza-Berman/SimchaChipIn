using SimchaChipIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaChipIn.Web.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public IEnumerable<SimchaContributor> Contributors { get; set; }
    }
}
