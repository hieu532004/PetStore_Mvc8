using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class CityController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public CityController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cities = _context.Cities.Include(c => c.Country).ToList();
            return View(cities);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: City/Create
        public IActionResult Create()
        {
            // Khởi tạo ViewModel mới
            var model = new CityDto();

            // Cung cấp danh sách các quốc gia cho dropdown
            ViewBag.Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.idCountry.ToString(),
                Text = c.nameCountry
            }).ToList();

            return View(model);
        }


        // POST: City/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CityDto model)
        {
            var existingCity = _context.Cities.FirstOrDefault(d => d.idCity == model.idCity);
            if (existingCity != null)
            {
                ModelState.AddModelError("idCity", "City ID already exists.");
            }
            if (ModelState.IsValid)
            {
                var newCity = new City
                {
                    idCity = model.idCity,
                    nameCity = model.nameCity,
                    idCountry = model.idCountry
                };

                _context.Cities.Add(newCity);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Countries in case of validation errors
            ViewBag.Countries = _context.Countries
                .Select(c => new SelectListItem
                {
                    Value = c.idCountry,
                    Text = c.nameCountry
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // GET: City/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = _context.Cities.Find(id);
            if (city == null)
            {
                return NotFound();
            }

            var cityDto = new CityDto
            {
                idCity = city.idCity,
                nameCity = city.nameCity,
                idCountry = city.idCountry
            };

            // Populate ViewBag.Countries for the dropdown
            ViewBag.Countries = _context.Countries
                .Select(c => new SelectListItem
                {
                    Value = c.idCountry,
                    Text = c.nameCountry
                })
                .ToList();

            return View(cityDto);
        }

        // POST: City/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CityDto model)
        {
            if (ModelState.IsValid)
            {
                var city = _context.Cities.Find(model.idCity);
                if (city == null)
                {
                    return NotFound();
                }

                city.nameCity = model.nameCity;
                city.idCountry = model.idCountry;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.Countries in case of validation errors
            ViewBag.Countries = _context.Countries
                .Select(c => new SelectListItem
                {
                    Value = c.idCountry,
                    Text = c.nameCountry
                })
                .ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin,StaffOrder")]
        // POST: City/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var city = _context.Cities.Find(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
