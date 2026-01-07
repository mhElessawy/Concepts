using Microsoft.EntityFrameworkCore;
using Concept.Models;

namespace Concept.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<DeffCountry> DeffCountries { get; set; }
        public DbSet<DeffCity> DeffCities { get; set; }
        public DbSet<DeffCategory> DeffCategories { get; set; }
        public DbSet<DeffSubCategory> DeffSubCategories { get; set; }
        public DbSet<DefUOM> DefUOMs { get; set; }
        public DbSet<DefSubUOM> DefSubUOMs { get; set; }
        public DbSet<DeffDepartment> DeffDepartments { get; set; }
        public DbSet<DeffJobTitle> DeffJobTitles { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<DefBank> DefBanks { get; set; }
        public DbSet<Vender> Venders { get; set; }
        public DbSet<DeffLocation> DeffLocations { get; set; }
        public DbSet<PurchaseRequestHeader> PurchaseRequestHeaders { get; set; }
        public DbSet<PurchaseRequestDetails> PurchaseRequestDetails { get; set; }

        public DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<DeffCountry>().ToTable("Deff_Country");
            modelBuilder.Entity<DeffCity>().ToTable("Deff_City");
            modelBuilder.Entity<DeffCategory>().ToTable("Deff_Category");
            modelBuilder.Entity<DeffSubCategory>().ToTable("Deff_SubCategory");
            modelBuilder.Entity<DefUOM>().ToTable("Def_UOM");
            modelBuilder.Entity<DefSubUOM>().ToTable("Def_SubUOM");
            modelBuilder.Entity<DeffDepartment>().ToTable("Deff_Department");
            modelBuilder.Entity<DeffJobTitle>().ToTable("Deff_JobTitle");
            modelBuilder.Entity<UserInfo>().ToTable("UserInfo");
            modelBuilder.Entity<StoreItem>().ToTable("Store_Item");
            modelBuilder.Entity<DeffLocation>().ToTable("Deff_Location");

            // Configure unique constraints
            modelBuilder.Entity<DeffCountry>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<DeffCity>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<DeffCategory>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<DeffSubCategory>().HasIndex(sc => sc.Code).IsUnique();

            // Configure indexes for better performance
            modelBuilder.Entity<StoreItem>().HasIndex(i => i.ItemCode);
            modelBuilder.Entity<StoreItem>().HasIndex(i => i.Active);
            modelBuilder.Entity<DeffSubCategory>().HasIndex(sc => sc.CategoryId);
            modelBuilder.Entity<DeffCity>().HasIndex(c => c.CountryId);

            // Configure relationships
            modelBuilder.Entity<DeffCity>()
                .HasOne(c => c.Country)
                .WithMany(co => co.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeffSubCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DefSubUOM>()
                .HasOne(su => su.UOM)
                .WithMany(u => u.SubUOMs)
                .HasForeignKey(su => su.UOMId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoreItem>()
                .HasOne(i => i.SubCategory)
                .WithMany(sc => sc.Items)
                .HasForeignKey(i => i.SubCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreItem>()
                .HasOne(i => i.Country)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreItem>()
                .HasOne(i => i.SubUOM)
                .WithMany(su => su.Items)
                .HasForeignKey(i => i.SubUOMId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure table names
            modelBuilder.Entity<DefBank>().ToTable("Def_Bank");
            modelBuilder.Entity<Vender>().ToTable("Vender");

            // Configure indexes
            modelBuilder.Entity<DefBank>().HasIndex(b => b.BankCode).IsUnique();
            modelBuilder.Entity<Vender>().HasIndex(v => v.VenderCode).IsUnique();

            // Configure relationships
            modelBuilder.Entity<Vender>()
                .HasOne(v => v.City)
                .WithMany()
                .HasForeignKey(v => v.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vender>()
                .HasOne(v => v.JobTitle)
                .WithMany()
                .HasForeignKey(v => v.JobTitleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vender>()
                .HasOne(v => v.Bank)
                .WithMany()
                .HasForeignKey(v => v.BankId)
                .OnDelete(DeleteBehavior.SetNull);

            // User relationships
            modelBuilder.Entity<UserInfo>()
                .HasOne(u => u.JobTitle)
                .WithMany(j => j.Users)
                .HasForeignKey(u => u.JobTitleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserInfo>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserInfo>()
                .HasOne(u => u.Location)
                .WithMany()
                .HasForeignKey(u => u.LocationId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    
    }
}