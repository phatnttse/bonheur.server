﻿using Bonheur.BusinessObjects.Enums;


namespace Bonheur.Services.DTOs.Account
{
    public class UserAccountDTO
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? PartnerName { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PictureUrl { get; set; }
        public Gender? Gender { get; set; }
        public bool? IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool? IsEnabled { get; set; }
        public string[]? Roles { get; set; }

    }
}
