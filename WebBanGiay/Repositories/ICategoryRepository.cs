using Microsoft.AspNetCore.Identity;


using System.Collections.Generic;

using WebBanGiay.Models;
public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(int id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}

