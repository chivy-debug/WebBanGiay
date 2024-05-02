using Microsoft.EntityFrameworkCore;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;

namespace WebBanGiay.Repositories
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAutoCompleteSuggestions(string searchString)
        {
            // Truy vấn cơ sở dữ liệu để lấy các gợi ý tìm kiếm dựa trên searchString
            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(searchString))
                .Select(p => p.Name)
                .Take(5) // Lấy tối đa 5 gợi ý
                .ToListAsync();

            return (IEnumerable<Product>)suggestions;
        }
    }
}
