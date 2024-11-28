using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;

namespace WebThuCung.ViewComponents
{
    public class CategoryShopViewComponent : ViewComponent
    {

        private readonly PetContext _petContext;

        public CategoryShopViewComponent(PetContext petContext)
        {
            _petContext = petContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách categories từ cơ sở dữ liệu
            var categories = await _petContext.Categories.ToListAsync();

            // Truyền danh sách categories đến view RenderCategory
            return View("RenderCategoryShop", categories);
        }
    }
}
