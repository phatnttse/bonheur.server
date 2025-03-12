using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.BlogPost;
using Bonheur.Services.DTOs.BlogTag;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Bonheur.Services
{
    public class BlogTagService : IBlogTagService
    {
        private readonly IBlogTagRepository _blogTagRepository;
        private readonly IMapper _mapper;

        public BlogTagService(IBlogTagRepository blogTagRepository, IMapper mapper)
        {
            _blogTagRepository = blogTagRepository;
            _mapper = mapper;
        }
        public async Task<ApplicationResponse> CreateBlogTagAsync(BlogTagDTO blogTagDTO)
        {
            try
            {
                var blogTag = _mapper.Map<BlogTag>(blogTagDTO);
                await _blogTagRepository.CreateBlogTagAsync(blogTag);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Blog Tag created successfully",
                    Data = blogTag,
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

        public async Task<ApplicationResponse> DeleteBlogTagAsync(int id)
        {
            try
            {
                var blogTag = await _blogTagRepository.GetBlogTagByIdAsync(id) ?? throw new ApiException("Blog tag does not exist!");   
                await _blogTagRepository.DeleteBlogTagAsync(id);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Blog tag deleted successfully",
                    Data = null,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<ApplicationResponse> GetBlogTagByIdAsync(int id)
        {
            try
            {
               var blogTag = await _blogTagRepository.GetBlogTagByIdAsync(id) ?? throw new ApiException("Blog Tag does not existed");
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query blog tag successfully",
                    Data = blogTag,
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

        public async Task<ApplicationResponse> GetBlogTagsAsync()
        {
            try
            {
                var blogTags = await _blogTagRepository.GetBlogTagsAsync();
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query blog tags successfully",
                    Data = blogTags,
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

        public async Task<ApplicationResponse> UpdateBlogTagAsync(int id, BlogTagDTO blogTagDTO)
        {
            try
            {
                var blogTagEntity = await _blogTagRepository.GetBlogTagByIdAsync(id) ?? throw new ApiException("Blog tag does not existed!");
                _mapper.Map(blogTagEntity, blogTagDTO);
                await _blogTagRepository.UpdateBlogTagAsync(blogTagEntity);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Update blog tag successfully!",
                    Data = null,
                    StatusCode = System.Net.HttpStatusCode.OK,
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
