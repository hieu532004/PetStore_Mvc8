using Microsoft.EntityFrameworkCore;
using WebThuCung.Models;

namespace WebThuCung.Data
{
    public class PetContext : DbContext
    {
        public PetContext(DbContextOptions<PetContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<VoteWarehouse> VoteWarehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DetailOrder> DetailOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DetailVoteWarehouse> DetailVoteWarehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<SaveProduct> SaveProducts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<ShipperOrder> ShipperOrders { get; set; }



    }
}
