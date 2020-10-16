using SimchaChipIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaChipIn.Web.Models
{
    public class ContributorsIndexViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
    }
}
