using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using WebThuCung.Dto;
using WebThuCung.Data;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class ShipperController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public ShipperController(PetContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId)  // Lọc theo shipperId
                .Include(so => so.Order)
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Select(so => new ShipperOrderDto
                {
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                // Tìm kiếm shipper trong cơ sở dữ liệu
                var ad = await _context.Shippers.Include(a => a.Role)
                                               .SingleOrDefaultAsync(n => n.userShipper == model.userName && n.passwordShipper == model.password);

                if (ad != null)
                {
                    TempData["success"] = "Đăng nhập thành công";

                    // Tạo Claims cho người dùng
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad.userShipper),
                new Claim(ClaimTypes.Email, ad.Email),
                new Claim(ClaimTypes.Role, ad.Role.Name) // Gán vai trò cho người dùng
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Lưu lại thông tin đăng nhập
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Thời gian hết hạn
                    };

                    // Đăng nhập cho người dùng
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                   new ClaimsPrincipal(claimsIdentity),
                                                   authProperties);
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    var shipperJson = Newtonsoft.Json.JsonConvert.SerializeObject(ad, settings);

                    HttpContext.Session.SetString("TaikhoanShipper", shipperJson);
                    HttpContext.Session.SetString("email", ad.Email);
                    return RedirectToAction("Index", "Shipper");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult CheckAuthentication()
        {
            string email = HttpContext.Session.GetString("email");

            if (!string.IsNullOrEmpty(email))
            {
                // Kiểm tra bảng customer
               
                var shipper = _context.Shippers.FirstOrDefault(c => c.Email == email);
                if (shipper != null)
                {
                    return new JsonResult(new
                    {
                        isAuthenticated = true,
                        isShipper = true,
                        avatar = shipper.Avatar// Giả sử bạn có trường Avatar trong customer
                    });
                }
                // Kiểm tra bảng Recruiter

            }

            // Nếu không tìm thấy email hoặc người dùng chưa đăng nhập
            return new JsonResult(new { isAuthenticated = false });
        }
        private string GetShipperIdFromSession()
        {
            var shipperEmail = HttpContext.Session.GetString("email");
            var shipper = _context.Shippers.FirstOrDefault(c => c.Email == shipperEmail);
            return shipper?.idShipper; // Trả về idAdmin (string) hoặc null
        }
       
        public IActionResult OrderinPending()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId && so.ShippingStatus == ShippingStatus.Pending)  // Lọc theo shipperId
                .Include(so => so.Order)
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Select(so => new ShipperOrderDto
                {
                    idShipperOrder = so.idShipperOrder,
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders); // Trả về view liệt kê các đơn hàng
        }
        public IActionResult OrderinTransit()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId && so.ShippingStatus == ShippingStatus.InProgress)  // Lọc theo shipperId
                .Include(so => so.Order)    
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Select(so => new ShipperOrderDto
                {
                    idShipperOrder = so.idShipperOrder,
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders); // Trả về view liệt kê các đơn hàng
        }
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy danh sách các DetailOrders cho một đơn hàng cụ thể và bao gồm thông tin sản phẩm liên quan
            var detailOrders = _context.DetailOrders
                .Include(d => d.Product)
                .Where(d => d.idOrder == id)
                .ToList();

            // Tính tổng giá cho mỗi DetailOrder
          
            // Truyền orderId vào view để liên kết trở lại đơn hàng
            ViewBag.OrderId = id;

            return View(detailOrders); // Trả về view detail cho DetailOrders
        }
        public IActionResult OrderDelivered()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId && so.ShippingStatus == ShippingStatus.Delivered)  // Lọc theo shipperId
                .Include(so => so.Order)
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Select(so => new ShipperOrderDto
                {
                    idShipperOrder = so.idShipperOrder,
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders); // Trả về view liệt kê các đơn hàng
        }
        public IActionResult OrderFailed()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId && so.ShippingStatus == ShippingStatus.Failed)  // Lọc theo shipperId
                .Include(so => so.Order)
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Select(so => new ShipperOrderDto
                {
                    idShipperOrder = so.idShipperOrder,
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders); // Trả về view liệt kê các đơn hàng
        }
        public IActionResult OrderRefund()
        {
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(shipperId))
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Tìm shipper trong bảng Shippers (kiểm tra nếu shipper tồn tại)
            var shipper = _context.Shippers.FirstOrDefault(s => s.idShipper == shipperId);
            if (shipper == null)
            {
                return RedirectToAction("Login", "Shipper");
            }

            // Lấy các đơn hàng mà shipper đó đang đảm nhiệm và ánh xạ chúng vào ShipperOrderDto
            var orders = _context.ShipperOrders
                .Where(so => so.idShipper == shipperId )  // Lọc theo shipperId
                .Include(so => so.Order)
                .ThenInclude(so => so.Customer)// Lấy thông tin đơn hàng
                .Where(so => so.Order.statusPay == PaymentStatus.Refunded)
                .Select(so => new ShipperOrderDto
                {
                    idShipperOrder = so.idShipperOrder,
                    idOrder = so.Order.idOrder,  // ID đơn hàng
                    nameCustomer = so.Order.Customer.nameCustomer,  // Tên khách hàng
                    OrderDate = so.Order.dateFrom,  // Ngày tạo đơn hàng
                    ShippingStatus = so.ShippingStatus.ToString(),  // Trạng thái vận chuyển          
                    ShipperName = shipper.Name,  // Tên shipper (lấy từ shipper)
                    AssignedDate = so.AssignedDate  // Ngày gán shipper
                })
                .ToList();

            //if (orders == null || !orders.Any())
            //{
            //    return NotFound("No orders found for this shipper.");
            //}

            return View(orders); // Trả về view liệt kê các đơn hàng
        }
        [HttpPost]
        public IActionResult AcceptOrderDelivery(int id)
        {
            // Tìm đơn hàng giao hàng theo idShipperOrder
            var shipperOrder = _context.ShipperOrders.FirstOrDefault(s => s.idShipperOrder == id);

            // Kiểm tra nếu không tìm thấy
            if (shipperOrder == null)
            {
                return NotFound("ShipperOrder not found.");
            }

            // Cập nhật shippingstatus thành 'inprogress'
            shipperOrder.ShippingStatus = ShippingStatus.InProgress;  // Giả sử shipping status là string

            // Hoặc nếu bạn sử dụng Enum thì có thể là
            // shipperOrder.ShippingStatus = ShippingStatus.InProgress;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
            TempData["success"] = "Order received";

            // Trả về kết quả thành công
            return RedirectToAction("OrderinTransit");
        }
        [HttpPost]
        public IActionResult ConfirmDelivery(int id)
        {
            // Tìm đơn hàng giao hàng theo idShipperOrder
            var shipperOrder = _context.ShipperOrders.Include(s => s.Shipper).FirstOrDefault(s => s.idShipperOrder == id);

            // Kiểm tra nếu không tìm thấy
            if (shipperOrder == null)
            {
                return NotFound("ShipperOrder not found.");
            }
            var shipperName = shipperOrder.Shipper.Name; // Nếu không có Shipper, ghi là "Unknown Shipper"
            var shipperId = shipperOrder.idShipper; // Nếu không có Shipper, id mặc định là 0
            var idOrder = shipperOrder.idOrder;
            // Cập nhật shippingstatus thành 'inprogress'
            shipperOrder.ShippingStatus = ShippingStatus.Delivered;  // Giả sử shipping status là string

            // Hoặc nếu bạn sử dụng Enum thì có thể là
            // shipperOrder.ShippingStatus = ShippingStatus.InProgress;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
           

            string recipientEmail = "hieu532004@gmail.com"; // Địa chỉ email cố định
            string subject = "Order Cancellation Notification";
            string body = $"The order with ID {idOrder} has been successfully delivered by the shipper.\n\n" +
                          $"Shipper Details:\n" +
                          $"- Shipper ID: {shipperId}\n" +
                          $"- Shipper Name: {shipperName}";

            string mailtoUrl = $"mailto:{recipientEmail}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            TempData["success"] = "Order confirmed";
            // Chuyển hướng tới Gmail với nội dung soạn sẵn
            return Redirect(mailtoUrl);
        }
        [HttpPost]
        public IActionResult DeleteOrderDelivery(int id)
        {
            // Tìm đơn hàng giao hàng theo idShipperOrder
            var shipperOrder = _context.ShipperOrders.Include(s => s.Shipper).FirstOrDefault(s => s.idShipperOrder == id);

            // Kiểm tra nếu không tìm thấy
            if (shipperOrder == null)
            {
                return NotFound("ShipperOrder not found.");
            }

            // Xóa đơn hàng khỏi database
            var shipperName = shipperOrder.Shipper.Name;
            var shipperId = shipperOrder.idShipper; // Nếu không có Shipper, id mặc định là 0
            var idOrder = shipperOrder.idOrder;

            // Xóa đơn hàng
            _context.ShipperOrders.Remove(shipperOrder);
            _context.SaveChanges();
           
            // Tạo URL Gmail với thông tin chi tiết
            string recipientEmail = "laduy191@gmail.com"; // Địa chỉ email cố định
            string subject = "Order Cancellation Notification";
            string body = $"The order with ID {idOrder} has been canceled by the shipper.\n\n" +
                          $"Shipper Details:\n" +
                          $"- Shipper ID: {shipperId}\n" +
                          $"- Shipper Name: {shipperName}";

            string mailtoUrl = $"mailto:{recipientEmail}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            TempData["success"] = "Order cancel";
            // Chuyển hướng tới Gmail với nội dung soạn sẵn
            return Redirect(mailtoUrl);
        }

    }
}
