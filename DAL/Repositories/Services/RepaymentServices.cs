using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlandingContext _peerlandingContext;
        public RepaymentServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }

        public async Task<string> CreateRepayment(ReqAddRepaymentDto reqRepaymentDto)
        {
            // Convert decimals to doubles for power calculation
            double interestRateDouble = (double)reqRepaymentDto.interest_rate;
            double amountDouble = (double)reqRepaymentDto.amount;
            double durationDouble = (int)reqRepaymentDto.duration;

            // Calculate installment (angsuran) using double values
            double angsuranDouble = (interestRateDouble / 100 * amountDouble) / (1 - (1 / Math.Pow(1 + interestRateDouble / 100, durationDouble)));
            // Convert result back to decimal
            decimal angsuran = Math.Round((decimal)angsuranDouble, 2);
            decimal totalBayar = Math.Round(angsuran * (decimal)durationDouble, 2);

            var newRepayment = new TrnRepayment
            {
                loan_id = reqRepaymentDto.loan_id,
                amount = totalBayar,
                repaid_amount = 0,
                balance_amount = totalBayar,
                repaid_status = "on_repay"
            };

            await _peerlandingContext.AddAsync(newRepayment);
            await _peerlandingContext.SaveChangesAsync();

            return newRepayment.loan_id;
        }


        public async Task<List<ResListRepaymentDto>> RepaymentListByBorrowerId(string? borrower_id = null)
        {
            var repaymentsQuery = _peerlandingContext.TrnRepayments
                .Include(repayment => repayment.Loans) // Join ke MstLoans
                .Where(repayment => borrower_id == null || repayment.Loans.BorrowerId == borrower_id) // Gunakan 'repayment'
                .Select(repayment => new ResListRepaymentDto
                {
                    Id = repayment.id,
                    LoanId = repayment.loan_id,
                    Amount = repayment.amount,
                    RepaidAmount = repayment.repaid_amount,
                    BalanceAmount = repayment.balance_amount,
                    InterestRate = repayment.Loans.InterestRate,
                    Duration = repayment.Loans.Duration,
                    RepaidStatus = repayment.repaid_status,
                    PaidAt = repayment.paid_at
                })
                .OrderByDescending(repayment => repayment.PaidAt);

            return await repaymentsQuery.ToListAsync();
        }


        public async Task<string> UpdateRepayment(ReqUpdateRepaymentDto reqRepaymentDto, string id)
        {
            
            try
            {
                // Cari user berdasarkan email atau Id dari token JWT
                var repayment = await _peerlandingContext.TrnRepayments.SingleOrDefaultAsync(repayment => repayment.id == id);
                if (repayment == null)
                {
                    throw new Exception("Repayment not found");
                }

                repayment.repaid_amount = repayment.repaid_amount + reqRepaymentDto.pay;
                repayment.balance_amount = repayment.balance_amount - reqRepaymentDto.pay;
                if(repayment.balance_amount < repayment.amount / 12)
                {
                    repayment.repaid_status = "done";
                }
                repayment.paid_at = DateTime.UtcNow;

                await _peerlandingContext.SaveChangesAsync();

                return $"Repayment with id={repayment.id} has been updated successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the repayment: " + ex.Message);
            }
        }
    }
}
