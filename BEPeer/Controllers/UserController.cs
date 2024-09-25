using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("api/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
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
                var res = await _userServices.Register(register);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if(ex.Message == "Email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userServices.Login(loginDto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User login success",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetAllUsers();
                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResUserDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ReqUpdateUserProfileDto updateUserDto)
        {
            try
            {
                // Mendapatkan email dari token JWT
                var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (email == null)
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "Unauthorized access",
                        Data = null
                    });
                }

                // Mengupdate user berdasarkan email yang diperoleh dari token
                var result = await _userServices.UpdateUserProfile(updateUserDto, email);
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



        //Update users method
        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminUpdateUser(string Id, ReqAdminUpdateUserDto updateUser)
        {
            try
            {
                var result = await _userServices.AdminUpdateUser(updateUser, Id);
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


        [HttpDelete]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _userServices.Delete(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User Deleted",
                });
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResBaseDto<object>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
        }

    }
}
