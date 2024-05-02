using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // Hiển thị chi tiết danh mục sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        // Hiển thị form sửa đổi danh mục sản phẩm
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return Redirect("/admin/category/index");

            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            await _categoryRepository.DeleteAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Redirect("/admin/category/index");
        }

        [Authorize(Roles = SD.Role_Admin)]

        // Hiển thị form thêm mới danh mục sản phẩm
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm mới danh mục sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return Redirect("/admin/category/index");

            }
            return View(category);
        }
    }
}
