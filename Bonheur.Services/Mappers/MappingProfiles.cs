﻿using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Bonheur.Utils.Constants;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Bonheur.Services.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // UserAccount
            CreateMap<CreateAccountDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ApplicationUser, UserAccountDTO>().ReverseMap();

            // UserRole
            CreateMap<ApplicationRole, UserRoleDTO>()
              .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
              .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
              .ReverseMap();
            CreateMap<UserRoleDTO, ApplicationRole>()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            // Permission
            CreateMap<ApplicationPermission, PermissionDTO>().ReverseMap();
            CreateMap<IdentityRoleClaim<string>, PermissionDTO>()
               .ConvertUsing(s => ((PermissionDTO)ApplicationPermissions.GetPermissionByValue(s.ClaimValue))!);

            // Claim
            CreateMap<IdentityRoleClaim<string>, ClaimDTO>()
              .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
              .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
              .ReverseMap();
        }
    }
}
