using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebThuCung.Data;
using WebThuCung.Models;

namespace WebThuCung.Controllers
{
    public class BaseController : Controller
    {
        private readonly PetContext _context;
        public BaseController(PetContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // Thực hiện các tác vụ trước khi hành động controller được gọi
            int customerId = GetCustomerIdFromSession(); // Lấy ID khách hàng từ session hoặc JWT
            int cartCount = _context.DetailOrders
                .Include(d => d.Order)
                .Count(d => d.Order.idCustomer == customerId && d.Order.statusOrder == OrderStatus.InCart);

            ViewBag.CartCount = cartCount; // Gán giá trị vào ViewBag

            base.OnActionExecuting(context);
        }
        private int GetCustomerIdFromSession()
        {
            var customerEmail = HttpContext.Session.GetString("email");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
            return customer?.idCustomer ?? 0;
        }
    }

}
