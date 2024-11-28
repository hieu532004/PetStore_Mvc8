using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class ColorController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public ColorController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var colors = _context.Colors.ToList();

            // Truyền danh sách admin sang view
            return View(colors);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ColorDto colorDto)
        {
            if (ModelState.IsValid)
            {
                var existingColor = _context.Colors.FirstOrDefault(s => s.idColor == colorDto.idColor);
                if (existingColor != null)
                {
                    ModelState.AddModelError("idColor", $"Color with ID '{colorDto.idColor}' already exists.");
                    return View(colorDto); // Trả lại form với thông báo lỗi
                }
                var color = new Color
                {
                    idColor = colorDto.idColor,
                    nameColor = colorDto.nameColor
                };
                _context.Colors.Add(color);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(colorDto);
        }

        [Authorize(Roles = "Admin,StaffProduct")]
        // Hiển thị form Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var color = _context.Colors.FirstOrDefault(s => s.idColor == id);
            if (color == null)
            {
                return NotFound();
            }

            // Chuyển từ model sang DTO (nếu cần)
            var colorDto = new ColorDto
            {
                idColor = color.idColor,
                nameColor = color.nameColor,

            };

            return View(colorDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ColorDto colorDto)
        {
            if (ModelState.IsValid)
            {
                var color = _context.Colors.FirstOrDefault(s => s.idColor == colorDto.idColor);
                if (color == null)
                {
                    return NotFound();
                }

                // Cập nhật các giá trị từ DTO vào model
                color.nameColor = colorDto.nameColor;

                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang danh sách Color sau khi cập nhật
            }

            return View(colorDto);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var color = _context.Colors.Find(id);
            if (color == null)
            {
                return NotFound();
            }

            _context.Colors.Remove(color);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Color sau khi xóa
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteColorProduct(string idProduct, string idColor)
        {
            // Kiểm tra nếu idProduct hoặc idColor null hoặc rỗng
            if (string.IsNullOrEmpty(idProduct) || string.IsNullOrEmpty(idColor))
            {
                return NotFound();
            }

            // Tìm bản ghi ProductColor dựa trên idProduct và idColor
            var productColor = _context.ProductColors
                .FirstOrDefault(pc => pc.idProduct == idProduct && pc.idColor == idColor);

            if (productColor == null)
            {
                return NotFound(); // Không tìm thấy bản ghi
            }

            // Xóa bản ghi
            _context.ProductColors.Remove(productColor);
            _context.SaveChanges();

            // Quay lại danh sách sau khi xóa
            return RedirectToAction("ColorProduct", new { id = productColor.idProduct });
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult ColorProduct(string id)
        {
            var colors = _context.ProductColors.Include(p => p.Product).Include(p => p.Color).Where(pc => pc.idProduct == id).ToList(); // Lấy danh sách color của sản phẩm
            ViewBag.ProductId = id;
            return View(colors); // Trả về view với danh sách color
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult CreateColorProduct(string id)
        {
            ViewBag.ProductId = id;
            ViewBag.ColorList = _context.Colors.Select(s => new SelectListItem
            {
                Value = s.idColor.ToString(),
                Text = s.nameColor
            }).ToList(); // Sử dụng Select để tạo danh sách Color
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateColorProduct(ProductColor productColor)
        {
            // Kiểm tra xem cặp khóa (idProduct, idColor) đã tồn tại chưa
            var existingColorProduct = _context.ProductColors
                .FirstOrDefault(ps => ps.idProduct == productColor.idProduct && ps.idColor == productColor.idColor);

            if (existingColorProduct != null)
            {
                // Nếu đã tồn tại, thêm thông báo lỗi vào ModelState
                ModelState.AddModelError(string.Empty, "Màu sắc này đã tồn tại cho sản phẩm này.");
                TempData["error"] = "Kích thước này đã tồn tại cho sản phẩm này.";
                return RedirectToAction("CreateColorProduct", new { id = productColor.idProduct });

            }
            _context.ProductColors.Add(productColor);
            _context.SaveChanges();

            // Chuyển hướng về danh sách kích thước của sản phẩm


            // Nếu ModelState không hợp lệ, lấy lại danh sách Color
            ViewBag.ColorList = _context.Colors.Select(s => new SelectListItem
            {
                Value = s.idColor.ToString(),
                Text = s.nameColor
            }).ToList();

            // Trả về view với sản phẩm đã chọn
            return RedirectToAction("ColorProduct", new { id = productColor.idProduct });
        }
    }
}
