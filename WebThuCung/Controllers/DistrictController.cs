using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class DistrictController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public DistrictController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var districts = _context.Districts.Include(d => d.City).ToList();
            return View(districts);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: District/Create
        public IActionResult Create()
        {
            // Khởi tạo ViewModel mới
            var model = new DistrictDto();

            // Cung cấp danh sách các thành phố cho dropdown
            ViewBag.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.idCity,
                    Text = c.nameCity
                })
                .ToList();

            return View(model);
        }

        // POST: District/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DistrictDto model)
        {
            var existingDistrict = _context.Districts.FirstOrDefault(d => d.idDistrict == model.idDistrict);
            if (existingDistrict != null)
            {
                ModelState.AddModelError("idDistrict", "District ID already exists.");
            }
            if (ModelState.IsValid)
            {
                var newDistrict = new District
                {
                    idDistrict = model.idDistrict,
                    nameDistrict = model.nameDistrict,
                    idCity = model.idCity
                };

                _context.Districts.Add(newDistrict);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Cities in case of validation errors
            ViewBag.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.idCity,
                    Text = c.nameCity
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: District/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = _context.Districts.Find(id);
            if (district == null)
            {
                return NotFound();
            }

            // Populate ViewModel with data from the database
            var model = new DistrictDto
            {
                idDistrict = district.idDistrict,
                nameDistrict = district.nameDistrict,
                idCity = district.idCity
            };

            // Populate ViewBag.Cities for the dropdown
            ViewBag.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.idCity,
                    Text = c.nameCity
                })
                .ToList();

            return View(model);
        }

        // POST: District/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DistrictDto model)
        {
            if (ModelState.IsValid)
            {
                var district = _context.Districts.Find(model.idDistrict);
                if (district == null)
                {
                    return NotFound();
                }

                district.nameDistrict = model.nameDistrict;
                district.idCity = model.idCity;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Cities in case of validation errors
            ViewBag.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.idCity,
                    Text = c.nameCity
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // POST: District/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var district = _context.Districts.Find(id);
            if (district == null)
            {
                return NotFound();
            }

            _context.Districts.Remove(district);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
