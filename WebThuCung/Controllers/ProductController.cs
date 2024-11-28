using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using WebThuCung.Data;
using WebThuCung.Dto;

using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class ProductController : BaseController
    {
        private readonly PetContext _context; // Biến để truy cập cơ sở dữ liệu

        public ProductController(PetContext context) : base(context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy danh sách các admin từ cơ sở dữ liệu
            var products = _context.Products
                                  .Include(p => p.Pet)
                                  .Include(p => p.Branch)
                                  .Include(p => p.Category)
                                  .Select(sp => new ProductViewDto
                                  {
                                      idProduct = sp.idProduct,
                                      nameProduct = sp.nameProduct,
                                      sellPrice = sp.sellPrice,
                                      Image = sp.Image,
                                      idBranch = sp.idBranch,
                                      idCategori = sp.idCategory,
                                      nameBranch = sp.Branch.nameBranch,       // Lấy tên thương hiệu từ đối tượng liên quan
                                      nameCategory = sp.Category.nameCategory, // Lấy tên loại từ đối tượng liên quan
                                      Quantity = sp.Quantity,
                                      Description = sp.Description,
                                      Logo = sp.Branch.Logo,
                                      DiscountedPrice = sp.sellPrice - (sp.sellPrice * (_context.Discounts
               .Where(d => d.idProduct == sp.idProduct)
               .Select(d => d.discountPercent)
               .FirstOrDefault() / 100m)),
                                      DiscountPercent = _context.Discounts
               .Where(d => d.idProduct == sp.idProduct)
               .Select(d => d.discountPercent)
               .FirstOrDefault()// Lấy logo từ đối tượng liên quan
                                  })
                                  .ToList();

            // Truyền danh sách admin sang viewsds
            return View(products);
        }
       
        [HttpGet]
        public IActionResult SearchByPet(string petName)
        {
            DateTime startDate = DateTime.Today.AddDays(-30);

            DateTime endDate = DateTime.Today;
            var customerId = GetCustomerIdFromSession();
            // Lấy danh sách các sản phẩm theo tên thú cưng (namePet) và ánh xạ sang ProductViewDto
            var products = _context.Products
                                   .Include(p => p.Pet)
                                   .Include(p => p.Branch)
                                   .Include(p => p.Category)
                                   .Where(p => p.Pet.namePet == petName) // Tìm kiếm theo tên thú cưng
                                   .Select(sp => new ProductViewDto
                                   {
                                       idProduct = sp.idProduct,
                                       nameProduct = sp.nameProduct,
                                       sellPrice = sp.sellPrice,
                                       Image = sp.Image,
                                       idBranch = sp.idBranch,
                                       idCategori = sp.idCategory,
                                       nameBranch = sp.Branch.nameBranch,       // Lấy tên thương hiệu từ đối tượng liên quan
                                       nameCategory = sp.Category.nameCategory, // Lấy tên loại từ đối tượng liên quan
                                       Quantity = sp.Quantity,
                                       Description = sp.Description,
                                       Logo = sp.Branch.Logo,
                                       DiscountedPrice = sp.sellPrice - (sp.sellPrice * (_context.Discounts
                .Where(d => d.idProduct == sp.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault() / 100m)),
                                       DiscountPercent = _context.Discounts
                .Where(d => d.idProduct == sp.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault()// Lấy logo từ đối tượng liên quan
                                   })
                                   .ToList();
            var savedProductIds = _context.SaveProducts
            .Where(s => s.idCustomer == customerId)
            .Select(s => s.idProduct)
            .ToList();

            // Gán danh sách cho ViewBag để sử dụng trong View
            ViewBag.SavedProductIds = savedProductIds;

            var topSellingProducts = _context.Orders
                 .Where(o => o.dateFrom.Date >= startDate && o.dateFrom.Date <= endDate)
                 .SelectMany(o => o.DetailOrders)
                 .GroupBy(d => new { d.idProduct, d.Product.nameProduct, d.Product.sellPrice, d.Product.Image })
                 .Select(g => new TopSellingProductDto
                 {
                     ProductId = g.Key.idProduct,
                     ProductName = g.Key.nameProduct,
                     Price = g.Key.sellPrice,
                     DiscountedPrice = g.Key.sellPrice - (g.Key.sellPrice * (_context.Discounts
                 .Where(d => d.idProduct == g.Key.idProduct)
                 .Select(d => d.discountPercent)
                 .FirstOrDefault() / 100m)),
                     Sold = g.Sum(d => d.Quantity),
                     Revenue = g.Sum(d => d.Product.sellPrice * d.Quantity),
                     ImageUrl = g.Key.Image,
                     DiscountPercent = _context.Discounts
                 .Where(d => d.idProduct == g.Key.idProduct)
                 .Select(d => d.discountPercent)
                 .FirstOrDefault()
                 })
                 .OrderByDescending(p => p.Sold)
                 .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                 .ToList();

            // Gán danh sách sản phẩm bán chạy nhất vào ViewBag để sử dụng trong View
            ViewBag.TopSellingProducts = topSellingProducts;

            // Truyền dữ liệu sản phẩm vào View
            return View(products);
        }
        [HttpGet]
        public IActionResult Shop(int page = 1, int pageSize = 3)
        {
            var customerId = GetCustomerIdFromSession();

            var productsQuery = _context.Products
                .Include(sp => sp.Branch)
                .Include(sp => sp.Category)
                .AsQueryable();

            var allProducts = productsQuery
                .Select(sp => new ProductViewDto
                {
                    idProduct = sp.idProduct,
                    nameProduct = sp.nameProduct,
                    sellPrice = sp.sellPrice,
                    Image = sp.Image,
                    idBranch = sp.idBranch,
                    idCategori = sp.idCategory,
                    nameBranch = sp.Branch.nameBranch,
                    nameCategory = sp.Category.nameCategory,
                    Quantity = sp.Quantity,
                    Description = sp.Description,
                    Logo = sp.Branch.Logo,
                    // Tính giá chiết khấu
                    DiscountedPrice = sp.sellPrice - (sp.sellPrice * (_context.Discounts
                        .Where(d => d.idProduct == sp.idProduct)
                        .Select(d => d.discountPercent)
                        .FirstOrDefault() / 100m)),
                    // Lấy phần trăm giảm giá
                    DiscountPercent = _context.Discounts
                        .Where(d => d.idProduct == sp.idProduct)
                        .Select(d => d.discountPercent)
                        .FirstOrDefault()
                })
                .OrderBy(p => p.idProduct)
                .Skip((page - 1) * pageSize) // Bỏ qua các sản phẩm của các trang trước
                .Take(pageSize) // Lấy sản phẩm của trang hiện tại
                .ToList();

            // Tính toán tổng số sản phẩm và số trang
            var totalProductsCount = productsQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalProductsCount / pageSize);

            // Lưu thông tin phân trang vào ViewBag để sử dụng trong view
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SavedProductIds = _context.SaveProducts
                .Where(s => s.idCustomer == customerId)
                .Select(s => s.idProduct)
                .ToList();

            return View(allProducts);
        }

        [HttpGet]
        public IActionResult ProductByCategory(string idCategory)
        {
            DateTime startDate = DateTime.Today.AddDays(-30);

            DateTime endDate = DateTime.Today;
            var customerId = GetCustomerIdFromSession();

            // Kiểm tra nếu idCategory là "Shop by Category" hoặc rỗng thì lấy toàn bộ sản phẩm
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Branch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(idCategory) && idCategory != "Shop by Category")
            {
                productsQuery = productsQuery.Where(p => p.idCategory == idCategory);
            }

            // Lấy dữ liệu cần thiết và chuyển sang DTO
            var products = productsQuery
                .Select(p => new ProductViewDto
                {
                    idProduct = p.idProduct,
                    nameProduct = p.nameProduct,
                    sellPrice = p.sellPrice,
                    Image = p.Image,
                    idBranch = p.idBranch,
                    idCategori = p.idCategory,
                    nameBranch = p.Branch.nameBranch,
                    nameCategory = p.Category.nameCategory,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    Logo = p.Branch.Logo,
                    DiscountedPrice = p.sellPrice - (p.sellPrice * (_context.Discounts
                .Where(d => d.idProduct == p.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault() / 100m)),
                    DiscountPercent = _context.Discounts
                .Where(d => d.idProduct == p.idProduct)
                .Select(d => d.discountPercent)
                .FirstOrDefault()
                })
                .ToList();

            // Lấy danh sách sản phẩm đã lưu của khách hàng hiện tại
            var savedProductIds = _context.SaveProducts
                .Where(s => s.idCustomer == customerId)
                .Select(s => s.idProduct)
                .ToList();

            // Gán danh sách ID sản phẩm đã lưu vào ViewBag để sử dụng trong View
            ViewBag.SavedProductIds = savedProductIds;

            var topSellingProducts = _context.Orders
                  .Where(o => o.dateFrom.Date >= startDate && o.dateFrom.Date <= endDate)
                  .SelectMany(o => o.DetailOrders)
                  .GroupBy(d => new { d.idProduct, d.Product.nameProduct, d.Product.sellPrice, d.Product.Image })
                  .Select(g => new TopSellingProductDto
                  {
                      ProductId = g.Key.idProduct,
                      ProductName = g.Key.nameProduct,
                      Price = g.Key.sellPrice,
                      DiscountedPrice = g.Key.sellPrice - (g.Key.sellPrice * (_context.Discounts
                  .Where(d => d.idProduct == g.Key.idProduct)
                  .Select(d => d.discountPercent)
                  .FirstOrDefault() / 100m)),
                      Sold = g.Sum(d => d.Quantity),
                      Revenue = g.Sum(d => d.Product.sellPrice * d.Quantity),
                      ImageUrl = g.Key.Image,
                      DiscountPercent = _context.Discounts
                  .Where(d => d.idProduct == g.Key.idProduct)
                  .Select(d => d.discountPercent)
                  .FirstOrDefault()
                  })
                  .OrderByDescending(p => p.Sold)
                  .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                  .ToList();

            // Gán danh sách sản phẩm bán chạy nhất vào ViewBag để sử dụng trong View
            ViewBag.TopSellingProducts = topSellingProducts;


            // Trả về Partial View với danh sách sản phẩm
            return PartialView("_ProductByCategory", products);
        }


        [HttpGet]
        public IActionResult FilterProducts(string categoryId, string branchId, string petId, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 3)
        {
            var customerId = GetCustomerIdFromSession();

            // Khởi tạo truy vấn với thông tin liên kết cần thiết
            var productsQuery = _context.Products
                .Include(sp => sp.Branch)
                .Include(sp => sp.Category)
                .Include(sp => sp.Pet)
                .AsQueryable();

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(categoryId))
            {
                productsQuery = productsQuery.Where(p => p.idCategory == categoryId);
            }

            if (!string.IsNullOrEmpty(branchId))
            {
                productsQuery = productsQuery.Where(p => p.idBranch == branchId);
            }

            if (!string.IsNullOrEmpty(petId))
            {
                productsQuery = productsQuery.Where(p => p.idPet == petId);
            }

            // Lọc theo giá
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.sellPrice >= minPrice && p.sellPrice <= maxPrice);
            }
            else if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.sellPrice >= minPrice);
            }
            else if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.sellPrice <= maxPrice);
            }

            // Tính toán tổng số sản phẩm và số trang
            var totalProductsCount = productsQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalProductsCount / pageSize);

            // Lấy sản phẩm cho trang hiện tại
            var products = productsQuery
                 .Select(sp => new ProductViewDto
                 {
                     idProduct = sp.idProduct,
                     nameProduct = sp.nameProduct,
                     sellPrice = sp.sellPrice,
                     Image = sp.Image,
                     idBranch = sp.idBranch,
                     idCategori = sp.idCategory,
                     nameBranch = sp.Branch.nameBranch,
                     nameCategory = sp.Category.nameCategory,
                     Quantity = sp.Quantity,
                     Description = sp.Description,
                     Logo = sp.Branch.Logo,
                     // Tính giá chiết khấu
                     DiscountedPrice = sp.sellPrice - (sp.sellPrice * (_context.Discounts
                        .Where(d => d.idProduct == sp.idProduct)
                        .Select(d => d.discountPercent)
                        .FirstOrDefault() / 100m)),
                     // Lấy phần trăm giảm giá
                     DiscountPercent = _context.Discounts
                        .Where(d => d.idProduct == sp.idProduct)
                        .Select(d => d.discountPercent)
                        .FirstOrDefault()
                 })
                .OrderBy(p => p.idProduct)
                .Skip((page - 1) * pageSize) // Bỏ qua các sản phẩm của các trang trước
                .Take(pageSize) // Lấy sản phẩm của trang hiện tại
                .ToList();

            // Lưu thông tin vào ViewBag
            var savedProductIds = _context.SaveProducts
                .Where(s => s.idCustomer == customerId)
                .Select(s => s.idProduct)
                .ToList();

            ViewBag.SavedProductIds = savedProductIds;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return PartialView("FilterProducts", products);
        }





        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public IActionResult SearchResults(string query)
        {
            DateTime startDate = DateTime.Today.AddDays(-30);

            DateTime endDate = DateTime.Today;
            // Chuẩn hóa từ khóa tìm kiếm
            string normalizedQuery = RemoveDiacritics(query.ToLower());

            // Lấy tất cả các sản phẩm trước, sau đó xử lý tìm kiếm trong bộ nhớ
            var products = _context.Products
         .Include(p => p.Branch)
         .Include(p => p.Category)
         .Select(p => new
         {
             Product = p,
             DiscountPercent = _context.Discounts
                 .Where(d => d.idProduct == p.idProduct)
                 .Select(d => d.discountPercent)
                 .FirstOrDefault()
         })
         .AsEnumerable() // Chuyển đổi sang client-side evaluation sau khi lấy dữ liệu cần thiết
         .Where(p => RemoveDiacritics(p.Product.nameProduct.ToLower()).Contains(normalizedQuery))
         .Select(p => new ProductViewDto
         {
             idProduct = p.Product.idProduct,
             nameProduct = p.Product.nameProduct,
             sellPrice = p.Product.sellPrice,
             Image = p.Product.Image,
             idBranch = p.Product.idBranch,
             idCategori = p.Product.idCategory,
             nameBranch = p.Product.Branch.nameBranch,
             nameCategory = p.Product.Category.nameCategory,
             Quantity = p.Product.Quantity,
             Description = p.Product.Description,
             Logo = p.Product.Branch.Logo,
             DiscountedPrice = p.Product.sellPrice - (p.Product.sellPrice * (p.DiscountPercent / 100m)),
             DiscountPercent = p.DiscountPercent
         })
         .ToList();
            var topSellingProducts = _context.Orders
                 .Where(o => o.dateFrom.Date >= startDate && o.dateFrom.Date <= endDate)
                 .SelectMany(o => o.DetailOrders)
                 .GroupBy(d => new { d.idProduct, d.Product.nameProduct, d.Product.sellPrice, d.Product.Image })
                 .Select(g => new TopSellingProductDto
                 {
                     ProductId = g.Key.idProduct,
                     ProductName = g.Key.nameProduct,
                     Price = g.Key.sellPrice,
                     DiscountedPrice = g.Key.sellPrice - (g.Key.sellPrice * (_context.Discounts
                 .Where(d => d.idProduct == g.Key.idProduct)
                 .Select(d => d.discountPercent)
                 .FirstOrDefault() / 100m)),
                     Sold = g.Sum(d => d.Quantity),
                     Revenue = g.Sum(d => d.Product.sellPrice * d.Quantity),
                     ImageUrl = g.Key.Image,
                     DiscountPercent = _context.Discounts
                 .Where(d => d.idProduct == g.Key.idProduct)
                 .Select(d => d.discountPercent)
                 .FirstOrDefault()
                 })
                 .OrderByDescending(p => p.Sold)
                 .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                 .ToList();

            // Gán danh sách sản phẩm bán chạy nhất vào ViewBag để sử dụng trong View
            ViewBag.TopSellingProducts = topSellingProducts;

            // Trả về View với danh sách sản phẩm tìm kiếm
            return View(products);
        }


        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Create()
        {
            // Có thể truyền danh sách chi nhánh, danh mục, màu sắc, v.v. vào ViewBag nếu cần
            ViewBag.Branchs = _context.Branchs.Select(b => new SelectListItem
            {
                Value = b.idBranch,
                Text = b.nameBranch
            }).ToList();

            ViewBag.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.idCategory,
                Text = c.nameCategory
            }).ToList();

            ViewBag.Pets = _context.Pets.Select(p => new SelectListItem
            {
                Value = p.idPet,
                Text = p.namePet
            }).ToList();

            return View();
        }
        // POST: Product/Create
        [HttpPost]
        public IActionResult Create(ProductCreateDto productDto)
        {
            ViewBag.Branchs = _context.Branchs.Select(b => new SelectListItem
            {
                Value = b.idBranch,
                Text = b.nameBranch
            }).ToList();

            ViewBag.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.idCategory,
                Text = c.nameCategory
            }).ToList();


            ViewBag.Pets = _context.Pets.Select(p => new SelectListItem
            {
                Value = p.idPet,
                Text = p.namePet
            }).ToList();
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.idProduct == productDto.idProduct);
                if (existingProduct != null)
                {
                    ModelState.AddModelError("idProduct", $"Product with ID '{productDto.idProduct}' already exists.");
                    return View(productDto); // Trả lại form với thông báo lỗi
                }
                string ImageFilePath = null;
                if (productDto.Image != null && productDto.Image.Length > 0)
                {
                    var ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productDto.Image.FileName);
                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                       productDto.Image.CopyTo(stream);
                    }
                    ImageFilePath = productDto.Image.FileName; // Cập nhật tên tệp Image
                }
                var product = new Product
                {
                    idProduct = productDto.idProduct,
                    nameProduct = productDto.nameProduct,
                    sellPrice = productDto.sellPrice,
                    idBranch = productDto.idBranch,
                    idCategory = productDto.idCategory,
                    idPet = productDto.idPet,
                    Quantity = productDto.Quantity,
                    Image = ImageFilePath,
                    Description = productDto.Description
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách sản phẩm
            }

            return View(productDto);
        }
        [Authorize(Roles = "Admin,StaffProduct")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = _context.Products.FirstOrDefault(p => p.idProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                idProduct = product.idProduct,
                nameProduct = product.nameProduct,
                sellPrice = product.sellPrice,
                idBranch = product.idBranch,
                idCategory = product.idCategory,
                idPet = product.idPet,
                Quantity = product.Quantity,
                Description = product.Description
            };

            // Truyền danh sách chi nhánh, danh mục, màu sắc vào ViewBag
            ViewBag.Branchs = _context.Branchs.Select(b => new SelectListItem
            {
                Value = b.idBranch,
                Text = b.nameBranch
            }).ToList();

            ViewBag.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.idCategory,
                Text = c.nameCategory
            }).ToList();

         

            ViewBag.Pets = _context.Pets.Select(p => new SelectListItem
            {
                Value = p.idPet,
                Text = p.namePet
            }).ToList();

            return View(productDto);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductDto productDto)
        {
            ViewBag.Branchs = _context.Branchs.Select(b => new SelectListItem
            {
                Value = b.idBranch,
                Text = b.nameBranch
            }).ToList();

            ViewBag.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.idCategory,
                Text = c.nameCategory
            }).ToList();
            ViewBag.Pets = _context.Pets.Select(p => new SelectListItem
            {
                Value = p.idPet,
                Text = p.namePet
            }).ToList();
            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(p => p.idProduct == productDto.idProduct);
                if (product == null)
                {
                    return NotFound();
                }
               
                // Cập nhật các giá trị từ DTO vào model
                product.nameProduct = productDto.nameProduct;
                product.sellPrice = productDto.sellPrice;
                product.idBranch = productDto.idBranch;
                product.idCategory = productDto.idCategory;
                product.idPet = productDto.idPet;
                product.Quantity = productDto.Quantity;
                product.Description = productDto.Description;
                if (productDto.Image != null && productDto.Image.Length > 0)
                {
                    var cvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productDto.Image.FileName);
                    if (System.IO.File.Exists(cvPath))
                    {
                        System.IO.File.Delete(cvPath);
                    }

                    using (var stream = new FileStream(cvPath, FileMode.Create, FileAccess.Write))
                    {
                        productDto.Image.CopyTo(stream);
                    }
                    product.Image = productDto.Image.FileName;
                }

                _context.SaveChanges();
                return RedirectToAction("Index"); // Quay lại trang danh sách sản phẩm sau khi cập nhật
            }

            return View(productDto);
        }
       
        // POST: Product/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StaffProduct")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách sản phẩm sau khi xóa
        }
      

        [HttpGet]
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // Trả về lỗi 404 nếu idProduct không tồn tại
            }
            var customerid = GetCustomerIdFromSession();
            // Tìm sản phẩm dựa trên idProduct
            var product = _context.Products
                .Include(sp => sp.Branch)           // Baogồm thông tin thương hiệu (Branch)
                .Include(sp => sp.Category)     
                .Include(sp => sp.Pet)         // Bao gồm thông tin loại (Category)
                                                    // Bao gồm thông tin loại (Category)
                .Include(sp => sp.ImageProducts)    // Bao gồm thông tin các hình ảnh sản phẩm
              .Include(sp => sp.ProductColors)      // Bao gồm ProductColors
        .ThenInclude(pc => pc.Color)      // Bao gồm thông tin Color
    .Include(sp => sp.ProductSizes)       // Bao gồm ProductSizes
        .ThenInclude(ps => ps.Size)         // Bao gồm thông tin kích thước
                .Include(sp => sp.Discounts)        // Bao gồm thông tin giảm giá
                .Where(p => p.idProduct == id)
                .Select(p => new ProductViewDto
                {
                    idProduct = p.idProduct,
                    nameProduct = p.nameProduct,
                    sellPrice = p.sellPrice,
                    Image = p.Image, // Hình ảnh chính của sản phẩm
                    idBranch = p.idBranch,
                    idCategori = p.idCategory,
                    idPet = p.idPet,
                    nameBranch = p.Branch != null ? p.Branch.nameBranch : "N/A", // Tên thương hiệu, nếu có
                    namePet = p.Pet != null ? p.Pet.namePet : "N/A", // Tên thương hiệu, nếu có
                    nameCategory = p.Category != null ? p.Category.nameCategory : "N/A", // Tên loại, nếu có
                    Quantity = p.Quantity,
                    Description = p.Description,
                    Colors = p.ProductColors != null ? p.ProductColors.Select(pc => pc.Color.nameColor).ToList() : new List<string>(), // Lấy danh sách màu sắc
                    Sizes = p.ProductSizes != null ? p.ProductSizes.Select(ps => ps.Size.nameSize).ToList() : new List<string>(),      // Lấy danh sách kích thước
                    Logo = p.Branch != null ? p.Branch.Logo : null, // Logo của chi nhánh
                    Discounts = p.Discounts != null ? p.Discounts.Select(s => s.discountPercent).ToList() : new List<int>(), // Phần trăm giảm giá
                    ImageProducts = p.ImageProducts != null ? p.ImageProducts.Select(i => i.Image).ToList() : new List<string>() // Hình ảnh sản phẩm khác
                })
                .FirstOrDefault();



            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }
            int maxDiscountPercent = product.Discounts.Any() ? product.Discounts.Max() : 0;
            decimal discountedPrice = product.sellPrice * (1 - maxDiscountPercent / 100m);

            // Truyền giá sau khi giảm vào View thông qua ViewData hoặc thêm vào DTO nếu cần
            ViewData["DiscountedPrice"] = discountedPrice;


            // Trả về view với thông tin chi tiết của sản phẩm
            return View(product);
        }
        private int GetCustomerIdFromSession()
        {
            var customerEmail = HttpContext.Session.GetString("email");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
            return customer?.idCustomer ?? 0;
        }
        private string GenerateOrderId()
        {
            Random random = new Random();
            int number = random.Next(10000, 99999); // Tạo ra một số ngẫu nhiên từ 10000 đến 99999
            return "O" + number.ToString(); // Kết hợp với "O" để tạo ra idOrder
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddToCart([FromBody] AddToCartViewDto model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) // Kiểm tra nếu model là null hoặc không hợp lệ
                {
                    return Json(new { success = false, message = "Invalid data." });
                }

                int idCustomer = GetCustomerIdFromSession();

                // Kiểm tra nếu idCustomer bằng 0, chuyển đến trang đăng nhập
                if (idCustomer == 0)
                {
                    return Json(new { success = false, redirectUrl = Url.Action("Login", "User") });
                }

                var product = _context.Products
                    .Include(p => p.Discounts) // Bao gồm Discounts để truy xuất chiết khấu
                    .FirstOrDefault(p => p.idProduct == model.ProductId);

                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                // Lấy chiết khấu (nếu có) từ bảng Discounts
                decimal discountPercent = product.Discounts?.FirstOrDefault()?.discountPercent ?? 0;
                decimal discountedPrice = product.sellPrice - (product.sellPrice * discountPercent / 100);

                // Kiểm tra đơn hàng hiện tại
                var order = _context.Orders.FirstOrDefault(o => o.idCustomer == idCustomer && o.statusOrder == OrderStatus.InCart);

                // Nếu không có đơn hàng hiện tại, tạo đơn hàng mới
                if (order == null)
                {
                    order = new Order
                    {
                        idOrder = GenerateOrderId(), // Gán idOrder với hàm GenerateOrderId
                        idCustomer = idCustomer,
                        statusOrder = OrderStatus.InCart
                    };
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                }

                // Kiểm tra xem sản phẩm đã có trong đơn hàng chưa với cùng idProduct
                var existingOrderDetail = _context.DetailOrders.FirstOrDefault(od =>
                    od.idOrder == order.idOrder &&
                    od.idProduct == model.ProductId &&
                    od.nameColor == model.Color &&
                    od.nameSize == model.Size);

                if (existingOrderDetail != null)
                {
                    // Nếu sản phẩm đã tồn tại với màu sắc và kích thước giống nhau, cập nhật số lượng
                    existingOrderDetail.Quantity += model.Quantity;
                    existingOrderDetail.totalPrice = existingOrderDetail.Quantity * discountedPrice; // Cập nhật tổng giá với giá sau chiết khấu
                    _context.DetailOrders.Update(existingOrderDetail);
                }
                else
                {
                    // Nếu sản phẩm hoàn toàn mới (không tồn tại trong đơn hàng hiện tại), tạo chi tiết đơn hàng mới
                    var newOrderDetail = new DetailOrder
                    {
                        idOrder = order.idOrder,
                        idProduct = model.ProductId,
                        nameColor = model.Color,
                        nameSize = model.Size,
                        Quantity = model.Quantity,
                        totalPrice = model.Quantity * discountedPrice // Tính tổng giá với giá sau chiết khấu
                    };

                    _context.DetailOrders.Add(newOrderDetail);
                }

                TempData["success"] = "Thêm giỏ hàng thành công";
                _context.SaveChanges(); // Lưu các thay đổi vào database

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception.";
                return Json(new { success = false, message = ex.Message + " Inner Exception: " + innerException });
            }
        }


        public IActionResult ViewCart()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            var order = _context.Orders
                .Include(o => o.DetailOrders)
                
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.idCustomer == idCustomer && o.statusOrder == OrderStatus.InCart);

            // Kiểm tra nếu đơn hàng hoặc DetailOrders là null hoặc DetailOrders không có sản phẩm nào
            if (order == null || order.DetailOrders == null || !order.DetailOrders.Any())
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                return View();
            }
            var customer = _context.Customers.Include(c => c.City)
                                             .Include(c => c.Country)
                                             .Include(c => c.District)
                                             .Include(c => c.Ward).FirstOrDefault(c => c.idCustomer == idCustomer);
            if (customer != null)
            {
                ViewBag.CustomerAddress = $"{customer.Address}/{customer.Ward.nameWard}/{customer.District.nameDistrict}/{customer.City.nameCity}";
            }
            else
            {
                ViewBag.CustomerAddress = string.Empty; // Để trống nếu không tìm thấy
            }
            // Tính toán tổng đơn hàng
            order.CalculateTotalOrder(); // Gọi phương thức tính tổng đơn hàng

            return View(order); // Truyền đơn hàng có trạng thái "InCart" vào View
        }
        public IActionResult MyOrderPending()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer && t.Order.statusOrder == OrderStatus.Pending)  // Thêm điều kiện lọc trạng thái đơn hàng
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                ViewBag.Message = "Bạn chưa có giao dịch nào.";
                return View();
            }

            return View(transactions);
        }
        public IActionResult MyOrderAccept()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer && t.Order.statusOrder == OrderStatus.Accepted)  // Thêm điều kiện lọc trạng thái đơn hàng
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                ViewBag.Message = "Bạn chưa có giao dịch nào.";
                return View();
            }

            return View(transactions);
        }
        public IActionResult MyOrderComplete()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer && t.Order.statusOrder == OrderStatus.Complete)  // Thêm điều kiện lọc trạng thái đơn hàng
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                ViewBag.Message = "Bạn chưa có giao dịch nào.";
                return View();
            }

            return View(transactions);
        }
        public IActionResult MyOrderReject()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer && t.Order.statusOrder == OrderStatus.Rejected)  // Thêm điều kiện lọc trạng thái đơn hàng
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                ViewBag.Message = "Bạn chưa có giao dịch nào.";
                return View();
            }

            return View(transactions);
        }
        public IActionResult MyOrderRefun()
        {
            int idCustomer = GetCustomerIdFromSession();
            if (idCustomer == 0)
            {
                TempData["error"] = "Hãy đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            // Lấy tất cả các giao dịch của khách hàng cùng với đơn hàng tương ứng
            var transactions = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(t => t.idCustomer == idCustomer && t.Order.statusPay == PaymentStatus.Refunded)  // Thêm điều kiện lọc trạng thái đơn hàng
                .ToList();

            // Kiểm tra nếu không có giao dịch nào
            if (transactions == null || !transactions.Any())
            {
                ViewBag.Message = "Bạn chưa có giao dịch nào.";
                return View();
            }

            return View(transactions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transaction(
     string name, string address, string phone, string email, string note,
     List<int> selectedDetailIds, string paymentType)
        {
            int idCustomer = GetCustomerIdFromSession();

            // Lấy giỏ hàng hiện tại
            var currentCart = _context.Orders
                .Include(o => o.DetailOrders)
                .FirstOrDefault(o => o.idCustomer == idCustomer && o.statusOrder == OrderStatus.InCart);

            if (currentCart == null)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("ViewCart");
            }

            // Tính tổng số tiền dựa trên các DetailOrder đã chọn
            decimal totalAmount = currentCart.DetailOrders
                .Where(d => selectedDetailIds.Contains(d.IdDetailOrder))
                .Sum(d => d.totalPrice ?? 0);

            // Tạo đơn hàng mới
            var newOrder = new Order
            {
                idOrder = GenerateOrderId(),
                idCustomer = idCustomer,
                statusOrder = OrderStatus.Pending,
                totalOrder = totalAmount
            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            // Tạo một transaction từ dữ liệu người dùng đã nhập
            var transaction = new Transaction
            {
                nameCustomer = name,
                shippingAddress = address,
                phoneNumber = phone,
                Email = email,
                TotalAmount = totalAmount,
                createdDate = DateTime.Now,
                cpdatedDate = DateTime.Now,
                Note = note,
                idCustomer = idCustomer,
                idOrder = newOrder.idOrder
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            // Lọc các DetailOrder trong giỏ hàng mà người dùng đã chọn
            var selectedDetails = currentCart.DetailOrders
                .Where(d => selectedDetailIds.Contains(d.IdDetailOrder))
                .ToList();

            foreach (var detail in selectedDetails)
            {
                var newOrderDetail = new DetailOrder
                {
                    idOrder = newOrder.idOrder,
                    idProduct = detail.idProduct,
                    Quantity = detail.Quantity,
                    nameColor = detail.nameColor,
                    nameSize = detail.nameSize,
                    totalPrice = detail.totalPrice
                };
                _context.DetailOrders.Add(newOrderDetail);
            }

            // Xóa các DetailOrder đã chuyển đi
            _context.DetailOrders.RemoveRange(selectedDetails);
            _context.SaveChanges();

            // Thêm thông tin thanh toán cho giao dịch
            if (paymentType == "immediate")
            {
                var payment = new Payment
                {
                    idTransaction = transaction.idTransaction,
                    Amount = totalAmount,
                    CreatedDate = DateTime.Now,
                    QRCodeUrl = GenerateQRCodeUrl(transaction.idOrder, totalAmount, transaction.nameCustomer)
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();

                TempData["success"] = "Đặt hàng thành công!";
                return RedirectToAction("ShowQRCode", new { paymentId = payment.Id });
            }
            else if (paymentType == "later")
            {
                var payment = new Payment
                {
                    idTransaction = transaction.idTransaction,
                    Amount = totalAmount,
                    CreatedDate = DateTime.Now,
                    QRCodeUrl = GenerateQRCodeUrl(transaction.idOrder, totalAmount, transaction.nameCustomer)
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();
                TempData["success"] = "Đặt hàng thành công! Bạn có thể thanh toán sau.";
                return RedirectToAction("MyOrderPending");
            }

            return RedirectToAction("ViewCart");
        }
        [HttpPost]
        public IActionResult CheckOrCreatePayment(int idTransaction)
        {
            var transaction = _context.Transactions
                .Include(t => t.Order)
                .FirstOrDefault(t => t.idTransaction == idTransaction);

            if (transaction == null)
            {
                return Json(new { success = false, message = "Giao dịch không tồn tại." });
            }

            // Kiểm tra xem đã có payment nào cho giao dịch này chưa
            var existingPayment = _context.Payments.FirstOrDefault(p => p.idTransaction == idTransaction);

            if (existingPayment != null)
            {
                // Nếu đã có payment, trả về paymentId để chuyển hướng tới ShowQRCode
                return Json(new { success = true, paymentId = existingPayment.Id });
            }

            // Nếu chưa có payment, tạo payment mới
            var payment = new Payment
            {
                idTransaction = transaction.idTransaction,
                Amount = transaction.TotalAmount ?? 0,
                CreatedDate = DateTime.Now,
                QRCodeUrl = GenerateQRCodeUrl(transaction.Order.idOrder, transaction.TotalAmount ?? 0, transaction.nameCustomer)
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return Json(new { success = true, paymentId = payment.Id });
        }


        private string GenerateQRCodeUrl(string idorder, decimal amount, string customerName)
        {
            // Thông tin tài khoản ngân hàng
            string bankCode = "MB"; // Thay bằng mã ngân hàng của bạn
            string accountNumber = "0347854097"; // Thay bằng số tài khoản của bạn

            // Tạo mô tả giao dịch bao gồm ID đơn hàng và tên khách hàng
            string transactionDescription = $"{customerName} thanh toán đơn hàng {idorder} ";

            // Tạo URL QR code tích hợp tài khoản ngân hàng và thông tin thanh toán
            string qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-qr_only.png?amount={amount}&addInfo={Uri.EscapeDataString(transactionDescription)}";

            return qrUrl;
        }


        public IActionResult ShowQRCode(int paymentId)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.Id == paymentId);

            if (payment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thanh toán.";
                return RedirectToAction("ViewCart");
            }

            // Tải dữ liệu giao dịch liên quan nếu cần
            var transaction = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(t => t.DetailOrders)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(t => t.idTransaction == payment.idTransaction);
            if (transaction != null)
            {
                ViewBag.Transaction = transaction;
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin giao dịch.";
                return RedirectToAction("ViewCart");
            }

            return View(payment);
        }

        [HttpPost]
        public IActionResult PaymentNotification(PaymentNotificationDto notification)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.idTransaction == notification.TransactionId);
            // Tải dữ liệu giao dịch liên quan nếu cần
            var transaction = _context.Transactions
                .Include(t => t.Order)
                .ThenInclude(t => t.DetailOrders)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(t => t.idTransaction == payment.idTransaction);

            if (payment != null && notification.IsSuccess)
            {
                TempData["success"] = "Thanh toán thành công!";
                transaction.Order.statusPay = PaymentStatus.Paid;
                _context.SaveChanges();

                // Thông báo cho người dùng
              
            }

            return Ok(); // Trả về phản hồi cho dịch vụ thanh toán
        }

        private void NotifyUser(Payment payment)
        {
            // Gửi thông báo cho người dùng về việc thanh toán thành công
            // Ví dụ: gửi email hoặc tin nhắn
        }





        [HttpPost]
        public IActionResult DeleteDetailOrder(int id)
        {
            // Kiểm tra id không hợp lệ
            if (id <= 0)
            {
                return BadRequest("ID không hợp lệ.");
            }

            // Tìm DetailOrder trong cơ sở dữ liệu
            var detail = _context.DetailOrders.Find(id);
            if (detail == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            try
            {
                // Lấy idOrder từ DetailOrder trước khi xóa
                var orderId = detail.idOrder;

                // Xóa sản phẩm khỏi giỏ hàng
                _context.DetailOrders.Remove(detail);
                _context.SaveChanges();

                // Kiểm tra xem Order có còn DetailOrder nào không
                var remainingDetails = _context.DetailOrders
                    .Any(d => d.idOrder == orderId);

                if (!remainingDetails)
                {
                    // Nếu không còn DetailOrder nào, xóa Order và Transaction liên quan
                    var order = _context.Orders.FirstOrDefault(o => o.idOrder == orderId);
                    if (order != null)
                    {
                        // Tìm Transaction liên quan đến Order này
                        var transaction = _context.Transactions
                            .FirstOrDefault(t => t.idOrder == orderId);

                        if (transaction != null)
                        {
                            _context.Transactions.Remove(transaction);
                        }

                        _context.Orders.Remove(order);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    // Nếu vẫn còn DetailOrder, cập nhật lại totalOrder cho Order và Transaction
                    decimal newTotalOrder = _context.DetailOrders
                        .Where(d => d.idOrder == orderId)
                        .Sum(d => d.totalPrice ?? 0);

                    var order = _context.Orders.FirstOrDefault(o => o.idOrder == orderId);
                    if (order != null)
                    {
                        order.totalOrder = newTotalOrder;
                    }

                    var transaction = _context.Transactions
                        .FirstOrDefault(t => t.idOrder == orderId);
                    if (transaction != null)
                    {
                        transaction.TotalAmount = newTotalOrder;
                    }
                    TempData["success"] = "Xóa sản phẩm thành công.";
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi khi xóa
                return StatusCode(500, $"Đã xảy ra lỗi khi xóa sản phẩm: {ex.Message}");
            }

            // Trả về thông báo thành công
            return Ok("Sản phẩm đã được xóa thành công và cập nhật đơn hàng.");
        }





        public IActionResult SavedProduct()
        {
            // Lấy customerId từ session
            var customerId = GetCustomerIdFromSession();

            // Kiểm tra xem customerId có hợp lệ không
            if (customerId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy các sản phẩm đã lưu cho customerId từ cơ sở dữ liệu
            var saveProducts = _context.SaveProducts
                .Where(s => s.idCustomer == customerId)
                .Include(s => s.Product)
                .ThenInclude(p => p.Discounts)
                .ToList();

            if (!saveProducts.Any())
            {
                ViewBag.Message = "Bạn chưa lưu sản phẩm nào.";
                return View(new List<SaveProductDto>());
            }

            // Chuyển đổi các sản phẩm đã lưu thành SaveProductDto và tính giá chiết khấu
            var saveProductDtos = saveProducts.Select(saveProduct =>
            {
                var product = saveProduct.Product;

                // Lấy chiết khấu đầu tiên nếu có
                decimal discountPercent = product.Discounts?.FirstOrDefault()?.discountPercent ?? 0;

                // Tính giá sau chiết khấu
                decimal discountedPrice = product.sellPrice - (product.sellPrice * discountPercent / 100);

                // Trả về DTO chứa thông tin sản phẩm và giá đã chiết khấu
                return new SaveProductDto
                {
                    idProduct = product.idProduct,
                    nameProduct = product.nameProduct,
                    SellPrice = product.sellPrice,
                    DiscountedPrice = discountedPrice,
                    Image = product.Image, // Cập nhật đường dẫn hình ảnh
                    Quantity = product.Quantity

                };
            }).ToList();

            return View(saveProductDtos);
        }



        [HttpPost]
        public async Task<IActionResult> SaveProduct(string idproduct)
        {
            try
            {
                int customerid = GetCustomerIdFromSession();


                if (customerid == 0)
                {
                    return Json(new { success = false, message = "Please log in first.", redirectToLogin = true });
                }

                var existingSaveProduct = await _context.SaveProducts
                    .FirstOrDefaultAsync(s => s.idProduct == idproduct && s.idCustomer == customerid);

                if (existingSaveProduct != null)
                {
                    _context.SaveProducts.Remove(existingSaveProduct);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product has been removed from saved products." });
                }
                else
                {
                    var saveproduct = new SaveProduct
                    {
                        idProduct = idproduct,
                        idCustomer = customerid,
                        SavedAt = DateTime.Now
                    };

                    _context.SaveProducts.Add(saveproduct);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product has been saved successfully!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while saving the product." });
            }
        }

        [HttpPost]
        public IActionResult DeleteSaveProduct(string productid)
        {
            var customerId = GetCustomerIdFromSession(); // Giả sử có hàm để lấy customer hiện tại

            if (customerId == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            var saveProduct = _context.SaveProducts
                .FirstOrDefault(sj => sj.idProduct == productid && sj.idCustomer == customerId);

            if (saveProduct != null)
            {
                _context.SaveProducts.Remove(saveProduct);
                _context.SaveChanges();
                TempData["success"] = "Product has been deleted successfully!";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Saved product not found!" });
        }





    }
}
