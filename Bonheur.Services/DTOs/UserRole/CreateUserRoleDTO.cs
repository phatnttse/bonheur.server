using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Permission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Bonheur.Services.DTOs.UserRole
{
    public class CreateUserRoleDTO
    {
        [Required(ErrorMessage = "Role name is required"),
         StringLength(200, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 200 characters")]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public PermissionDTO[]? Permissions { get; set; }

    }
}
