using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);
        Task<List<ResUserDto>> GetAllUsers();
        Task<ResLoginDto> Login(ReqLoginDto reqLogin);
        Task<ResDeleteDto> Delete(string id);

        //Task<string> AdminUpdateUser(ReqAdminUpdateUserDto updateUserDto, string Id);
        Task<ResDetailUserDto> DetailUser(string id);
        Task<ResUpdateBalanceDto> UpdateBalanceUser(ReqUpdateBalanceDto reqUpdateBalanceDto, string id);
        Task<ResUpdateBalanceDto> DecreaseBalanceUser(ReqUpdateBalanceDto reqUpdateBalanceDto, string id);


        Task<string> UpdateUserProfile(ReqUpdateUserProfileDto updateUserProfileDto, string email, string id, string role);
    }
}
