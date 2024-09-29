﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResListRepaymentDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public decimal Amount { get; set; }
        public decimal RepaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public string RepaidStatus { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
