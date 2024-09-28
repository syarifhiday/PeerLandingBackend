using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResListFundingDto
    {
        public string Id {  get; set; }
        public string loanId { get; set; }
        public string lenderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime fundedAt { get; set; }
    }
}
