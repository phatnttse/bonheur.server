using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Supplier;
using Microsoft.AspNetCore.Identity;
using Bonheur.Services.DTOs.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Bonheur.Utils.Constants;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Bonheur.Services.DTOs.RequestPricing;

namespace Bonheur.Services.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // UserAccount
            CreateMap<CreateAccountDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserAccountDTO>()
                  .ForMember(d => d.Roles, map => map.Ignore()).ReverseMap();

            CreateMap<UserAccountDTO, ApplicationUser>()
                 .ForMember(dest => dest.Roles, opt => opt.Ignore())
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateUserProfileDTO, ApplicationUser>()
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserAccountStatusDTO, ApplicationUser>().ReverseMap();

            CreateMap<GoogleAccountDTO, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));


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
            CreateMap<UserRoleDTO, ApplicationRole>();

            // SupplierCategory
            CreateMap<SupplierCategoryDTO, SupplierCategory>();

            CreateMap<SupplierCategory, SupplierCategoryDTO>();

            CreateMap<CreateSupplierCategoryDTO, SupplierCategory>();

            // Supplier
            CreateMap<CreateSupplierDTO, Supplier>();
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
            CreateMap<UpdateSupplierProfileDTO, Supplier>();
            CreateMap<UpdateSupplierAddressDTO, Supplier>();

            // SupplierImage
            CreateMap<SupplierImage, SupplierImageDTO>().ReverseMap();

            //RequestPricing
            CreateMap<CreateRequestPricingDTO, RequestPricing>();
            CreateMap<RequestPricing, RequestPricingsDTO>();
            CreateMap<RequestPricingsDTO, RequestPricing>();


        }
    }
}
