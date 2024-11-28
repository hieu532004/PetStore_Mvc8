using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class SizeController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public SizeController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var sizes = _context.Sizes.ToList();

            return View(sizes);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(SizeDto sizeDto)
        {

            if (ModelState.IsValid)
            {
              
                var existingSize = _context.Sizes.FirstOrDefault(s => s.idSize == sizeDto.idSize);
                if (existingSize != null)
                {
                    ModelState.AddModelError("idSize", $"Size with ID '{sizeDto.idSize}' already exists.");
                    return View(sizeDto); // Trả lại form với thông báo lỗi
                }
                            
                var size = new Size
                {
                    idSize = sizeDto.idSize,
                    nameSize = sizeDto.nameSize,
                };
                _context.Sizes.Add(size);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
         
            return View(sizeDto);
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

            // Tìm Size dựa trên id, sử dụng FirstOrDefault để kiểm tra an toàn hơn
            var size = _context.Sizes.FirstOrDefault(s => s.idSize == id);
            if (size == null)
            {
                return NotFound();
            }
          

            // Chuyển Size entity sang SizeDto để sử dụng trong view
            var sizeDto = new SizeDto
            {
                idSize = size.idSize,
                nameSize = size.nameSize,
            };

            return View(sizeDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SizeDto sizeDto)
        {
            if (ModelState.IsValid)
            {
                var size = _context.Sizes.FirstOrDefault(s => s.idSize == sizeDto.idSize);
                if (size == null)
                {
                    return NotFound();
                }
                
                // Cập nhật các giá trị từ DTO vào model
                size.nameSize = sizeDto.nameSize;

             
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
              

                return RedirectToAction("Index"); // Quay lại trang danh sách Size sau khi cập nhật
            }


            return View(sizeDto); // Trả về form với thông báo lỗi (nếu có)
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

            var size = _context.Sizes.Find(id);
            if (size == null)
            {
                return NotFound();
            }

            _context.Sizes.Remove(size);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Size sau khi xóa
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductSize(string idProduct, string idSize)
        {
            // Kiểm tra nếu idProduct hoặc idSize null hoặc rỗng
            if (string.IsNullOrEmpty(idProduct) || string.IsNullOrEmpty(idSize))
            {
                return NotFound();
            }

            // Tìm bản ghi ProductSize dựa trên idProduct và idSize
            var productSize = _context.ProductSizes
                .FirstOrDefault(ps => ps.idProduct == idProduct && ps.idSize == idSize);

            if (productSize == null)
            {
                return NotFound(); // Không tìm thấy bản ghi
            }

            // Xóa bản ghi
            _context.ProductSizes.Remove(productSize);
            _context.SaveChanges();

            // Quay lại danh sách sau khi xóa
            return RedirectToAction("SizeProduct", new { id = productSize.idProduct });
        }

        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult SizeProduct(string id)
        {
            var sizes = _context.ProductSizes.Include(p => p.Product).Include(p => p.Size).Where(ps => ps.idProduct == id).ToList();
            ViewBag.ProductId = id;
            return View(sizes); // Trả về view với danh sách size
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult CreateSizeProduct(string id)
        {
            ViewBag.ProductId = id;
            ViewBag.SizeList = _context.Sizes.Select(s => new SelectListItem
            {
                Value = s.idSize.ToString(),
                Text = s.nameSize
            }).ToList(); // Sử dụng Select để tạo danh sách Size
            return View();
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSizeProduct(ProductSize productSize)
        {
            // Kiểm tra xem cặp khóa (idProduct, idSize) đã tồn tại chưa
            var existingSizeProduct = _context.ProductSizes
                .FirstOrDefault(ps => ps.idProduct == productSize.idProduct && ps.idSize == productSize.idSize);

            if (existingSizeProduct != null)
            {
                // Nếu đã tồn tại, thêm thông báo lỗi vào ModelState
                ModelState.AddModelError(string.Empty, "Kích thước này đã tồn tại cho sản phẩm này.");
                TempData["error"] = "Kích thước này đã tồn tại cho sản phẩm này.";
                return RedirectToAction("CreateSizeProduct", new { id = productSize.idProduct });

            }
            _context.ProductSizes.Add(productSize);
            _context.SaveChanges();

            // Chuyển hướng về danh sách kích thước của sản phẩm


            // Nếu ModelState không hợp lệ, lấy lại danh sách Size
            ViewBag.SizeList = _context.Sizes.Select(s => new SelectListItem
            {
                Value = s.idSize.ToString(),
                Text = s.nameSize
            }).ToList();

            // Trả về view với sản phẩm đã chọn
            return RedirectToAction("SizeProduct", new { id = productSize.idProduct });
        }



    }
}
