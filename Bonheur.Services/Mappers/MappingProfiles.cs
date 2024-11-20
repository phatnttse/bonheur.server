using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.SupplierCategory;
using Bonheur.Services.DTOs.UserAccount;
using Bonheur.Services.DTOs.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // UserAccount
            CreateMap<CreateAccountDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserAccountDTO>();

            // UserRole
            CreateMap<CreateUserRoleDTO, ApplicationRole>();

            // SupplierCategory
            CreateMap<SupplierCategoryDTO, SupplierCategory>();

            CreateMap<SupplierCategory, SupplierCategoryDTO>();

            CreateMap<CreateSupplierCategoryDTO, SupplierCategory>();
        }
    }
}
