using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_funding")]
    public class TrnFunding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loans")]
        [Column("loan_id")]
        public string loan_id { get; set; }
        [Required]
        [ForeignKey("User")]
        [Column("lender_id")]
        public string lender_id { get; set; }
        [Required]
        [Column("amount")]
        public decimal amount { get; set; }
        [Required]
        [Column("funded_at")]
        public DateTime funded_at { get; set; } = DateTime.UtcNow;

        public MstUser User { get; set; }
        public MstLoans Loans { get; set; }
    }
}
