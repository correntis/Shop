using Microsoft.EntityFrameworkCore;
using Shop.Core;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Security.Authentication.Web.Core;

namespace Shop.DB
{
    public class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterValue> CharacterValues { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UsersAccounts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserProduct> UsersProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrdersItems { get; set; }
        public DbSet<Request> Requests { get; set; }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminAccount> AdminsAccounts { get; set; }
        public DbSet<AdminProduct> AdminsProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=CORRENTI;Database=Shop;Trusted_Connection=True;Encrypt=False");

            base.OnConfiguring(optionsBuilder);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity(j => j.ToTable("ProductsCategories"));

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Categories)
                .AutoInclude();

            modelBuilder.Entity<Product>()
                .Navigation(p => p.CharacterValues)
                .AutoInclude();

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Reviews)
                .AutoInclude();


            modelBuilder.Entity<Character>()
                .HasMany(c => c.Categories)
                .WithMany(c => c.Characters)
                .UsingEntity(j => j.ToTable("CategoriesCharacters"));


            modelBuilder.Entity<Category>()
                .Navigation(p => p.Characters)
                .AutoInclude();


            modelBuilder.Entity<CharacterValue>()
                .HasOne(cv => cv.Character)
                .WithMany(c => c.CharacterValues)
                .HasForeignKey(cv => cv.CharactersId);


            //////////////////////////////////////
            //////////////////////////////////////
            modelBuilder.Entity<CharacterValue>()
                .HasMany(cv => cv.Products)
                .WithMany(p => p.CharacterValues)
                .UsingEntity(j => j.ToTable("ProductsCharsValues"));


            modelBuilder.Entity<UserAccount>()
                .HasOne(ua => ua.User)
                .WithOne(u => u.UserAccount)
                .HasForeignKey<UserAccount>(ua => ua.UsersId);

            modelBuilder.Entity<UserAccount>()
                .Navigation(ua => ua.User)
                .AutoInclude();


            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UsersId);

            modelBuilder.Entity<Review>()
                .Navigation(r => r.User)
                .AutoInclude();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductsId);



            modelBuilder.Entity<UserProduct>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProducts)
                .HasForeignKey(up => up.UsersId);

            modelBuilder.Entity<UserProduct>()
                .HasOne(up => up.Product)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(up => up.ProductsId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .Navigation(u => u.UserProducts)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .HasOne(u => u.DeliveryInfo)
                .WithOne()
                .HasForeignKey<DeliveryInfo>(di => di.UsersId);

            modelBuilder.Entity<User>()
                .Navigation(u => u.DeliveryInfo)
                .AutoInclude();


            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(o => o.OrdersId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UsersId);

            modelBuilder.Entity<Order>()
                .Navigation(o => o.OrderItems)
                .AutoInclude();




            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductsId)
                .OnDelete(DeleteBehavior.Cascade);



             modelBuilder.Entity<Request>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Requests)
                .HasForeignKey(r => r.ProductsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UsersId);


            modelBuilder.Entity<AdminAccount>()
                .HasOne(aa => aa.Admin)
                .WithOne(a => a.AdminAccount)
                .HasForeignKey<AdminAccount>(aa => aa.AdminsId);

            modelBuilder.Entity<AdminAccount>()
            .Navigation(ua => ua.Admin)
            .AutoInclude();


            modelBuilder.Entity<AdminProduct>()
               .HasOne(up => up.Admin)
               .WithMany(u => u.AdminProducts)
               .HasForeignKey(up => up.AdminsId);



            modelBuilder.Entity<AdminProduct>()
                .HasOne(up => up.Product)
                .WithMany(p => p.AdminProducts)
                .HasForeignKey(up => up.ProductsId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Admin>()
              .Navigation(a => a.AdminProducts)
              .AutoInclude();

            base.OnModelCreating(modelBuilder);
        }

        public async Task<User> FindUserToUserProducts(int id)
        {
            return await Task.Run(() =>
            {
                return Users
                .Include(u => u.UserProducts)
                .ThenInclude(up => up.Product)
                .AsParallel()
                .FirstOrDefault(u => u.Id == id);
            });
        }

        public async Task<User> FindUserAsync(string login)
        {
            return await Task.Run(() =>
            {
                return Users
                    .Include(u => u.UserAccount)
                    .FirstOrDefault(u => string.Equals(u.UserAccount.Login, login));
            });
        }

        public async Task<Admin> FindAdminAsync(string login)
        {
            return await Task.Run(() =>
            {
                return Admins
                    .Include(u => u.AdminAccount)
                    .FirstOrDefault(u => string.Equals(u.AdminAccount.Login, login));
            });
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Categories.ToListAsync();
        }

        public async Task<List<Character>> GetCategoryCharactersAsync(int id)
        {
            return await Characters
                    .AsNoTracking()
                    .Include(c => c.CharacterValues)
                    .Where(c => c.Categories.Any(c => c.Id == id))
                    .ToListAsync();
        }

        //var category = await Categories
        //.AsNoTracking()
        //.Include(c => c.Characters)
        //.ThenInclude(c => c.CharacterValues)
        //.FirstOrDefaultAsync(c => c.Id == categoryId);

        //    return category.Characters;

        //public async Task<List<Category>> GetCategoryCatalogAsync()
        //{
        //    return await Categories
        //        .Include(c => c.Products.Where(p => p.Status == (int)ProductStatus.Upload)) 
        //        .Include(c => c.Characters)
        //        .OrderByDescending(c => c.Id)
        //        .ToListAsync(); 
        //}

        public async Task<List<Category>> GetCategoryCatalogAsync()
        {
            return await Categories
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task LoadCategoryCharactersAsync(Category category)
        {
            await Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.Status == (int)ProductStatus.Upload)
                .LoadAsync();

            await Entry(category)
                .Collection(c => c.Characters)
                .LoadAsync();
        }


        public async Task<Product> FindProductEditCharactersAsync(int id)
        {           
            var product = await Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                await Entry(product)
                    .Collection(p => p.Categories)
                    .LoadAsync();

                foreach (var category in product.Categories)
                {
                    await Entry(category)
                        .Collection(c => c.Characters)
                        .LoadAsync();

                    foreach (var character in category.Characters)
                    {
                        await Entry(character)
                            .Collection(c => c.CharacterValues)
                            .LoadAsync();
                    }
                }
            }

            return product;
        }

        public async Task<Product> FindProductAsync(int id)
        {
            return await Task.Run(() =>
            {
                return Products.FirstOrDefault(p => p.Id == id);
            });
        }

        public void RemoveProduct(Product product)
        {
            var productDelete = this.Products
                .Include(p => p.AdminProducts)
                .Include(p => p.UserProducts)
                .Include(p => p.OrderItems)
                .Include(p => p.Requests)
                .FirstOrDefault(p => p.Id == product.Id);

            if (productDelete is not null)
            {
                this.Products.Remove(productDelete);

                this.SaveChanges();
            }
        }

        public async Task<User> FindUserToUserProductsAsync(int id)
        {
            return await Task.Run(() =>
            {
                return Users
                .Include(u => u.UserProducts)
                .ThenInclude(up => up.Product)
                .AsParallel()
                .FirstOrDefault(u => u.Id == id);
            });
        }

        public async Task<User> FindUserToAccountAsync(int id)
        {
            return await Task.Run(() =>
            {
                return Users
                    .Include(u => u.UserProducts)
                    .Include(u => u.Orders.OrderByDescending(o => o.Id))
                    .ThenInclude(o => o.OrderItems)
                    .Include(u => u.UserAccount)
                    .Include(u => u.DeliveryInfo)
                    .AsParallel()
                    .FirstOrDefault(u => u.Id == id);
            });
        }
    }
}
