using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class CategoryController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public CategoryController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var categories = _context.Categories.ToList();

            // Truyền danh sách admin sang view
            return View(categories);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = _context.Categories.FirstOrDefault(s => s.idCategory == categoryDto.idCategory);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("idCategory", $"Category with ID '{categoryDto.idCategory}' already exists.");
                    return View(categoryDto); // Trả lại form với thông báo lỗi
                }
                var category = new Category
                {
                    idCategory = categoryDto.idCategory,
                    nameCategory = categoryDto.nameCategory
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryDto);
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

            var category = _context.Categories.FirstOrDefault(s => s.idCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            // Chuyển từ model sang DTO (nếu cần)
            var CategoryDto = new CategoryDto
            {
                idCategory = category.idCategory,
                nameCategory = category.nameCategory,

            };

            return View(CategoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                var category = _context.Categories.FirstOrDefault(s => s.idCategory == categoryDto.idCategory);
                if (category == null)
                {
                    return NotFound();
                }

                // Cập nhật các giá trị từ DTO vào model
                category.nameCategory = categoryDto.nameCategory;

                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang danh sách Category sau khi cập nhật
            }

            return View(categoryDto);
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

            var Category = _context.Categories.Find(id);
            if (Category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(Category);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Category sau khi xóa
        }


    }
}
