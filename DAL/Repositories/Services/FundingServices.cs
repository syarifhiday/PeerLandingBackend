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
    public class FundingServices : IFundingServices
    {
        private readonly PeerlandingContext _peerlandingContext;
        public FundingServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }
        public async Task<string> CreateFunding(ReqFundingDto funding)
        {
            var newFunding = new TrnFunding
            {
                loan_id = funding.loan_id,
                lender_id = funding.lender_id,
                amount = funding.amount,
            };

            await _peerlandingContext.AddAsync(newFunding);
            await _peerlandingContext.SaveChangesAsync();

            return newFunding.loan_id;
        }

        public async Task<List<ResListFundingDto>> FundingList(string? lender_id = null)
        {
            var fundingsQuery = _peerlandingContext.TrnFundings
                .Include(f => f.User)
                .OrderByDescending(funding => funding.funded_at)
                .Where(funding => lender_id == null || funding.lender_id == lender_id)
                .Select(funding => new ResListFundingDto
                {
                    Id = funding.Id,
                    loanId = funding.loan_id,
                    lenderId = funding.lender_id,
                    Amount = funding.amount,
                    fundedAt = funding.funded_at,
                });

            return await fundingsQuery.ToListAsync();
        }

    }
}
