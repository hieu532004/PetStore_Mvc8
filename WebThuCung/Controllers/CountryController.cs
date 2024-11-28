using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class CountryController : Controller
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public CountryController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các country từ cơ sở dữ liệu
            var countries = _context.Countries.ToList();

            // Truyền danh sách country sang view
            return View(countries);
        }

        [Authorize(Roles = "Admin,StaffOrder")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CountryDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingCountry = _context.Countries.FirstOrDefault(c => c.idCountry == model.idCountry);
            if (existingCountry != null)
            {
                ModelState.AddModelError("idCountry", "Country ID already exists.");
                return View(model);
            }

            // Create a new Country entity and save it to the database
            var newCountry = new Country
            {
                idCountry = model.idCountry,
                nameCountry = model.nameCountry,
            };

            _context.Countries.Add(newCountry);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,StaffOrder")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var country = _context.Countries.FirstOrDefault(c => c.idCountry == id);
            if (country == null)
            {
                return NotFound();
            }

            var countryDto = new CountryDto
            {
                idCountry = country.idCountry,
                nameCountry = country.nameCountry,
            };

            return View(countryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CountryDto countryDto)
        {
            if (ModelState.IsValid)
            {
                var country = _context.Countries.FirstOrDefault(c => c.idCountry == countryDto.idCountry);
                if (country == null)
                {
                    return NotFound();
                }

                // Update country properties
                country.nameCountry = countryDto.nameCountry;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(countryDto);
        }

        [Authorize(Roles = "Admin,StaffOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var country = _context.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
