namespace WebBanGiay.Repositories

{
    using System.Collections.Generic;
    using WebBanGiay.Models;

    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId); // Định nghĩa phương thức này
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }



}
