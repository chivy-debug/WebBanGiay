using WebBanGiay.Models;

namespace WebBanGiay.Repositories
{
    public interface ISearchService
    {
        Task<IEnumerable<Product>> GetAutoCompleteSuggestions(string searchString);
    }
}
