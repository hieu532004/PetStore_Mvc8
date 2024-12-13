using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class WardController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public WardController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var wards = _context.Wards.Include(w => w.District).ToList();
            return View(wards);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: Ward/Create
        public IActionResult Create()
        {
            ViewBag.Districts = _context.Districts.Select(d => new SelectListItem
            {
                Value = d.idDistrict,
                Text = d.nameDistrict
            }).ToList();

            return View(new WardDto());
        }

        // POST: Ward/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WardDto model)
        {
            var existingWard = _context.Wards.FirstOrDefault(d => d.idWard == model.idWard);
            if (existingWard != null)
            {
                ModelState.AddModelError("idWard", "Ward ID already exists.");
            }
            if (ModelState.IsValid)
            {
                var newWard = new Ward
                {
                    idWard = model.idWard,
                    nameWard = model.nameWard,
                    idDistrict = model.idDistrict
                };

                _context.Wards.Add(newWard);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Districts in case of validation errors
            // Tái tạo lại ViewBag.Districts trong trường hợp có lỗi xác thực
            ViewBag.Districts = _context.Districts
                .Select(d => new SelectListItem
                {
                    Value = d.idDistrict,
                    Text = d.nameDistrict
                }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: Ward/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = _context.Wards.Find(id);
            if (ward == null)
            {
                return NotFound();
            }

            var model = new WardDto
            {
                idWard = ward.idWard,
                nameWard = ward.nameWard,
                idDistrict = ward.idDistrict
            };

            // Populate ViewBag.Districts for the dropdown
            ViewBag.Districts = _context.Districts
                .Select(d => new SelectListItem
                {
                    Value = d.idDistrict,
                    Text = d.nameDistrict
                }).ToList();

            return View(model);
        }

        // POST: Ward/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WardDto model)
        {
            if (ModelState.IsValid)
            {
                var ward = _context.Wards.Find(model.idWard);
                if (ward == null)
                {
                    return NotFound();
                }

                ward.nameWard = model.nameWard;
                ward.idDistrict = model.idDistrict;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Districts in case of validation errors
            ViewBag.Districts = _context.Districts
                .Select(d => new SelectListItem
                {
                    Value = d.idDistrict,
                    Text = d.nameDistrict
                }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // POST: Ward/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var ward = _context.Wards.Find(id);
            if (ward == null)
            {
                return NotFound();
            }

            _context.Wards.Remove(ward);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
