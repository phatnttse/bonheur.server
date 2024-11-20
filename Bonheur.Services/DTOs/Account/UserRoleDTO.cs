using Bonheur.BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace Bonheur.Services.DTOs.Account
{
    public class UserRoleDTO
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Role name is required"),
         StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public int? UsersCount { get; set; }

        public PermissionDTO[]? Permissions { get; set; }

    }
}
