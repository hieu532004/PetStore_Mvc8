using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;

namespace WebThuCung.ViewComponents
{
    public class PetViewComponent : ViewComponent
    {
        private readonly PetContext _petContext;

        public PetViewComponent(PetContext petContext)
        {
            _petContext = petContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách categories từ cơ sở dữ liệu
            var pets = await _petContext.Pets.ToListAsync();

            // Truyền danh sách categories đến view RenderCategory
            return View("RenderPet", pets);
        }
    }
}
