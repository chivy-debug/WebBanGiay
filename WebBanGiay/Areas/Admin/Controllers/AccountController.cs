using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRolesModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRolesModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(usersWithRoles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Forgot Password
        [AllowAnonymous]
        [HttpGet("forgot-password-admin")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("forgot-password-admin")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Redirect("/Admin/Account/index");
            }
            else
            {
                // Handle errors if deletion fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Index", await _userManager.Users.ToListAsync());
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles.ToList(),
                AllRoles = allRoles
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(EditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Remove current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add new roles
                foreach (var role in model.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                return Redirect("/Admin/account/index");
            }
            else
            {
                // Handle errors if update fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
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
                return RedirectToAction("Admin/Account/OrderList");
            }

            // Trả về View với model là order đã tìm được
            return View(order);
        }



    }
}
