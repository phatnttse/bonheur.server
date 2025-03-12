using AutoMapper;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
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
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMapper _mapper;
        public BlogPostService(IBlogPostRepository blogPostRepository, IMapper mapper)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
        }
        public async Task<ApplicationResponse> AddBlogPostAsync(BlogPostDTO blogPostDTO)
        {
            var blogPost = _mapper.Map<BlogPost>(blogPostDTO);
            await _blogPostRepository.AddBlogPostAsync(blogPost);
            return new ApplicationResponse
            {
                Success = true,
                Message = "Adding blog post async!",
                Data = null,
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public async Task<ApplicationResponse> DeleteBlogPostAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetBlogPostByIdAsync(id) ?? throw new ApiException("Blog post does not existed!");
            await _blogPostRepository.DeleteBlogPostAsync(blogPost);
            return new ApplicationResponse {
                Success = true,
                Message = "Delete blog post async successfully!",
                Data = null,
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public async Task<ApplicationResponse> GetBlogPostByIdAsync(int id)
        {
            try
            {
                var blogPost = await _blogPostRepository.GetBlogPostByIdAsync(id);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Query blog post successfully",
                    Data = blogPost,
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

        public async Task<ApplicationResponse> GetBlogPostsAsync(string? searchTitle, string? searchContent, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var blogPostsPagedList = await _blogPostRepository.GetBlogPostsAsync(searchTitle, searchContent, pageNumber, pageSize);
                var listBlogPostDTO = _mapper.Map<List<BlogPostDTO>>(blogPostsPagedList);

                var responseData = new PagedData<BlogPostDTO>
                {
                    Items = listBlogPostDTO,
                    PageNumber = blogPostsPagedList.PageNumber,
                    PageSize = blogPostsPagedList.PageSize,
                    TotalItemCount = blogPostsPagedList.TotalItemCount,
                    PageCount = blogPostsPagedList.PageCount,
                    IsFirstPage = blogPostsPagedList.IsFirstPage,
                    IsLastPage = blogPostsPagedList.IsLastPage,
                    HasNextPage = blogPostsPagedList.HasNextPage,
                    HasPreviousPage = blogPostsPagedList.HasPreviousPage
                };

                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Blog posts retrieved successfully",
                    Data = responseData,
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


        public async Task<ApplicationResponse> GetBlogPostsByTags(List<BlogTagDTO> tags, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var blogTags = _mapper.Map<List<BlogTag>>(tags);
                var blogPosts = await _blogPostRepository.GetBlogPostsByTagsAsync(blogTags, pageNumber, pageSize);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Blog posts retrieved successfully",
                    Data = blogPosts,
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

        public async Task<ApplicationResponse> UpdateBlogPostAsync(int id, BlogPostDTO blogPostDTO)
        {
            try
            {
                var blogPostEntity = await _blogPostRepository.GetBlogPostByIdAsync(id) ?? throw new ApiException("Blog post does not existed!");
                _mapper.Map(blogPostEntity, blogPostDTO);
                await _blogPostRepository.UpdateBlogPostAsync(blogPostEntity);
                return new ApplicationResponse
                {
                    Success = true,
                    Message = "Update blog post successfully!",
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
