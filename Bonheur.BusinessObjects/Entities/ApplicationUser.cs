using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonheur.BusinessObjects.Enums;
using Microsoft.AspNetCore.Identity;


namespace Bonheur.BusinessObjects.Entities
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        public string? FullName { get; set; }
        public string? PartnerName { get; set; }
        public string? PictureUrl { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsLockedOut => LockoutEnabled && LockoutEnd >= DateTimeOffset.UtcNow;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();
    }
}
