using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebThuCung.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WebThuCung.Models;
using WebThuCung.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using NuGet.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text;

namespace WebThuCung.Controllers
{
    public class AdminController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public AdminController(PetContext context)
        {
            _context = context;
        }
        private string GetAdminIdFromSession()
        {
            var adminEmail = HttpContext.Session.GetString("email");
            var admin = _context.Admins.FirstOrDefault(c => c.Email == adminEmail);
            return admin?.idAdmin; // Trả về idAdmin (string) hoặc null
        }
        private string GetShipperIdFromSession()
        {
            var shipperEmail = HttpContext.Session.GetString("email");
            var shipper = _context.Shippers.FirstOrDefault(c => c.Email == shipperEmail);
            return shipper?.idShipper; // Trả về idAdmin (string) hoặc null
        }
        public IActionResult AccessDenied()
        {
            TempData["error"] = "Bạn không có quyền truy cập "; 

            // Có thể trả về một view AccessDenied hoặc chuyển hướng đến một trang mong muốn
            return View();
        }



        public IActionResult Index(string period = "today")
        {
            var adminId = GetAdminIdFromSession();
            var shipperId = GetShipperIdFromSession();

            if (string.IsNullOrEmpty(adminId) && string.IsNullOrEmpty(shipperId))
            {
                // Xử lý nếu không có idAdmin, ví dụ: chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Admin");
            }
            // Lấy dữ liệu bán hàng (số đơn hàng theo ngày, tuần, tháng)
            var salesData = GetSalesData(period);
            var revenueData = GetRevenueData(period);
            var customerData = GetCustomerData(period);
            var topSellingData = GetTopSellingProducts(period);
            var adminData = GetAdminData();
            var model = new DashboardViewDto
            {
                Sales = salesData,
                Revenue = revenueData,
                Customer = customerData,
                TopSellingProducts = topSellingData,
                Admin = adminData
            };


            ViewBag.SelectedPeriod = period; // Để theo dõi khoảng thời gian được chọn

            // Trả về view Index với model đã khởi tạo
            return View(model);
        }
        private AdminDto GetAdminData()
        {
            var adminID = GetAdminIdFromSession(); // Lấy ID của admin từ session

            // Tìm admin dựa trên ID
            var admin = _context.Admins.FirstOrDefault(a => a.idAdmin == adminID);

            if (admin != null)
            {
                return new AdminDto
                {
                    idAdmin = admin.idAdmin,
                    Name = admin.Name,
                    Address = admin.Address,
                    Phone = admin.Phone,
                    userAdmin = admin.userAdmin,
                    Email = admin.Email,
                    // Có thể thêm các thuộc tính khác nếu cần
                };
            }

            return null; // Hoặc có thể trả về một đối tượng AdminDto rỗng nếu không tìm thấy
        }



        public IActionResult UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                // Đường dẫn lưu trữ file ảnh
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/img");

                // Sử dụng tên file gốc
                var fileName = avatar.FileName;

                // Đường dẫn lưu file
                var filePath = Path.Combine(uploads, fileName);

                // Kiểm tra xem file đã tồn tại hay chưa
                if (System.IO.File.Exists(filePath))
                {
                    // Nếu tệp tồn tại, xóa nó
                    System.IO.File.Delete(filePath);
                }

