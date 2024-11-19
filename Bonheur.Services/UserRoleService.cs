using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.UserRole;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;

namespace Bonheur.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;


        public UserRoleService(IUserRoleRepository userRoleRepository, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<ApplicationResponse> CreateRoleAsync(CreateUserRoleDTO createUserRoleDTO, IEnumerable<string> claims)
        {
            try
            {
                var role = _mapper.Map<ApplicationRole>(createUserRoleDTO);

                var existingRole = await _userRoleRepository.GetRoleByNameAsync(role.Name!);

                if (existingRole != null)
                {
                    throw new ApiException("Role already exists", System.Net.HttpStatusCode.BadRequest);
                }

                var result = await _userRoleRepository.CreateRoleAsync(role, claims);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                return new ApplicationResponse
                {
                    Message = $"Role {createUserRoleDTO.Name} created successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK

                };

            } catch (ApiException)
            {
                throw;

            } catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public Task<ApplicationResponse> DeleteRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationResponse> GetRoleByIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationResponse> GetRoleByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationResponse> GetRoleLoadRelatedAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationResponse> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            try
            {
                var result = await _userRoleRepository.GetRolesLoadRelatedAsync(page, pageSize);

                if (result == null)
                {
                    throw new ApiException("No roles found", System.Net.HttpStatusCode.NotFound);
                }

                return new ApplicationResponse
                {
                    Data = result,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public Task<ApplicationResponse> UpdateRoleAsync(ApplicationRole role, IEnumerable<string>? claims)
        {
            throw new NotImplementedException();
        }
    }
}
