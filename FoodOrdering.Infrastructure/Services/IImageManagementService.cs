
using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Infrastructure.Services
{
    public interface IImageManagementService
    {
        Task<string> AddImageAsync(IFormFile file, string src);
        Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
        void DeleteImageAsync( string src);
    }
}
