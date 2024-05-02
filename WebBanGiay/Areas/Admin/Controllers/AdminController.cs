using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> CreateAdminAccount()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var user = new IdentityUser
            {
                UserName = "Admin@gmail.com",//Thay đổi tùy ý
                Email = "Admin@gmail.com"//Thay đổi tùy ý

            };
            var result = await _userManager.CreateAsync(user, "Admin@123");//Chú ý format mật khẩu
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content("Admin account created successfully!");

            }
            return BadRequest("Failed to create admin account");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
