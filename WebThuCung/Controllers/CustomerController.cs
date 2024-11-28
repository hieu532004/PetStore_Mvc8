using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class CustomerController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public CustomerController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var customers = _context.Customers.ToList();

            // Truyền danh sách admin sang view
            return View(customers);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id) // Thay đổi kiểu từ string sang int
        {
            if (id <= 0) // Kiểm tra id không hợp lệ
            {
                return NotFound();
            }

            var customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Size sau khi xóa
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        //#region Đăng ký tài khoản người dùng
        [HttpPost]
        public IActionResult Create(RegisterDto model)
        {
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

                    var khachHang = new Customer
                    {
                        userCustomer = model.userCustomer,
                        nameCustomer = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        Image = "avatar_user.png", 
                    };

                    // Sử dụng BCrypt để băm mật khẩu
                    khachHang.passwordCustomer = HashPassword(model.paswordCusstomer);

                    // Thêm khách hàng mới vào cơ sở dữ liệu
                    _context.Customers.Add(khachHang);
                    _context.SaveChanges();

                    return RedirectToAction("ConfirmOtp", new { email = model.Email });
                }
            }

            // Trả về view với dữ liệu của model khi có lỗi
            return View(model);
        }
        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        [HttpPost]
        public IActionResult Search(int? IdCustomer, string NameCustomer)
        {
            // Tạo truy vấn cơ sở dữ liệu
            var query = _context.Customers.AsQueryable();

            // Nếu IdCustomer có giá trị, thêm điều kiện vào truy vấn
            if (IdCustomer.HasValue)
            {
                query = query.Where(c => c.idCustomer == IdCustomer.Value);
            }

            // Nếu NameCustomer không rỗng, thêm điều kiện tìm kiếm tên
            if (!string.IsNullOrEmpty(NameCustomer))
            {
                query = query.Where(c => c.nameCustomer.Contains(NameCustomer));
            }

            // Thực hiện truy vấn và lấy danh sách khách hàng
            var customers = query.ToList();

            // Trả về view với danh sách khách hàng tìm được
            return View("Index", customers);
        }



    }
}
