using ALJEproject.Models;
using Microsoft.EntityFrameworkCore;

namespace ALJEproject.Data
{
    public class ALJEprojectDbContext : DbContext
    {
        public ALJEprojectDbContext(DbContextOptions<ALJEprojectDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleView> UserRoles { get; set; }  // DbSet for the view
        public DbSet<Option> Options { get; set; }
        public DbSet<Menu> Menus { get; set; }

        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<UserAccessView> UserAccessesView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Option>().ToTable("Options");
            modelBuilder.Entity<Menu>().ToTable("Menus");
            modelBuilder.Entity<UserAccess>().ToTable("UserAccess");

            // Configure UserRoleView to map to the vw_UserRoles view
            modelBuilder.Entity<UserRoleView>()
                .HasNoKey()  // Specify that this entity has no primary key
                .ToView("vw_UserRoles");  // Map to the view name in the database
            modelBuilder.Entity<UserAccessView>()
               .HasNoKey()  // Specify that this entity has no primary key
               .ToView("vw_UserAccess");  // Map to the view name in the database

            modelBuilder.Entity<Menu>()
               .HasOne(m => m.ParentMenu)
               .WithMany(m => m.SubMenus)
               .HasForeignKey(m => m.ParentMenuID);


        }
    }
}
