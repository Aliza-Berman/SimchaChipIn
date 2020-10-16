using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaChipIn.Data
{
    public class Deposit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }
}
