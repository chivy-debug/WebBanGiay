using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;
using WebBanGiay.Repositories;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using X.PagedList;
using System.Net.Mail;
using System.Net;

namespace WebBanGiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly ISearchService _searchService;


        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository,
        ICategoryRepository categoryRepository, ApplicationDbContext dBcontext, ISearchService searchService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = dBcontext;
            _searchService = searchService;
        }



        // Hàm Index hi?n th? danh sách s?n ph?m và tìm ki?m
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
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult LienHe()
        {
            return View();
        }
        public IActionResult HoTro()
        {
            return View();
        }
        public IActionResult Empty()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult QuyDinh()
        {
            return View();
        }

        public IActionResult Thanhtoan()
        {
            return View();
        }

        public IActionResult VanChuyen()
        {
            return View();
        }

        public IActionResult DoiTra()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public async Task<IActionResult> ShowCategory()
        {
            // L?y t?t c? các danh m?c t? repository
            var categories = await _categoryRepository.GetAllAsync();

            // S?p x?p danh sách các danh m?c theo Id
            var sortedCategories = categories.OrderBy(c => c.Id).ToList();

            // Tr? v? view v?i danh sách các danh m?c ?ã s?p x?p
            return PartialView(sortedCategories);

        }

        public async Task<IActionResult> SanPhamTheoCategory(int categoryId)
        {
            // L?y t?t c? các danh m?c t? repository
            var categories = await _categoryRepository.GetAllAsync();
            // Truy?n danh sách danh m?c s?n ph?m sang view
            ViewBag.Categories = categories;

            var productsInCategory = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return View("SanPhamTheoCategory", productsInCategory);
        }
        public async Task<IActionResult> ChiTietSanPham(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
       [HttpPost]
        public IActionResult SendMessage(string name, string email, string message)
        {
            try
            {
                // T?o ??i t??ng MailMessage
                MailMessage mail = new MailMessage();
                
                // ??a ch? email c?a ng??i g?i
                mail.From = new MailAddress(email);
                
                // ??a ch? email nh?n
                mail.To.Add("chivy10112003@gmail.com");
                
                // Tiêu ?? email
                mail.Subject = "Yêu c?u h? tr? t? " + name;
                
                // N?i dung email
                mail.Body = "Tên: " + name + "\n" +
                            "Email: " + email + "\n" +
                            "N?i dung:\n" + message;

                // C?u hình SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("chivy10112003@gmail.com", "vyduong1011");
                smtpClient.EnableSsl = true;

                // G?i email
                smtpClient.SendMailAsync(mail);

                // Tr? v? k?t qu? thành công
                return Content("Email ?ã g?i thành công!");
            }

            catch (Exception ex)
            {
                // Tr? v? l?i n?u g?i email không thành công
                return Content("Có l?i x?y ra: " + ex.Message);
            }
        }
    }
}
