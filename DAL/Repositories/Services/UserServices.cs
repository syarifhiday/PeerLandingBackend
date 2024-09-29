using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(PeerlandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);

            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = (int)register.Balance,
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }
        
        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role != "admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                }).ToListAsync();
        }

        public async Task<ResDetailUserDto> DetailUser(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var resUser = new ResDetailUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                Balance = user.Balance
            };


            return resUser;
        }

        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if(!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            var loginResponse = new ResLoginDto
            {
                Token = token,
            };
            return loginResponse;
        }

        private string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Balance", user.Balance.ToString()),
                new Claim("Id", user.Id.ToString()), // Tambahkan Id sebagai claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ResDeleteDto> Delete(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();
            var deleteResponse = new ResDeleteDto
            {
                Message = "User deleted"
            };

            return deleteResponse;
            
        }

		public async Task<string> UpdateUserProfile(ReqUpdateUserProfileDto updateUserDto, string email, string id, string role)
		{
			try
			{
				// Cari user berdasarkan email atau Id dari token JWT
				var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
				if (user == null)
				{
					throw new Exception("User not found");
				}

				// Check role, if admin, allow updating name, role, and balance
				if (role == "admin")
				{
					user.Name = updateUserDto.Name;
					user.Role = updateUserDto.Role;
					user.Balance = updateUserDto.Balance;
				}
				else
				{
					// For non-admin, only allow name update
					user.Name = updateUserDto.Name;
				}

				_context.MstUsers.Update(user);
				await _context.SaveChangesAsync();

				return $"User {user.Name} has been updated successfully.";
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while updating the user profile: " + ex.Message);
			}
		}


        public async Task<ResUpdateBalanceDto> UpdateBalanceUser(ReqUpdateBalanceDto updateBalanceDto, string id)
        {
            try
            {
                // Cari user berdasarkan email atau Id dari token JWT
                var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.Balance += updateBalanceDto.balance;
                
                _context.MstUsers.Update(user);
                await _context.SaveChangesAsync();

                return new ResUpdateBalanceDto
                {
                    message = "Update balance successfully"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user profile: " + ex.Message);
            }
        }

        public async Task<ResUpdateBalanceDto> DecreaseBalanceUser(ReqUpdateBalanceDto updateBalanceDto, string id)
        {
            try
            {
                // Cari user berdasarkan email atau Id dari token JWT
                var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.Balance -= updateBalanceDto.balance;

                _context.MstUsers.Update(user);
                await _context.SaveChangesAsync();

                return new ResUpdateBalanceDto
                {
                    message = "Update balance successfully"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user profile: " + ex.Message);
            }
        }


    }
}
