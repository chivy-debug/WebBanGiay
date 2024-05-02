using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.DataAccess;
using WebBanGiay.Helper;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    [Authorize]
    [Authorize(Roles = "Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ShoppingCartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult AddToCart(int productId, int quantity, int size)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                // Xử lý khi không tìm thấy sản phẩm
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng từ Session hoặc tạo mới nếu không tồn tại
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng với cùng một productId và size hay không
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId && item.Size == size);

            if (existingItem != null)
            {
                // Nếu sản phẩm đã tồn tại với cùng một productId và size, tăng số lượng
                existingItem.Quantity += quantity;
            }
            else
            {
                // Nếu sản phẩm không tồn tại trong giỏ hàng, thêm sản phẩm mới vào giỏ hàng
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Size = size,
                    Price = product.Price,
                    Quantity = quantity
                };
                cart.AddItem(cartItem);
            }

            // Lưu giỏ hàng vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Đếm số lượng sản phẩm trong giỏ hàng
            int itemCount = CountItemsInCart(cart);

            // Gán số lượng sản phẩm vào ViewBag
            ViewBag.CartItemCount = itemCount;

            return RedirectToAction("Index", "Home");
        }

        // Hàm để đếm số lượng sản phẩm trong giỏ hàng
        private int CountItemsInCart(ShoppingCart cart)
        {
            if (cart == null || cart.Items == null)
            {
                return 0;
            }

            int count = 0;
            foreach (var item in cart.Items)
            {
                count += item.Quantity;
            }
            return count;
        }



        public IActionResult Checkout()
        {
            return View(new Order());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction("Index", "Product");
            }
            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("OrderCompleted", new { orderId = order.Id });

        }

        public IActionResult OrderCompleted(int orderId)
        {
            // Lấy thông tin đơn hàng từ cơ sở dữ liệu bằng orderId
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                // Xử lý khi không tìm thấy đơn hàng
                return RedirectToAction("Index", "Product");

            }
            return View(order);
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Kiểm tra nếu productId và quantity hợp lệ
            if (productId > 0 && quantity > 0)
            {
                // Lấy giỏ hàng từ Session
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

                // Tìm sản phẩm trong giỏ hàng
                var itemToUpdate = cart.Items.FirstOrDefault(item => item.ProductId == productId);

                // Kiểm tra xem sản phẩm có tồn tại không
                if (itemToUpdate != null)
                {
                    // Cập nhật số lượng sản phẩm
                    itemToUpdate.Quantity = quantity;

                    // Lưu giỏ hàng đã cập nhật vào Session
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            // Chuyển hướng lại trang giỏ hàng
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> OrderList()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _context.Orders
                                 .Where(o => o.UserId == user.Id)
                                 .ToList();

            if (!orders.Any())
            {
                // Xử lý khi không có đơn hàng nào
                ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                return View();
            }

            // Tạo ViewModel nếu muốn truyền thêm dữ liệu hoặc thông tin lý giải cho View
            return View(orders); // Gởi danh sách đơn hàng đến View
        }
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return RedirectToAction("OrderList");
            }

            // Trả về View với model là order đã tìm được
            return View(order);
        }
        public IActionResult RemoveFromCart(int productId)
        {
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
    }
}
