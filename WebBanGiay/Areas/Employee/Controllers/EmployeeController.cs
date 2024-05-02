using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

    }
}
