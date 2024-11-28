using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class DiscountController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public DiscountController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var discounts = _context.Discounts.ToList();

            // Truyền danh sách admin sang view
            return View(discounts);
        }
        public IActionResult Create()
        {
            var products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct,
                Text = p.nameProduct
            }).ToList();

            // Truyền danh sách sản phẩm vào ViewBag để sử dụng trong view
            ViewBag.Products = products;
            return View();
        }
        [HttpPost]
        public IActionResult Create(DiscountDto discountDto)
        {
            var products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct,
                Text = p.nameProduct
            }).ToList();

            // Truyền danh sách sản phẩm vào ViewBag để sử dụng trong view
            ViewBag.Products = products;
            if (ModelState.IsValid)
            {
                
               var existingDiscount = _context.Discounts.FirstOrDefault(s => s.idDiscount == discountDto.idDiscount);
                if (existingDiscount != null)
                {
                    ModelState.AddModelError("idDiscount", $"Discount with ID '{discountDto.idDiscount}' already exists.");
                    return View(discountDto); // Trả lại form với thông báo lỗi
                }

                var discount = new Discount
                {
                    idDiscount = discountDto.idDiscount,
                    discountPercent = discountDto.discountPercent,
                    idProduct = discountDto.idProduct
                };
                _context.Discounts.Add(discount);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(discountDto);
        }

        // Hiển thị form Edit
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Tìm Discount dựa trên id, sử dụng FirstOrDefault để kiểm tra an toàn hơn
            var discount = _context.Discounts.FirstOrDefault(s => s.idDiscount == id);
            if (discount == null)
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm để hiển thị trong dropdown
            var products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct,
                Text = p.nameProduct
            }).ToList();

            // Truyền danh sách sản phẩm vào ViewBag để sử dụng trong view
            ViewBag.Products = products;

            // Chuyển Discount entity sang DiscountDto để sử dụng trong view
            var discountDto = new DiscountDto
            {
                idDiscount = discount.idDiscount,
                discountPercent = discount.discountPercent,
                idProduct = discount.idProduct
            };

            return View(discountDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DiscountDto discountDto)
        {
            if (ModelState.IsValid)
            {
                var discount = _context.Discounts.FirstOrDefault(s => s.idDiscount == discountDto.idDiscount);
                if (discount == null)
                {
                    return NotFound();
                }

                // Cập nhật các giá trị từ DTO vào model
                discount.discountPercent = discountDto.discountPercent;
                discount.idProduct = discountDto.idProduct;
              
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu


                return RedirectToAction("Index"); // Quay lại trang danh sách Discount sau khi cập nhật
            }

            // Nếu ModelState không hợp lệ, truyền lại danh sách sản phẩm vào ViewBag
            var products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct,
                Text = p.nameProduct
            }).ToList();
            ViewBag.Products = products;

            return View(discountDto); // Trả về form với thông báo lỗi (nếu có)
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var discount = _context.Discounts.Find(id);
            if (discount == null)
            {
                return NotFound();
            }

            _context.Discounts.Remove(discount);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Discount sau khi xóa
        }
    }
}
