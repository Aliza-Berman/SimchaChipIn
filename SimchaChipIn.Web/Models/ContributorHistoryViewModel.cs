using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaChipIn.Web.Models
{
    public class ContributorHistoryViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
