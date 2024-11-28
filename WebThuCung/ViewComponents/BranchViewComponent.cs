using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;

namespace WebThuCung.ViewComponents
{
    public class BranchViewComponent : ViewComponent
    {

        private readonly PetContext _petContext;

        public BranchViewComponent(PetContext petContext)
        {
            _petContext = petContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách categories từ cơ sở dữ liệu
            var branchs = await _petContext.Branchs.ToListAsync();

            // Truyền danh sách categories đến view RenderCategory
            return View("RenderBranch", branchs);
        }
    }
}
