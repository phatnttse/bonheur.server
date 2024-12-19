using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System.Data;
using static Bonheur.Utils.Constants;

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

        public async Task<ApplicationResponse> CreateRoleAsync(UserRoleDTO createUserRoleDTO, IEnumerable<string> claims)
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

            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _userRoleRepository.GetRoleByIdAsync(roleId);

                if (role == null)
                {
                    throw new ApiException("Role not found", System.Net.HttpStatusCode.NotFound);
                }

                var roleData = _mapper.Map<UserRoleDTO>(role);

                return new ApplicationResponse
                {
                    Message = $"Role {role.Name} found",
                    Data = roleData,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var role = await _userRoleRepository.GetRoleByNameAsync(roleName);

                var rolesData = _mapper.Map<UserRoleDTO>(role);

                return new ApplicationResponse
                {
                    Message = $"Role {roleName} found",
                    Data = rolesData,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetRoleLoadRelatedAsync(string roleName)
        {

            try
            {
                var role = await _userRoleRepository.GetRoleLoadRelatedAsync(roleName);

                if (role == null)
                {
                    throw new ApiException("Role not found", System.Net.HttpStatusCode.NotFound);
                }

                var roleData = _mapper.Map<UserRoleDTO>(role);

                return new ApplicationResponse
                {
                    Message = $"Role {role.Name} found",
                    Data = roleData,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            try
            {
                var roles = await _userRoleRepository.GetRolesLoadRelatedAsync(page, pageSize);

                if (roles == null)
                {
                    throw new ApiException("No roles found", System.Net.HttpStatusCode.NotFound);
                }

                var rolesData = _mapper.Map<List<UserRoleDTO>>(roles);

                return new ApplicationResponse
                {
                    Data = rolesData,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public ApplicationResponse GetAllPermissions()
        {
            try
            {
                var permissions = _mapper.Map<List<PermissionDTO>>(ApplicationPermissions.AllPermissions);

                return new ApplicationResponse
                {
                    Data = permissions,
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApplicationResponse> UpdateRoleAsync(string id, UserRoleDTO userRoleDTO, IEnumerable<string>? claims)
        {
            try
            {
                if (userRoleDTO == null)
                {
                    throw new ApiException("Role cannot be null", System.Net.HttpStatusCode.BadRequest);
                }

                var existingRole = await _userRoleRepository.GetRoleByIdAsync(id);

                if (existingRole == null)
                {
                    throw new ApiException("Role not found", System.Net.HttpStatusCode.NotFound);
                }

                _mapper.Map(userRoleDTO, existingRole);

                var result = await _userRoleRepository.UpdateRoleAsync(existingRole, claims);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                return new ApplicationResponse
                {
                    Message = $"Role {userRoleDTO.Name} updated successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApplicationResponse> DeleteRoleAsync(string id)
        {
            try
            {
                var existingRole = await _userRoleRepository.GetRoleByIdAsync(id);

                if (existingRole == null)
                {
                    throw new ApiException("Role not found", System.Net.HttpStatusCode.NotFound);
                }

                var result = await _userRoleRepository.DeleteRoleAsync(existingRole);

                if (!result.Succeeded)
                {
                    throw new ApiException(string.Join("; ", result.Errors.Select(error => error)), System.Net.HttpStatusCode.BadRequest);
                }

                return new ApplicationResponse
                {
                    Message = $"Role {existingRole.Name} deleted successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