                // Lưu file vào thư mục "wwwroot/admin/img"
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    avatar.CopyTo(fileStream);
                }

                // Trả về tên file gốc cho client để hiển thị
                return Json(new { fileName });
            }

            return BadRequest();
        }
        private int GetCustomerIdFromSession()
        {
            var customerEmail = HttpContext.Session.GetString("email");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
            return customer?.idCustomer ?? 0;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAvatar(AdminDto adminDto)
        {
            if (ModelState.IsValid)
            {
                var  AdminId = GetAdminIdFromSession();
                var existingAdmin = _context.Admins.Find(AdminId);
                if (existingAdmin == null) return NotFound();

                // Upload avatar nếu có file mới, nếu không giữ nguyên
                if (adminDto.Avatar != null && adminDto.Avatar.Length > 0)
                {


                    existingAdmin.Avatar = adminDto.Avatar.FileName; // Cập nhật tên hình đại diện
                }


                _context.Admins.Update(existingAdmin);
                _context.SaveChangesAsync();

                return RedirectToAction("Profile");
            }

            return View("Profile");
        }
        private SalesViewDto GetSalesData(string period)
        {
            // Lấy ngày hôm nay
            var today = DateTime.Today;

            var salesViewModel = new SalesViewDto();

            switch (period.ToLower())
            {
                case "week":
                    salesViewModel = GetWeeklySalesData(today);
                    break;
                case "month":
                    salesViewModel = GetMonthlySalesData(today);
                    break;
                case "today":
                default:
                    salesViewModel = GetDailySalesData(today);
                    break;
            }

            return salesViewModel;
        }
        private RevenueViewDto GetRevenueData(string period)
        {
            // Lấy ngày hôm nay
            var today = DateTime.Today;

            var revenueViewModel = new RevenueViewDto();

            switch (period.ToLower())
            {
                case "week":
                    revenueViewModel = GetWeeklyRevenueData(today);
                    break;
                case "month":
                    revenueViewModel = GetMonthlyRevenueData(today);
                    break;
                case "today":
                default:
                    revenueViewModel = GetDailyRevenueData(today);
                    break;
            }

            return revenueViewModel;
        }
        private CustomerViewDto GetCustomerData(string period)
        {
            // Lấy ngày hôm nay
            var today = DateTime.Today;

            var customerViewModel = new CustomerViewDto();

            switch (period.ToLower())
            {
                case "week":
                    customerViewModel = GetWeeklyCustomerData(today);
                    break;
                case "month":
                    customerViewModel = GetMonthlyCustomerData(today);
                    break;
                case "today":
                default:
                    customerViewModel = GetDailyCustomerData(today);
                    break;
            }

            return customerViewModel;
        }
        private List<TopSellingProductDto> GetTopSellingProducts(string period)
        {
            DateTime today = DateTime.Today;
            DateTime startDate, endDate;

            switch (period)
            {
                case "today":
                    startDate = today;
                    endDate = today;
                    break;
                case "week":
                    startDate = today.AddDays(-(int)today.DayOfWeek); 
                    endDate = startDate.AddDays(6);
                    break;
                case "month":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;
                default:
                    startDate = today;
                    endDate = today;
                    break;
            }

            var topSellingProducts = _context.Orders
     .Where(o => o.dateFrom.Date >= startDate && o.dateFrom.Date <= endDate)
     .SelectMany(o => o.DetailOrders)
     .GroupBy(d => new { d.idProduct, d.Product.nameProduct, d.Product.sellPrice, d.Product.Image })
     .Select(g => new TopSellingProductDto
     {
         ProductId = g.Key.idProduct,
         ProductName = g.Key.nameProduct,
         Price = g.Key.sellPrice,
         DiscountedPrice = g.Key.sellPrice - (g.Key.sellPrice * (_context.Discounts
             .Where(d => d.idProduct == g.Key.idProduct)
             .Select(d => d.discountPercent)
             .FirstOrDefault() / 100m)),
         Sold = g.Sum(d => d.Quantity),
         ImageUrl = g.Key.Image
     })
     .OrderByDescending(p => p.Sold)
     .Take(5)
     .ToList();

            return topSellingProducts;
        }
        private CustomerViewDto GetDailyCustomerData(DateTime today)
        {
            int totalCustomerToday = _context.Customers
                                           .Where(o => o.createdAt.Date == today)
                                           .Count();

            var yesterday = today.AddDays(-1);
            int totalCustomerYesterday = _context.Customers
                                               .Where(o => o.createdAt.Date == yesterday)
                                               .Count();

            double growthPercentageDay = CalculateGrowthPercentage(totalCustomerToday, totalCustomerYesterday);

            return new CustomerViewDto
            {
                TotalCustomerToday = totalCustomerToday,
                GrowthPercentageDay = growthPercentageDay
            };
        }

        private CustomerViewDto GetWeeklyCustomerData(DateTime today)
        {
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Ngày đầu tuần (Chủ Nhật)
            var endOfWeek = startOfWeek.AddDays(6); // Ngày cuối tuần (Thứ Bảy)

            int totalCustomerThisWeek = _context.Customers
                                              .Where(o => o.createdAt.Date >= startOfWeek && o.createdAt.Date <= endOfWeek)
                                              .Count();

            var startOfLastWeek = startOfWeek.AddDays(-7);
            var endOfLastWeek = endOfWeek.AddDays(-7);

            int totalCusstomerLastWeek = _context.Customers
                                              .Where(o => o.createdAt.Date >= startOfLastWeek && o.createdAt.Date <= endOfLastWeek)
                                              .Count();

            double growthPercentageWeek = CalculateGrowthPercentage(totalCustomerThisWeek, totalCusstomerLastWeek);

            return new CustomerViewDto
            {
                TotalCustomerThisWeek = totalCustomerThisWeek,
                GrowthPercentageWeek = growthPercentageWeek
            };
        }

        private CustomerViewDto GetMonthlyCustomerData(DateTime today)
        {
            var startOfMonth = new DateTime(today.Year, today.Month, 1); // Ngày đầu tháng
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng

            int totalCustomerThisMonth = _context.Customers
                                               .Where(o => o.createdAt.Date >= startOfMonth && o.createdAt.Date <= endOfMonth)
                                               .Count();

            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng trước

            int totalCustomerLastMonth = _context.Customers
                                               .Where(o => o.createdAt.Date >= startOfLastMonth && o.createdAt.Date <= endOfLastMonth)
                                               .Count();

            double growthPercentageMonth = CalculateGrowthPercentage(totalCustomerThisMonth, totalCustomerLastMonth);

            return new CustomerViewDto
            {
                TotalCustomerThisMonth = totalCustomerThisMonth,
                GrowthPercentageMonth = growthPercentageMonth
            };
        }

        private SalesViewDto GetDailySalesData(DateTime today)
        {
            int totalOrdersToday = _context.Orders
                                           .Where(o => o.dateFrom.Date == today)
                                           .Count();

            var yesterday = today.AddDays(-1);
            int totalOrdersYesterday = _context.Orders
                                               .Where(o => o.dateFrom.Date == yesterday)
                                               .Count();

            double growthPercentageDay = CalculateGrowthPercentage(totalOrdersToday, totalOrdersYesterday);

            return new SalesViewDto
            {
                TotalOrdersToday = totalOrdersToday,
                GrowthPercentageDay = growthPercentageDay
            };
        }

        private SalesViewDto GetWeeklySalesData(DateTime today)
        {
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Ngày đầu tuần (Chủ Nhật)
            var endOfWeek = startOfWeek.AddDays(6); // Ngày cuối tuần (Thứ Bảy)

            int totalOrdersThisWeek = _context.Orders
                                              .Where(o => o.dateFrom.Date >= startOfWeek && o.dateFrom.Date <= endOfWeek)
                                              .Count();

            var startOfLastWeek = startOfWeek.AddDays(-7);
            var endOfLastWeek = endOfWeek.AddDays(-7);

            int totalOrdersLastWeek = _context.Orders
                                              .Where(o => o.dateFrom.Date >= startOfLastWeek && o.dateFrom.Date <= endOfLastWeek)
                                              .Count();

            double growthPercentageWeek = CalculateGrowthPercentage(totalOrdersThisWeek, totalOrdersLastWeek);

            return new SalesViewDto
            {
                TotalOrdersThisWeek = totalOrdersThisWeek,
                GrowthPercentageWeek = growthPercentageWeek
            };
        }

        private SalesViewDto GetMonthlySalesData(DateTime today)
        {
            var startOfMonth = new DateTime(today.Year, today.Month, 1); // Ngày đầu tháng
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng

            int totalOrdersThisMonth = _context.Orders
                                               .Where(o => o.dateFrom.Date >= startOfMonth && o.dateFrom.Date <= endOfMonth)
                                               .Count();

            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng trước

            int totalOrdersLastMonth = _context.Orders
                                               .Where(o => o.dateFrom.Date >= startOfLastMonth && o.dateFrom.Date <= endOfLastMonth)
                                               .Count();

            double growthPercentageMonth = CalculateGrowthPercentage(totalOrdersThisMonth, totalOrdersLastMonth);

            return new SalesViewDto
            {
                TotalOrdersThisMonth = totalOrdersThisMonth,
                GrowthPercentageMonth = growthPercentageMonth
            };
        }

       

        private RevenueViewDto GetDailyRevenueData(DateTime today)
        {
            var ordersToday = _context.Orders
     .Where(o => o.dateFrom.Date == today)
     .Include(o => o.DetailOrders)
     .ThenInclude(p => p.Product)// Bao gồm cả DetailOrders
     .ToList();  // Lấy toàn bộ dữ liệu ra phía client

            decimal totalRevenueToday = ordersToday
                .Where(o => o.DetailOrders != null && o.DetailOrders.Any())
                .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity));

            var yesterday = today.AddDays(-1);
            var ordersYesterday = _context.Orders
     .Where(o => o.dateFrom.Date == yesterday)
     .Include(o => o.DetailOrders)
      .ThenInclude(p => p.Product)// Bao gồm DetailOrders trong kết quả
     .ToList();  // Lấy toàn bộ đơn hàng của ngày hôm qua về phía client

            decimal totalRevenueYesterday = ordersYesterday
                .Where(o => o.DetailOrders != null && o.DetailOrders.Any())  // Lọc các đơn hàng có DetailOrders
                 .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity)); // Tính tổng doanh thu của các DetailOrders

            double growthPercentageDay = CalculateGrowthPercentage1(totalRevenueToday, totalRevenueYesterday);

            return new RevenueViewDto
            {
                TotalRevenueToday = totalRevenueToday,           
                GrowthPercentageDay = growthPercentageDay
            };
        }

        private RevenueViewDto GetWeeklyRevenueData(DateTime today)
        {
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Ngày đầu tuần (Chủ Nhật)
            var endOfWeek = startOfWeek.AddDays(6); // Ngày cuối tuần (Thứ Bảy)

            var ordersThisWeek = _context.Orders
    .Where(o => o.dateFrom.Date >= startOfWeek && o.dateFrom.Date <= endOfWeek)
    .Include(o => o.DetailOrders)
     .ThenInclude(p => p.Product)// Bao gồm DetailOrders trong kết quả
    .ToList();  // Lấy toàn bộ đơn hàng của tuần về phía client

            decimal totalRevenueThisWeek = ordersThisWeek
                .Where(o => o.DetailOrders != null && o.DetailOrders.Any())  // Lọc các đơn hàng có DetailOrders
               .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity));  // Tính tổng doanh thu của các DetailOrders


            var startOfLastWeek = startOfWeek.AddDays(-7);
            var endOfLastWeek = endOfWeek.AddDays(-7);

            var ordersLastWeek = _context.Orders
    .Where(o => o.dateFrom.Date >= startOfLastWeek && o.dateFrom.Date <= endOfLastWeek)
    .Include(o => o.DetailOrders)
     .ThenInclude(p => p.Product)// Bao gồm DetailOrders trong kết quả
    .ToList();  // Lấy toàn bộ đơn hàng của tuần trước về phía client

            decimal totalRevenueLastWeek = ordersLastWeek
                .Where(o => o.DetailOrders != null && o.DetailOrders.Any())  // Lọc các đơn hàng có DetailOrders
                .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity)); // Tính tổng doanh thu của các DetailOrders


            double growthPercentageWeek = CalculateGrowthPercentage1(totalRevenueThisWeek, totalRevenueLastWeek);

            return new RevenueViewDto
            {
                TotalRevenueThisWeek = totalRevenueThisWeek,
                GrowthPercentageWeek = growthPercentageWeek
            };
        }

        private RevenueViewDto GetMonthlyRevenueData(DateTime today)
        {
            var startOfMonth = new DateTime(today.Year, today.Month, 1); // Ngày đầu tháng
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng

            var ordersThisMonth = _context.Orders
    .Where(o => o.dateFrom.Month == today.Month && o.dateFrom.Year == today.Year)
    .Include(o => o.DetailOrders)
     .ThenInclude(p => p.Product)// Bao gồm DetailOrders trong kết quả
    .ToList();  // Lấy toàn bộ đơn hàng của tháng hiện tại về phía client

            decimal totalRevenueThisMonth = ordersThisMonth
                .Where(o => o.DetailOrders != null && o.DetailOrders.Any())  // Lọc các đơn hàng có DetailOrders
               .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity));  // Tính tổng doanh thu của các DetailOrders


            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1); // Ngày cuối tháng trước

           var ordersLastMonth = _context.Orders
    .Where(o => o.dateFrom >= startOfLastMonth && o.dateFrom <= endOfLastMonth)
    .Include(o => o.DetailOrders)
     .ThenInclude(p => p.Product)// Bao gồm DetailOrders trong kết quả
    .ToList();  // Lấy toàn bộ đơn hàng của tháng trước về phía client

