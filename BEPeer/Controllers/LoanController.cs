using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BEPeer.Controllers
{
    [Route("api/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }
        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loanDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _loanServices.CreateLoan(loanDto);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success add loan data",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusLoan(string Id, ReqUpdateStatusLoanDto updateStatusLoanDto)
        {
            try
            {
                var result = await _loanServices.UpdateLoan(updateStatusLoanDto, Id);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = result,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoanList([FromQuery] string? status = null)
        {
            try
            {
                var res = await _loanServices.LoanList(status);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success getting loans",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> RequestedLoanByBorrowerId([FromQuery] string? borrower_id = null)
        {
            try
            {
                var res = await _loanServices.RequestedLoanByBorrowerId(borrower_id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success getting loans",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

    }
}
