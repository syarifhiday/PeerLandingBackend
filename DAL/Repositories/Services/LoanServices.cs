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
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerlandingContext;
        public LoanServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }
        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerlandingContext.AddAsync(newLoan);
            await _peerlandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResListLoanDto>> LoanList(string? status = null)
        {
            var loansQuery = _peerlandingContext.MstLoans
                .Include(l => l.User)
                .OrderByDescending(loan => loan.CreatedAt)
                .Where(loan => status == null || loan.Status == status)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                });

            return await loansQuery.ToListAsync();
        }


        public async Task<string> UpdateLoan(ReqUpdateStatusLoanDto updateStatusLoanDto, string id)
        {
            try
            {
                // Cari user berdasarkan email atau Id dari token JWT
                var loan = await _peerlandingContext.MstLoans.SingleOrDefaultAsync(loan => loan.Id == id);
                if (loan == null)
                {
                    throw new Exception("Loan not found");
                }

                // Lakukan update seperti sebelumnya
                loan.Status = updateStatusLoanDto.status;
                loan.UpdatedAt = DateTime.UtcNow;

                await _peerlandingContext.SaveChangesAsync();

                return $"Loan with id={loan.Id} has been updated successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the loan status: " + ex.Message);
            }
        }
    }


    
}
