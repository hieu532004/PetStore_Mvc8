using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class SupplierController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public SupplierController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var suppliers = _context.Suppliers.ToList();

            // Truyền danh sách admin sang view
            return View(suppliers);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SupplierCreateDto supplierDto)
        {
            if (ModelState.IsValid)
            {
                var existingSupplier = _context.Suppliers.FirstOrDefault(s => s.idSupplier == supplierDto.idSupplier);
                if (existingSupplier != null)
                {
                    ModelState.AddModelError("idSupplier", $"Supplier with ID '{supplierDto.idSupplier}' already exists.");
                    return View(supplierDto); // Trả lại form với thông báo lỗi
                }
                string ImageFilePath = null;
                if (supplierDto.Image != null && supplierDto.Image.Length > 0)
                {
                    var ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Customer", supplierDto.Image.FileName);
                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        supplierDto.Image.CopyTo(stream);
                    }
                    ImageFilePath = supplierDto.Image.FileName; // Cập nhật tên tệp Image
                }
                // Chuyển đổi DTO sang model Supplier
                var supplier = new Supplier
                {
                    idSupplier =supplierDto.idSupplier,
                    nameSupplier = supplierDto.NameSupplier,
                    Phone = supplierDto.Phone,
                    Address = supplierDto.Address,
                    Email = supplierDto.Email,
                    Image = ImageFilePath
                };

                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại danh sách nhà cung cấp
            }

            return View(supplierDto);
        }
        [Authorize(Roles = "Admin")]
        // GET: Supplier/Edit/{id}
        public IActionResult Edit(string id)
        {
            var supplier = _context.Suppliers.FirstOrDefault(s => s.idSupplier == id);
            if (supplier == null)
            {
                return NotFound();
            }

            var supplierDto = new SupplierDto
            {
                idSupplier = supplier.idSupplier,
                NameSupplier = supplier.nameSupplier,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Email = supplier.Email,

            };

            return View(supplierDto);
        }

        // POST: Supplier/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SupplierDto supplierDto)
        {
            if (ModelState.IsValid)
            {
                var supplier = _context.Suppliers.FirstOrDefault(s => s.idSupplier== supplierDto.idSupplier);
                if (supplier == null)
                {
                    return NotFound();
                }

                if (supplierDto.Image != null && supplierDto.Image.Length > 0)
                {
                    var cvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Customer", supplierDto.Image.FileName);
                    using (var stream = new FileStream(cvPath, FileMode.Create))
                    {
                        supplierDto.Image.CopyToAsync(stream);
                    }
                    supplier.Image = supplierDto.Image.FileName;
                }
                supplier.nameSupplier = supplierDto.NameSupplier;
                supplier.Phone = supplierDto.Phone;
                supplier.Address = supplierDto.Address;
                supplier.Email = supplierDto.Email;
               
                _context.Update(supplier);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplierDto);
        }
        [Authorize(Roles = "Admin")]
        // GET: Supplier/Delete/{id}
        public IActionResult Delete(string id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }
        [Authorize(Roles = "Admin")]
        // POST: Supplier/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
