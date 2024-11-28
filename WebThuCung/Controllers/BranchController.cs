using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class BranchController : Controller
    {

        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public BranchController(PetContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var branchs = _context.Branchs.ToList();

            // Truyền danh sách admin sang view
            return View(branchs);

        }
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(BranchCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingBranch = _context.Branchs.FirstOrDefault(b => b.idBranch == model.idBranch);
            if (existingBranch != null)
            {
                ModelState.AddModelError("idBranch", "Branch ID already exists."); // Thêm lỗi vào ModelState
                return View(model); // Trả về view với thông báo lỗi
            }
            // Xử lý tải lên logo
            string logoFilePath = null;
            if (model.Logo != null && model.Logo.Length > 0)
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/brand-logo", model.Logo.FileName);
                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    model.Logo.CopyTo(stream);
                }
                logoFilePath = model.Logo.FileName; // Cập nhật tên tệp logo
            }

           

            // Tạo đối tượng Branch và lưu vào cơ sở dữ liệu
            var newBranch = new Branch
            {
                idBranch = model.idBranch,
                nameBranch = model.nameBranch,
                // Nếu bạn lưu đường dẫn logo và CV vào DB, thêm vào đây
                Logo = logoFilePath,
                
            };

            _context.Branchs.Add(newBranch);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách chi nhánh
        }
        [Authorize(Roles = "Admin,StaffProduct")]

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var branch = _context.Branchs.FirstOrDefault(s => s.idBranch == id);
            if (branch == null)
            {
                return NotFound();
            }

            // Chuyển từ model sang DTO (nếu cần)
            var branchDto = new BranchDto
            {
                idBranch = branch.idBranch,
                nameBranch = branch.nameBranch,
                // Không cần lấy Logo ở đây vì sẽ không hiển thị trong input
            };

            return View(branchDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BranchDto branchDto)
        {
            if (ModelState.IsValid)
            {
                var branch = _context.Branchs.FirstOrDefault(s => s.idBranch == branchDto.idBranch);
                if (branch == null)
                {
                    return NotFound();
                }

                // Cập nhật các giá trị từ DTO vào model
                branch.nameBranch = branchDto.nameBranch;

                // Xử lý tải lên logo
                if (branchDto.Logo != null && branchDto.Logo.Length > 0)
                {
                    var cvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/brand-logo", branchDto.Logo.FileName);
                    if (System.IO.File.Exists(cvPath))
                    {
                        System.IO.File.Delete(cvPath);
                    }

                    using (var stream = new FileStream(cvPath, FileMode.Create, FileAccess.Write))
                    {
                        branchDto.Logo.CopyTo(stream);
                    }
                    branch.Logo = branchDto.Logo.FileName;
                }


                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang danh sách Branch sau khi cập nhật
            }

            return View(branchDto);
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

            var branch = _context.Branchs.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branchs.Remove(branch);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách Branch sau khi xóa
        }


    }
}
