using Bonheur.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class PermissionDTO
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }

        [return: NotNullIfNotNull(nameof(permission))]
        public static explicit operator PermissionDTO?(ApplicationPermission? permission)
        {
            if (permission == null)
                return null;

            return new PermissionDTO
            {
                Name = permission.Name,
                Value = permission.Value,
                GroupName = permission.GroupName,
                Description = permission.Description
            };
        }
    }
}
