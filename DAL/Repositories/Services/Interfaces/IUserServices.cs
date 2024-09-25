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

        Task<string> AdminUpdateUser(ReqAdminUpdateUserDto updateUserDto, string Id);

        Task<string> UpdateUserProfile(ReqUpdateUserProfileDto updateUserProfileDto, string Id, string role);
    }
}
