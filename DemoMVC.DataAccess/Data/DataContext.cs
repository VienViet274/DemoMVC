using DemoMVC.Models;
using DemoMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Data
{
    public class DataContext:IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData
                (
                new Category { ID = 1,Name="Viet",NamSinh=2000 },
                new Category { ID = 2,Name="Nam",NamSinh=2002 },
                new Category { ID=3,Name="Hung",NamSinh=1999}
                ) ;
            modelBuilder.Entity<Company>().HasData
                (
                new Company { ID = 1, Name = "Tech Solution", StreetAddress = "123 Tech St", City = "Tech City", PostalCode = "12121", State = "IL", PhoneNumber = "6669990000" },
                new Company { ID = 2, Name = "Vivid Books", StreetAddress = "999 Vid St", City = "Vid City", PostalCode = "66666", State = "IL", PhoneNumber = "7779990000" },
                new Company { ID = 3, Name = "Readers Club", StreetAddress = "999 Main St", City = "Lala land", PostalCode = "99999", State = "NY", PhoneNumber = "1113335555" }
                );
     
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
