﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaChipIn.Data
{
    public class SimchaContributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal? Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
