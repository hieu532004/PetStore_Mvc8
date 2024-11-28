using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Models;

namespace WebThuCung.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly PetContext _petContext;

        public CategoryViewComponent(PetContext petContext)
        {
            _petContext = petContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách categories từ cơ sở dữ liệu
            var categories = await _petContext.Categories.ToListAsync();

            // Truyền danh sách categories đến view RenderCategory
            return View("RenderCategory", categories);
        }
    }
}
