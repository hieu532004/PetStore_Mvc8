using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class PetController : Controller
    {
        private readonly PetContext _context;
        public PetController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var pets = _context.Pets.ToList();

            // Truyền danh sách admin sang view
            return View(pets);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(PetDto petDto)
        {
            if (ModelState.IsValid)
            {
                var existingPet = _context.Pets.FirstOrDefault(s => s.idPet == petDto.idPet);
                if (existingPet != null)
                {
                    ModelState.AddModelError("idPet", $"Pet with ID '{petDto.idPet}' already exists.");
                    return View(petDto); // Trả lại form với thông báo lỗi
                }
                var pet = new Pet
                {
                    idPet = petDto.idPet,
                    namePet = petDto.namePet
                };
                _context.Pets.Add(pet);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(petDto);
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

            var pet = _context.Pets.FirstOrDefault(s => s.idPet == id);
            if (pet == null)
            {
                return NotFound();
            }

            // Chuyển từ model sang DTO (nếu cần)
            var petDto = new PetDto
            {
                idPet = pet.idPet,
                namePet = pet.namePet,

            };

            return View(petDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PetDto petDto)
        {
            if (ModelState.IsValid)
            {
                var pet = _context.Pets.FirstOrDefault(s => s.idPet == petDto.idPet);
                if (pet == null)
                {
                    return NotFound();
                }

                // Cập nhật các giá trị từ DTO vào model
                pet.namePet = petDto.namePet;

                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang danh sách Pet sau khi cập nhật
            }

            return View(petDto);
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

            var pet = _context.Pets.Find(id);
            if (pet == null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Pet sau khi xóa
        }
    }
}
