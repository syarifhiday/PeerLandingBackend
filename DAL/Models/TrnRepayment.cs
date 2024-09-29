using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("trn_repayment")]
    public class TrnRepayment
    {
        [Key]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loans")]
        [Column("loan_id")]
        public string loan_id { get; set; }

        [Required]
        [Column("amount")]
        public decimal amount { get; set; }

        [Required]
        [Column("repaid_amount")]
        public decimal repaid_amount { get; set; }

        [Required]
        [Column("balance_amount")]
        public decimal balance_amount { get; set; }

        [Required]
        [Column("repaid_status")]
        public string repaid_status { get; set; } // on repay / done

        [Required]
        [Column("paid_at")]
        public DateTime paid_at { get; set; } = DateTime.UtcNow;

        public MstLoans Loans { get; set; }
    }
}
