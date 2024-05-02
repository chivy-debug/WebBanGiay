using WebBanGiay.Models;
using X.PagedList;

namespace WebBanGiay.Controllers
{
    internal class ProductCategoryViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}