decimal totalRevenueLastMonth = ordersLastMonth
    .Where(o => o.DetailOrders != null && o.DetailOrders.Any())  // Lọc các đơn hàng có DetailOrders
    .Sum(o => o.DetailOrders.Sum(d => d.Product.sellPrice * d.Quantity));  // Tính tổng doanh thu của các DetailOrders

            double growthPercentageMonth = CalculateGrowthPercentage1(totalRevenueThisMonth, totalRevenueLastMonth);

            return new RevenueViewDto
            {
                TotalRevenueThisMonth = totalRevenueThisMonth,
                GrowthPercentageMonth = growthPercentageMonth,
            };
        }


        private double CalculateGrowthPercentage(int current, int previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0; // Tránh chia cho 0
            return ((double)(current - previous) / previous) * 100;
        }
        private double CalculateGrowthPercentage1(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0; // Tránh chia cho 0
                                                             // Chuyển đổi 'previous' thành 'double' trước khi chia
            return ((double)(current - previous) / (double)previous) * 100;
        }

        // GET: Hiển thị trang đăng nhập
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
                // Tìm kiếm admin trong cơ sở dữ liệu
                var ad = await _context.Admins.Include(a => a.Role)
                                               .SingleOrDefaultAsync(n => n.userAdmin == model.userName && n.passwordAdmin == model.password);

                if (ad != null)
                {
                    TempData["success"] = "Đăng nhập thành công";

                    // Tạo Claims cho người dùng
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad.userAdmin),
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
                    var adminJson = Newtonsoft.Json.JsonConvert.SerializeObject(ad, settings);

                    HttpContext.Session.SetString("TaikhoanAdmin", adminJson);
                    HttpContext.Session.SetString("email", ad.Email);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            return View(model);
        }
       
        [Authorize]
        public IActionResult Profilerole()
        {
            var username = User.Identity.Name; // Lấy tên người dùng
            var email = User.FindFirstValue(ClaimTypes.Email); // Lấy email
            var role = User.FindFirstValue(ClaimTypes.Role); // Lấy vai trò

            ViewBag.Username = username;
            ViewBag.Email = email;
            ViewBag.Role = role;

            return View();
        }

        public IActionResult ListAdmin()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var admins = _context.Admins.ToList();

            // Truyền danh sách admin sang view
            return View(admins);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("TaikhoanAdmin");

            ViewBag.Thongbao = "Bạn đã đăng xuất thành công.";

            return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public IActionResult CheckAuthentication()
        {
            string email = HttpContext.Session.GetString("email");

            if (!string.IsNullOrEmpty(email))
            {
                // Kiểm tra bảng customer
                var admin = _context.Admins.FirstOrDefault(c => c.Email == email);
                if (admin != null)
                {
                    return new JsonResult(new
                    {
                        isAuthenticated = true,
                        isadmin = true,
                        avatar = admin.Avatar// Giả sử bạn có trường Avatar trong customer
                    });
                }
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
        public IActionResult LoadProfilePartial()
        {
            var idAdmin = GetAdminIdFromSession();
            if (string.IsNullOrEmpty(idAdmin))
            {
                // Xử lý nếu không có idAdmin, ví dụ: chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Admin");
            }

            // Lấy thông tin khách hàng từ cơ sở dữ liệu, bao gồm các thông tin liên quan
            var admin = _context.Admins
           .FirstOrDefault(c => c.idAdmin == idAdmin);

            if (admin == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang khác
            }

            return PartialView("ProfileOverview", admin);
        }
        public IActionResult Profile()
        {
          
            var idAdmin = GetAdminIdFromSession();
            if (string.IsNullOrEmpty(idAdmin))
            {
                // Xử lý nếu không có idAdmin, ví dụ: chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Admin");
            }

            // Lấy thông tin khách hàng từ cơ sở dữ liệu, bao gồm các thông tin liên quan
            var admin = _context.Admins
              .FirstOrDefault(c => c.idAdmin == idAdmin);

            if (admin == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang khác
            }

            return View(admin); // Trả về View Profiler với model là thông tin người dùng
        }
        [HttpGet]
        public IActionResult EditProfilePartial()
        {
            var idAdmin = GetAdminIdFromSession();
            if (string.IsNullOrEmpty(idAdmin))
            {
                return RedirectToAction("Login", "Admin");
            }

            var admin = _context.Admins.FirstOrDefault(c => c.idAdmin == idAdmin);
            if (admin == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home");
            }
            var adminDto = new AdminDto
            {
                Name = admin.Name,  
                Address = admin.Address,
                Phone = admin.Phone,
                Email = admin.Email,
               
            };
            return PartialView("ProfileEdit", adminDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(AdminDto model)
        {
            var idAdmin = GetAdminIdFromSession();
            if (string.IsNullOrEmpty(idAdmin))
            {
                return RedirectToAction("Login", "Admin");
            }

            var admin = _context.Admins.Find(idAdmin);
            if (admin == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home");
            }


            // Cập nhật thông tin từ model, giữ nguyên giá trị cũ nếu giá trị mới là null
            admin.Name = model.Name ?? admin.Name;
            admin.Address = model.Address ?? admin.Address;
            admin.Phone = model.Phone ?? admin.Phone;
            admin.Email = model.Email ?? admin.Email;

            // Nếu cần cập nhật thêm các thông tin khác
            // admin.OtherProperty = model.OtherProperty ?? admin.OtherProperty;

            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            TempData["success"] = "Cập nhật thông tin thành công.";

            var updatedAdminDto = new AdminDto
            {          
                Name = admin.Name,
                Address = admin.Address,
                Phone = admin.Phone,
                Email = admin.Email
            };

            // Trả về `PartialView` với `AdminDto` sau khi cập nhật
            return PartialView("ProfileEdit", updatedAdminDto);
        }

       

       

    }
}
