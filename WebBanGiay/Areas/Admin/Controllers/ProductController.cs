using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;
using WebBanGiay.Repositories;
using X.PagedList;


namespace WebBanGiay.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles=SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository, ApplicationDbContext dBcontext)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = dBcontext;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // L?y t?t c? s?n ph?m t? repository
            var products = await _productRepository.GetAllAsync();

            // L?y t?t c? danh m?c t? repository
            var categories = await _categoryRepository.GetAllAsync();

            // L?c s?n ph?m n?u có t? khóa tìm ki?m
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }

            // Phân trang
            int pageSize = 6;
            int pageNumber = page ?? 1;

            // S?p x?p s?n ph?m theo Id
            var sortedProducts = products.OrderBy(p => p.Id);

            // Ki?m tra xem có s?n ph?m không
            if (sortedProducts.Count() == 0)
            {
                // N?u không có s?n ph?m, chuy?n h??ng ??n trang "Empty"
                return RedirectToAction("Empty");
            }

            // Truy?n danh sách danh m?c s?n ph?m sang view
            ViewBag.Categories = categories;

            // Tr? v? view v?i danh sách s?n ph?m, danh sách danh m?c và phân trang
            return View(sortedProducts.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public JsonResult AutoComplete(string search)
        {
            var Result = _context.Products.Where(x => x.Name.Contains(search))
                                        .Select(x => new {
                                            label = x.Name,
                                            value = x.Name
                                        }).ToList();
            return Json(Result);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.AddAsync(product);
                return Redirect("/admin/product/index");

            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
		{
			var product = await _productRepository.GetByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",

            product.CategoryId);

            return View(product);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)

        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl

            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await

                _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên

                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);
                return Redirect("/admin/product/index");
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Redirect("/admin/product/index");

        }

	}
}
