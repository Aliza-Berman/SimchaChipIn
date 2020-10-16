using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaChipIn.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
    }
}
