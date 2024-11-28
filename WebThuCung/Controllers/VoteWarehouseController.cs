using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Dto;
using WebThuCung.Models;
using static NuGet.Packaging.PackagingConstants;

namespace WebThuCung.Controllers
{
    public class VoteWarehouseController : Controller
    {
        private readonly PetContext _context;
        public VoteWarehouseController(PetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var voteWarehouses = _context.VoteWarehouses
                .Include(v => v.Supplier)
                .Include(d => d.DetailVoteWarehouses)// Kết hợp thông tin nhà cung cấp
                .ToList();
            // Tính toán tổng giá trị cho mỗi đơn hàng
            foreach (var vote in voteWarehouses)
            {
                vote.CalculateTotalVoteWarehouse(); // Calculate total order based on DetailOrders
            }


            return View(voteWarehouses);
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // GET: VoteWarehouse/Create
        public IActionResult Create()
        {
            // Lấy danh sách nhà cung cấp để hiển thị trong dropdown
            ViewBag.Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.idSupplier,
                Text = s.nameSupplier
            }).ToList();

            return View(new VoteWarehouseDto());
        }

        // POST: VoteWarehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VoteWarehouseDto voteWarehouseDto)
        {
            ViewBag.Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.idSupplier,
                Text = s.nameSupplier
            }).ToList();

            // Kiểm tra ModelState
            if (ModelState.IsValid)
            {
                var existingvotewarehouse = _context.VoteWarehouses.FirstOrDefault(s => s.idVotewarehouse == voteWarehouseDto.idVoteWarehouse);
                if (existingvotewarehouse != null)
                {
                    ModelState.AddModelError("", $"votewarehouse with ID '{voteWarehouseDto.idVoteWarehouse}' already exists.");
                    return View(voteWarehouseDto); // Trả lại form với thông báo lỗi
                }
                // Tạo mới đối tượng VoteWarehouse từ DTO
                var voteWarehouse = new VoteWarehouse
                {
                    idVotewarehouse = voteWarehouseDto.idVoteWarehouse, // Tạo ID mới
                    dateEntry = voteWarehouseDto.dateEntry, // Ngày nhập
                    idSupplier = voteWarehouseDto.idSupplier,                  
                };
                voteWarehouse.CalculateTotalVoteWarehouse();
                _context.VoteWarehouses.Add(voteWarehouse);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index)); // Quay lại trang danh sách
            }

            // Nếu ModelState không hợp lệ, lấy lại danh sách nhà cung cấp
          
            return View(voteWarehouseDto);
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // GET: VoteWarehouse/Edit/{id}
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var voteWarehouse = _context.VoteWarehouses.FirstOrDefault(s => s.idVotewarehouse == id);
            if (voteWarehouse == null)
            {
                return NotFound();
            }

            var voteWarehouseDto = new VoteWarehouseDto
            {
                idVoteWarehouse = voteWarehouse.idVotewarehouse,
                dateEntry = voteWarehouse.dateEntry,
                idSupplier = voteWarehouse.idSupplier,
       
                totalVoteWarehouse = voteWarehouse.totalVoteWarehouse
            };

            ViewBag.Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.idSupplier,
                Text = s.nameSupplier
            }).ToList();

            return View(voteWarehouseDto);
        }

        // POST: VoteWarehouse/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VoteWarehouseDto voteWarehouseDto)
        {
            if (ModelState.IsValid)
            {
                var voteWarehouse = _context.VoteWarehouses.FirstOrDefault(s => s.idVotewarehouse == voteWarehouseDto.idVoteWarehouse);
                if (voteWarehouse == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin đối tượng VoteWarehouse
                voteWarehouse.dateEntry = voteWarehouseDto.dateEntry;
                voteWarehouse.idSupplier = voteWarehouseDto.idSupplier;
             
                voteWarehouse.CalculateTotalVoteWarehouse();

                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); // Quay lại trang danh sách
            }

            // Nếu ModelState không hợp lệ, lấy lại danh sách nhà cung cấp
            ViewBag.Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.idSupplier,
                Text = s.nameSupplier
            }).ToList();

            return View(voteWarehouseDto);
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // POST: VoteWarehouse/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var voteWarehouse = _context.VoteWarehouses.Find(id);
            if (voteWarehouse == null)
            {
                return NotFound();
            }

            _context.VoteWarehouses.Remove(voteWarehouse);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index)); // Quay lại trang danh sách
        }
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy danh sách các DetailVoteWarehouses cho một phiếu kho cụ thể và bao gồm thông tin sản phẩm liên quan
            var detailVoteWarehouses = _context.DetailVoteWarehouses
                .Include(d => d.Product)
                .Where(d => d.idVotewarehouse == id)
                .ToList();

            // Tính tổng giá cho mỗi DetailVoteWarehouse
            foreach (var detailVoteWarehouse in detailVoteWarehouses)
            {
                detailVoteWarehouse.totalPrice = detailVoteWarehouse.CalculateTotalPriceWare();
            }

            // Truyền id vào view để liên kết trở lại phiếu kho
            ViewBag.VoteWarehouseId = id;

            return View(detailVoteWarehouses); // Trả về view detail cho DetailVoteWarehouses
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // GET: DetailVoteWarehouse/Create/{voteWarehouseId}
        [HttpGet]
        public IActionResult CreateDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Truyền voteWarehouseId sang view
            ViewBag.VoteWarehouseId = id;

            // Truyền danh sách sản phẩm để chọn lựa
            ViewBag.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct.ToString(),
                Text = p.nameProduct
            }).ToList();
            var model = new DetailVoteWarehouseDto
            {
                idVoteWarehouse = id// Hoặc gán giá trị hợp lệ cho idOrder
            };

            return View(model);
        }

        // POST: DetailVoteWarehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDetail(DetailVoteWarehouseDto detailVoteWarehouseDto)
        {
            if (ModelState.IsValid)
            {
                // Chuyển đổi từ DTO sang model DetailVoteWarehouse
                var detailVoteWarehouse = new DetailVoteWarehouse
                {
                    idVotewarehouse = detailVoteWarehouseDto.idVoteWarehouse,
                    idProduct = detailVoteWarehouseDto.idProduct,
                    Quantity = detailVoteWarehouseDto.Quantity,
                    purchasePrice = detailVoteWarehouseDto.purchasePrice,
                    totalPrice = detailVoteWarehouseDto.Quantity * detailVoteWarehouseDto.purchasePrice // Tính tổng giá
                };

                _context.DetailVoteWarehouses.Add(detailVoteWarehouse);
                _context.SaveChanges();

                // Chuyển hướng về danh sách DetailVoteWarehouse của phiếu kho cụ thể
                return RedirectToAction("Detail", new { id = detailVoteWarehouse.idVotewarehouse });
            }

            // Nếu không hợp lệ, tải lại form
            ViewBag.VoteWarehouseId = detailVoteWarehouseDto.idVoteWarehouse;
            ViewBag.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct.ToString(),
                Text = p.nameProduct
            }).ToList();

            return View(detailVoteWarehouseDto);
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // GET: DetailVoteWarehouse/Edit/{voteWarehouseId}/{productId}
        [HttpGet]
        public IActionResult EditDetail(string voteWarehouseId, string productId)
        {
            if (string.IsNullOrEmpty(voteWarehouseId) || string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }

            var detailVoteWarehouse = _context.DetailVoteWarehouses
                .FirstOrDefault(d => d.idVotewarehouse == voteWarehouseId && d.idProduct == productId);

            if (detailVoteWarehouse == null)
            {
                return NotFound();
            }

            var detailVoteWarehouseDto = new DetailVoteWarehouseDto
            {
                idVoteWarehouse = detailVoteWarehouse.idVotewarehouse,
                idProduct = detailVoteWarehouse.idProduct,
                Quantity = detailVoteWarehouse.Quantity,
                purchasePrice = detailVoteWarehouse.purchasePrice,
                totalPrice = detailVoteWarehouse.CalculateTotalPriceWare()
            };

            ViewBag.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct.ToString(),
                Text = p.nameProduct
            }).ToList();

            return View(detailVoteWarehouseDto);
        }

        // POST: DetailVoteWarehouse/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetail(DetailVoteWarehouseDto detailVoteWarehouseDto)
        {
            if (ModelState.IsValid)
            {
                var detailVoteWarehouse = _context.DetailVoteWarehouses
                    .FirstOrDefault(d => d.idVotewarehouse == detailVoteWarehouseDto.idVoteWarehouse && d.idProduct == detailVoteWarehouseDto.idProduct);

                if (detailVoteWarehouse == null)
                {
                    return NotFound();
                }

                detailVoteWarehouse.Quantity = detailVoteWarehouseDto.Quantity;
                detailVoteWarehouse.purchasePrice = detailVoteWarehouseDto.purchasePrice;
                detailVoteWarehouse.totalPrice = detailVoteWarehouseDto.Quantity * detailVoteWarehouseDto.purchasePrice;

                _context.SaveChanges();

                return RedirectToAction("Detail", new { id = detailVoteWarehouse.idVotewarehouse });
            }

            ViewBag.Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.idProduct.ToString(),
                Text = p.nameProduct
            }).ToList();

            return View(detailVoteWarehouseDto);
        }
        [Authorize(Roles = "Admin,StaffWareHouse")]
        // POST: DetailVoteWarehouse/Delete/{voteWarehouseId}/{productId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDetail(string id, string productId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }

            // Fetch the detail vote warehouse to delete
            var detailVoteWarehouse = _context.DetailVoteWarehouses
                .FirstOrDefault(d => d.idVotewarehouse == id && d.idProduct == productId);

            if (detailVoteWarehouse == null)
            {
                return NotFound();
            }

            _context.DetailVoteWarehouses.Remove(detailVoteWarehouse);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id });
        }
    }
}
