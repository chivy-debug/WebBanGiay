using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;


namespace WebBanGiay.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> OrderList()
        {
            var users = await _userManager.Users.ToListAsync();
            var allOrders = new List<Order>();

            foreach (var user in users)
            {
                var userOrders = await _context.Orders
                                                .Where(o => o.UserId == user.Id)
                                                .ToListAsync();
                allOrders.AddRange(userOrders);
            }

            if (!allOrders.Any())
            {
                // Xử lý khi không có đơn hàng nào
                ViewBag.Message = "Hiện tại không có đơn hàng nào.";
                return View();
            }
            // Tính tổng doanh thu
            decimal TongDoanhThu = _context.Orders.Sum(o => o.TotalPrice);

            ViewBag.TongDoanhThu = TongDoanhThu; // Truyền tổng doanh thu vào ViewBag

            // Tạo ViewModel nếu muốn truyền thêm dữ liệu hoặc thông tin lý giải cho View
            return View(allOrders); // Gởi danh sách đơn hàng đến View
        }
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _context.Orders
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Product)
                                        .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return RedirectToAction("Employee/Home/OrderList");
            }

            // Trả về View với model là order đã tìm được
            return View(order);
        }
    }
}
