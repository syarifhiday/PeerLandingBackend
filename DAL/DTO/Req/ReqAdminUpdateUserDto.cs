using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace DAL.DTO.Req
{
    public class ReqAdminUpdateUserDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(30, ErrorMessage = "Role cannot exceed 30 characters")]
        public string Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be positive")]
        public int? Balance { get; set; }
    }
}
