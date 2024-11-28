using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebThuCung.Controllers
{
    public class UserController : BaseController
    {
        private readonly PetContext _context;
        public UserController(PetContext context) : base(context)
        {
            _context = context;
        }
        public IActionResult Index(string idCategory)
        {
            // Mặc định nếu không truyền vào ngày bắt đầu và ngày kết thúc, lấy dữ liệu 30 ngày gần nhất
      
            DateTime startDate = DateTime.Today.AddDays(-30);

            DateTime endDate = DateTime.Today;

            var customerId = GetCustomerIdFromSession();

            // Truy vấn sản phẩm
            var productsQuery = _context.Products
                .Include(sp => sp.Branch)  // Bao gồm thông tin thương hiệu
                .Include(sp => sp.Category) // Bao gồm thông tin loại
                .AsQueryable(); // Chuyển đổi thành IQueryable để dễ dàng thêm điều kiện

            // Nếu idCategory có giá trị, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(idCategory))
            {
                productsQuery = productsQuery.Where(sp => sp.idCategory == idCategory);
            }

            // Chọn các thuộc tính cần thiết
            var allProducts = productsQuery
                .Select(sp => new ProductViewDto
                {
                    idProduct = sp.idProduct,
                    nameProduct = sp.nameProduct,
                    sellPrice = sp.sellPrice,
                    Image = sp.Image,
                    idBranch = sp.idBranch,
                    idCategori = sp.idCategory,
                    nameBranch = sp.Branch.nameBranch,
                    nameCategory = sp.Category.nameCategory,
                    Quantity = sp.Quantity,
                    Description = sp.Description,
                    Logo = sp.Branch.Logo,
                    DiscountedPrice = sp.sellPrice - (sp.sellPrice * (_context.Discounts
                .Where(d => d.idProduct == sp.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault() / 100m)),
                    DiscountPercent = _context.Discounts
                .Where(d => d.idProduct == sp.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault()
                })
                .OrderBy(p => p.idProduct)
                .ToList();

            // Lấy danh sách ID sản phẩm đã lưu
            var savedProductIds = _context.SaveProducts
                .Where(s => s.idCustomer == customerId)
                .Select(s => s.idProduct)
                .ToList();

            // Gán danh sách sản phẩm đã lưu vào ViewBag
            ViewBag.SavedProductIds = savedProductIds;

            // Truy vấn sản phẩm bán chạy nhất
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
                    Revenue = g.Sum(d => d.Product.sellPrice * d.Quantity),
                    ImageUrl = g.Key.Image,
                    DiscountPercent = _context.Discounts
                .Where(d => d.idProduct == g.Key.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault()
                })
                .OrderByDescending(p => p.Sold)
                .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                .ToList();

            // Gán danh sách sản phẩm bán chạy nhất vào ViewBag để sử dụng trong View
            ViewBag.TopSellingProducts = topSellingProducts;

            return View(allProducts);
        }


        private int GetCustomerIdFromSession()
        {
            var customerEmail = HttpContext.Session.GetString("email");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
            return customer?.idCustomer ?? 0;
        }
        [HttpGet]
        public IActionResult CheckAuthentication()
        {
            string email = HttpContext.Session.GetString("email");

            if (!string.IsNullOrEmpty(email))
            {
                // Kiểm tra bảng customer
                var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    return new JsonResult(new
                    {
                        isAuthenticated = true,
                        iscustomer = true,
                        avatar =  customer.Image// Giả sử bạn có trường Avatar trong customer
                    });
                }

                // Kiểm tra bảng Recruiter
             
            }

            // Nếu không tìm thấy email hoặc người dùng chưa đăng nhập
            return new JsonResult(new { isAuthenticated = false });
        }
        public bool Checkusername(string username)
        {
            return _context.Customers.Any(x => x.userCustomer == username);
        }

        public bool CheckEmail(string email)
        {
            return _context.Customers.Any(x => x.Email == email);
        }
        public bool CheckEmailAdmin(string email)
        {
            return _context.Admins.Any(x => x.Email == email);
        }

        // Hàm sử dụng BCrypt để băm mật khẩu
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            // Đảm bảo chúng ta bao gồm ID quốc gia với các thành phố
            ViewBag.Cities = _context.Cities.Select(c => new
            {
                Value = c.idCity.ToString(),
                Text = c.nameCity,
                CountryId = c.idCountry // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Districts = _context.Districts.Select(d => new
            {
                Value = d.idDistrict.ToString(),
                Text = d.nameDistrict,
                CityId = d.idCity // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Wards = _context.Wards.Select(w => new
            {
                Value = w.idWard.ToString(),
                Text = w.nameWard,
                DistrictId = w.idDistrict // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            return View();
        }


        //#region Đăng ký tài khoản người dùng
        [HttpPost]
        public IActionResult Register(RegisterDto model)
        {
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            // Đảm bảo chúng ta bao gồm ID quốc gia với các thành phố
            ViewBag.Cities = _context.Cities.Select(c => new
            {
                Value = c.idCity.ToString(),
                Text = c.nameCity,
                CountryId = c.idCountry // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Districts = _context.Districts.Select(d => new
            {
                Value = d.idDistrict.ToString(),
                Text = d.nameDistrict,
                CityId = d.idCity // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Wards = _context.Wards.Select(w => new
            {
                Value = w.idWard.ToString(),
                Text = w.nameWard,
                DistrictId = w.idDistrict // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            // Kiểm tra xem model có hợp lệ không
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                if (Checkusername(model.userCustomer))
                {
                    ModelState.AddModelError("userCustomer", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                // Kiểm tra xem email đã tồn tại chưa
                if (CheckEmail(model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }
                if (CheckEmailAdmin(model.Email))
                {
                    ModelState.AddModelError("Email", "Email này là của Admin");
                    return View(model);
                }

                // Nếu không có lỗi xác thực nào, tiến hành tạo khách hàng
                if (ModelState.IsValid)
                {
                    var otp = new Random().Next(100000, 999999).ToString();
                
                    var khachHang = new Customer
                    {
                        userCustomer = model.userCustomer,
                        nameCustomer = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                        OtpCode = otp, // Lưu OTP trong cơ sở dữ liệu
                        OtpExpiryTime = DateTime.Now.AddMinutes(5),
                        idCountry = model.idCountry, // Giá trị từ model
                        idCity = model.idCity,
                        idDistrict = model.idDistrict,
                        idWard = model.idWard,
                        Image = "avatar_user.png", // Có thể thay đổi nếu bạn xử lý hình ảnh upload
                    };

                    // Sử dụng BCrypt để băm mật khẩu
                    khachHang.passwordCustomer = HashPassword(model.paswordCusstomer);

                    // Thêm khách hàng mới vào cơ sở dữ liệu
                    _context.Customers.Add(khachHang);
                    _context.SaveChanges();
                    SendOtpEmail(model.Email, otp);
                  
                    return RedirectToAction("ConfirmOtp", new { email = model.Email });
                }
            }

            // Trả về view với dữ liệu của model khi có lỗi
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                // Tìm kiếm khách hàng theo tên đăng nhập hoặc email
                var customer = _context.Customers
                                       .SingleOrDefault(c => c.userCustomer == model.userName || c.Email == model.userName);

                // Nếu tài khoản không tồn tại
                if (customer == null)
                {
                    // Thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại. Vui lòng kiểm tra lại email hoặc tên đăng nhập.");
                    return View(model); // Trả về view cùng thông báo lỗi
                }

                // Kiểm tra xem email đã được xác nhận chưa
                if (!customer.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email của bạn chưa được xác nhận.");
                    return View(model); // Trả về view cùng thông báo lỗi
                }

                // Kiểm tra mật khẩu
                if (!BCrypt.Net.BCrypt.Verify(model.password, customer.passwordCustomer))
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu không đúng. Vui lòng thử lại.");
                    return View(model); // Trả về view cùng thông báo lỗi
                }

                // Nếu vượt qua tất cả các kiểm tra, đăng nhập thành công
                ViewBag.Thongbao = "Đăng nhập thành công";
                TempData["success"] = "Đăng nhập thành công";
                // Serialize thông tin khách hàng thành JSON và lưu vào Session
                var customerJson = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                HttpContext.Session.SetString("Taikhoan", customerJson);
                HttpContext.Session.SetString("email", customer.Email);
                HttpContext.Session.SetString("phone", customer.Phone);

                // Lấy giá trị từ Session để gán vào ViewBag
                var Phone = HttpContext.Session.GetString("phone");
                var Email = HttpContext.Session.GetString("email");
                ViewBag.Phone = Phone;
                ViewBag.Email = Email;
                // Chuyển hướng về trang chính sau khi đăng nhập thành công
                return RedirectToAction("Index", "User");
            }

            // Nếu model không hợp lệ, trả về view với thông báo lỗi ModelState
            return View(model);
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Taikhoan");

            ViewBag.Thongbao = "Bạn đã đăng xuất thành công.";

            return RedirectToAction("Login", "User");
        }

        public IActionResult ConfirmOtp(string email)
        {
            var model = new ConfirmOtpDto { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmOtp(ConfirmOtpDto model)
        {

            var customer = _context.Customers.SingleOrDefault(c => c.Email == model.Email && c.OtpCode == model.Otp);

            if (customer != null && customer.OtpExpiryTime > DateTime.Now)
            {
                // Xử lý cho customer
                customer.EmailConfirmed = true;
                customer.OtpCode = null; // Xóa OTP sau khi xác nhận thành công
                customer.OtpExpiryTime = null;
                _context.SaveChanges();

                return RedirectToAction("Login");
            }      
            else
            {
                ModelState.AddModelError("Otp", "Mã OTP không đúng hoặc đã hết hạn.");
            }

            // Nếu có lỗi, trả về view với thông báo lỗi và các giá trị nhập
            return View(model); // Gửi lại model về view
        }
        private void SendOtpEmail(string email, string otp)
        {
            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587; // Port cho TLS
                client.EnableSsl = true; // Bật SSL
                client.Credentials = new NetworkCredential(
                    Environment.GetEnvironmentVariable("EMAIL_USERNAME"),
                    Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
                );

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Environment.GetEnvironmentVariable("EMAIL_USERNAME")),
                    Subject = "Mã OTP Xác Nhận Địa Chỉ Email",
                    Body = $"<h1>Xác Nhận Địa Chỉ Email</h1>" +
                           $"<p>Đây là mã OTP của bạn: <strong>{otp}</strong></p>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                try
                {
                    client.Send(mailMessage);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }
        public IActionResult ConfirmOtpReset(string email)
        {
            var model = new ConfirmOtpDto { Email = email };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmOtpReset(ConfirmOtpDto model)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Email == model.Email && c.OtpCode == model.Otp);
           

            if (customer != null && customer.OtpExpiryTime > DateTime.Now)
            {
                // Xử lý cho customer
                customer.OtpCode = null; // Xóa OTP sau khi xác nhận thành công
                customer.OtpExpiryTime = null; // Xóa thông tin OTP
                _context.SaveChanges();

                // Chuyển hướng đến trang nhập lại mật khẩu
                return RedirectToAction("ResetPassword", new { email = model.Email });
            }        
            else
            {
                ModelState.AddModelError("Otp", "Mã OTP không đúng hoặc đã hết hạn.");
            }

            // Nếu có lỗi, trả về view với thông báo lỗi và các giá trị nhập
            return View(model); // Gửi lại model về view
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {

                // Kiểm tra trong bảng customer
                var customer = _context.Customers.SingleOrDefault(c => c.Email == model.Email);
                if (customer != null)
                {
                    // Tạo mã OTP ngẫu nhiên
                    var otp = new Random().Next(100000, 999999).ToString();
                    customer.OtpCode = otp; // Gán mã OTP
                    customer.OtpExpiryTime = DateTime.Now.AddMinutes(10); // Đặt thời gian hết hạn
                    _context.SaveChanges();

                    // Gửi email chứa mã OTP
                    SendOtpEmail(model.Email, otp);
                    return RedirectToAction("ConfirmOtpReset", new { email = model.Email });
                }            

                // Nếu không tìm thấy người dùng, thêm thông báo lỗi
                ModelState.AddModelError(string.Empty, "Email không tồn tại.");
            }

            // Trả lại view với thông báo lỗi
            return View(model);
        }


        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordDto { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng theo email
                var customer = _context.Customers.SingleOrDefault(c => c.Email == model.Email);

                if (customer != null)
                {
                    // Cập nhật mật khẩu cho customer
                    customer.passwordCustomer = HashPassword(model.Password); // Băm mật khẩu
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    // Nếu không tìm thấy người dùng, có thể gửi thông báo thành công giả để bảo mật
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }
            return View(model);
        }

        public IActionResult Profile()
        {
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            // Đảm bảo chúng ta bao gồm ID quốc gia với các thành phố
            ViewBag.Cities = _context.Cities.Select(c => new
            {
                Value = c.idCity.ToString(),
                Text = c.nameCity,
                CountryId = c.idCountry // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Districts = _context.Districts.Select(d => new
            {
                Value = d.idDistrict.ToString(),
                Text = d.nameDistrict,
                CityId = d.idCity // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Wards = _context.Wards.Select(w => new
            {
                Value = w.idWard.ToString(),
                Text = w.nameWard,
                DistrictId = w.idDistrict // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                return RedirectToAction("Login", "User"); // Redirect về trang chủ hoặc trang khác
            }// Lấy ID khách hàng từ session hoặc JWT

            // Lấy thông tin khách hàng từ cơ sở dữ liệu, bao gồm các thông tin liên quan
            var customer =  _context.Customers
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.Ward)
                .FirstOrDefault(c => c.idCustomer == idCustomer);

            if (customer == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang khác
            }

            return View(customer); // Trả về View Profiler với model là thông tin người dùng
        }
        public IActionResult LoadProfilePartial()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                return RedirectToAction("Login", "User"); // Redirect về trang chủ hoặc trang khác
            }// Lấy ID khách hàng từ session hoặc JWT

            // Lấy thông tin khách hàng từ cơ sở dữ liệu, bao gồm các thông tin liên quan
            var customer = _context.Customers
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.Ward)
                .FirstOrDefault(c => c.idCustomer == idCustomer);

            if (customer == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang khác
            }

            return PartialView("_Profile", customer);
        }

        public IActionResult LoadAddressPartial()
        {
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            // Đảm bảo chúng ta bao gồm ID quốc gia với các thành phố
            ViewBag.Cities = _context.Cities.Select(c => new
            {
                Value = c.idCity.ToString(),
                Text = c.nameCity,
                CountryId = c.idCountry // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Districts = _context.Districts.Select(d => new
            {
                Value = d.idDistrict.ToString(),
                Text = d.nameDistrict,
                CityId = d.idCity // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Wards = _context.Wards.Select(w => new
            {
                Value = w.idWard.ToString(),
                Text = w.nameWard,
                DistrictId = w.idDistrict // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                return RedirectToAction("Login", "User"); // Redirect về trang chủ hoặc trang khác
            }// Lấy ID khách hàng từ session hoặc JWT

            // Lấy thông tin khách hàng từ cơ sở dữ liệu, bao gồm các thông tin liên quan
            var customer = _context.Customers
                .Include(c => c.Country)
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.Ward)
                .FirstOrDefault(c => c.idCustomer == idCustomer);

            if (customer == null)
            {
                TempData["error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang khác
            }
            return PartialView("_Address", customer);
        }
        public IActionResult LoadOrderPartial()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer)
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                TempData["success"] = "Bạn chưa có đơn hàng nào";
                return View("_Order", transactions);
            }

            return View("_Order", transactions);
        }
        [HttpGet]
        public IActionResult ChangePasswordPartial()
        {
            var email = HttpContext.Session.GetString("email");

            // Nếu không có email trong session, chuyển hướng đến trang đăng nhập hoặc thông báo lỗi
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth"); // Thay đổi tên controller nếu cần
            }

            // Tạo một đối tượng ChangePasswordModel với email từ session
            var model = new ChangePasswordDto
            {
                Email = email // Chuyển email vào model
            };

            // Trả về view ChangePassword
            return View("_ChangePassword", model);
           
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordDto model)
        {
          
            var email = HttpContext.Session.GetString("email");

            // Kiểm tra xem email có hợp lệ không
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Email không hợp lệ.");
                return PartialView("_ChangePassword", model);
            }

            // Kiểm tra trong bảng customer
            var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
            if (customer != null)
            {
                // Kiểm tra mật khẩu cũ
                if (BCrypt.Net.BCrypt.Verify(model.CurrentPassword, customer.passwordCustomer))
                {
                    // Mã hóa mật khẩu mới và cập nhật vào cơ sở dữ liệu
                    customer.passwordCustomer = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    _context.SaveChanges();

                    // Thông báo thành công
                    TempData["success"] = "Mật khẩu của bạn đã được thay đổi thành công.";
                    return View("_ChangePassword", model);
                }
                else
                {
                    TempData["error"] = "Mật khẩu cũ không chính xác.";
                    return View("_ChangePassword", model);
                }
            }
            return View("_ChangePassword", model);
        }
        public IActionResult UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                // Đường dẫn lưu trữ file ảnh
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/customer");

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CustomerDto customerDto)
        {
            if (ModelState.IsValid)
            {
                int customerId = GetCustomerIdFromSession();
                var existingCustomer =  _context.Customers.Find(customerId);
                if (existingCustomer == null) return NotFound();

                // Upload avatar nếu có file mới, nếu không giữ nguyên
                if (customerDto.Image != null && customerDto.Image.Length > 0)
                {
                  

                    existingCustomer.Image = customerDto.Image.FileName; // Cập nhật tên hình đại diện
                }


                _context.Customers.Update(existingCustomer);
                _context.SaveChangesAsync();

                return RedirectToAction("Profile");
            }

            return View("Profile");
        }
        [HttpPost]
        public IActionResult UpdateProfile(CustomerDto customerDto)
        {
           
            if (ModelState.IsValid)
            {
                var idCustomer = GetCustomerIdFromSession();
                var customer = _context.Customers.Find(idCustomer);

                if (customer != null)
                {
                    // Cập nhật thông tin khách hàng, giữ nguyên giá trị cũ nếu giá trị mới là null
                    customer.nameCustomer = customerDto.nameCustomer ?? customer.nameCustomer;
                    customer.Phone = customerDto.Phone ?? customer.Phone;
                    customer.Address = customerDto.Address ?? customer.Address;
                    customer.dateBirth = customerDto.dateBirth ?? customer.dateBirth;

                    // Cập nhật thông tin địa chỉ

                    TempData["success"] = "Cập nhật profile thành công.";
                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                    return PartialView("_Profile", customer);// Chuyển hướng sau khi cập nhật thành công
                }
            }

            return PartialView("_Profile");
        }
        [HttpPost]
        public IActionResult UpdateAddress(CustomerDto customerDto)
        {
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            // Đảm bảo chúng ta bao gồm ID quốc gia với các thành phố
            ViewBag.Cities = _context.Cities.Select(c => new
            {
                Value = c.idCity.ToString(),
                Text = c.nameCity,
                CountryId = c.idCountry // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Districts = _context.Districts.Select(d => new
            {
                Value = d.idDistrict.ToString(),
                Text = d.nameDistrict,
                CityId = d.idCity // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();

            ViewBag.Wards = _context.Wards.Select(w => new
            {
                Value = w.idWard.ToString(),
                Text = w.nameWard,
                DistrictId = w.idDistrict // Đảm bảo điều này khớp với quan hệ khóa ngoại đúng
            }).ToList();
            if (ModelState.IsValid)
            {
                var idCustomer = GetCustomerIdFromSession();
                var customer = _context.Customers.Find(idCustomer);

                if (customer != null)
                {
                    // Cập nhật thông tin khách hàng, giữ nguyên giá trị cũ nếu giá trị mới là null


                    // Cập nhật thông tin địa chỉ
                    customer.idCountry = customerDto.idCountry ?? customer.idCountry;
                    customer.idCity = customerDto.idCity ?? customer.idCity;
                    customer.idDistrict = customerDto.idDistrict ?? customer.idDistrict;
                    customer.idWard = customerDto.idWard ?? customer.idWard;
                    customer.Address = customerDto.Address ?? customer.Address;

                    TempData["success"] = "Cập nhật Address thành công.";

                    _context.SaveChanges();
                    return PartialView("_Address", customer);// Chuyển hướng sau khi cập nhật thành công
                }
            }

            return PartialView("_Address");
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RefunOrder(string idOrder)
        {
            if (string.IsNullOrEmpty(idOrder))
            {
                return Json(new { success = false, message = "Order ID is missing." });
            }

            // Tìm đơn hàng trong database
            var order = _context.Orders.FirstOrDefault(o => o.idOrder == idOrder);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

     // Kiểm tra trạng thái đơn hàng

            if (order.statusPay != PaymentStatus.Paid)
            {
                return Json(new { success = false, message = "Your unpaid order is currently non-refundable." });
            }

            // Cập nhật trạng thái đơn hàng
            order.statusPay = PaymentStatus.Refunded;
            var shipperOrder = _context.ShipperOrders.FirstOrDefault(s => s.idOrder == idOrder);

            if (shipperOrder != null)
            {
                // Đổi trạng thái giao hàng thành 'Pending'
                shipperOrder.ShippingStatus = ShippingStatus.Pending;
            }
            _context.SaveChanges();
            TempData["success"] = "Order refuned";
            return Json(new { success = true });
        }
    }
}
