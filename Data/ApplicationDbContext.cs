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

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<PurchaseRecievedHeader> PurchaseRecievedHeaders { get; set; }
        public DbSet<PurchaseRecievedDetails> PurchaseRecievedDetails { get; set; }

        public DbSet<StoreTransferHeader> StoreTransferHeaders { get; set; }
        public DbSet<StoreTransferDetails> StoreTransferDetails { get; set; }

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

            // Warehouse Configuration

            modelBuilder.Entity<Warehouse>().ToTable("Warehouse");

            modelBuilder.Entity<Warehouse>().HasIndex(w => w.WarehouseCode).IsUnique();



            modelBuilder.Entity<Warehouse>()

                .HasOne(w => w.Location)

                .WithMany()

                .HasForeignKey(w => w.LocationId)

                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Warehouse>()

                .HasOne(w => w.Country)

                .WithMany()

                .HasForeignKey(w => w.CountryId)

                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Warehouse>()

                .HasOne(w => w.User)

                .WithMany()

                .HasForeignKey(w => w.UserId)

                .OnDelete(DeleteBehavior.Restrict);



            // Purchase Received Header Configuration

            modelBuilder.Entity<PurchaseRecievedHeader>().ToTable("PurchaseRecieved_Header");

            modelBuilder.Entity<PurchaseRecievedHeader>().HasIndex(p => p.RecieveNo);



            modelBuilder.Entity<PurchaseRecievedHeader>()

                .HasOne(p => p.Warehouse)

                .WithMany()

                .HasForeignKey(p => p.WarehouseId)

                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PurchaseRecievedHeader>()

                .HasOne(p => p.Vender)

                .WithMany()

                .HasForeignKey(p => p.VenderId)

                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<PurchaseRecievedHeader>()

                .HasOne(p => p.User)

                .WithMany()

                .HasForeignKey(p => p.UserId)

                .OnDelete(DeleteBehavior.Restrict);



            // Purchase Received Details Configuration

            modelBuilder.Entity<PurchaseRecievedDetails>().ToTable("PurchaseRecieved_Details");
            modelBuilder.Entity<PurchaseRecievedDetails>()

                .HasOne(p => p.PurchaseRecievedHeader)

                .WithMany()

                .HasForeignKey(p => p.PurchaseRecievedHeaderId)

                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<PurchaseRecievedDetails>()

                .HasOne(p => p.SubUOM)

                .WithMany()

                .HasForeignKey(p => p.SubUOMId)

                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<PurchaseRecievedDetails>()

                .HasOne(p => p.StoreItem)

                .WithMany()

                .HasForeignKey(p => p.ItemId)

                .OnDelete(DeleteBehavior.Restrict);



            // Configure decimal precision

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.OrderQuantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.RecieveQuantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.PendingQuantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.FreeQuantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.TotalQuantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.Discount).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.NetPrice).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRecievedDetails>()

                .Property(p => p.ValueOrUnit).HasColumnType("decimal(18,2)");

            // Store Transfer Header Configuration
            modelBuilder.Entity<StoreTransferHeader>().ToTable("StoreTransfer_Header");
            modelBuilder.Entity<StoreTransferHeader>().HasIndex(t => t.TransferNo);

            modelBuilder.Entity<StoreTransferHeader>()
                .HasOne(t => t.FromWarehouse)
                .WithMany()
                .HasForeignKey(t => t.FromWarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreTransferHeader>()
                .HasOne(t => t.FromDepartment)
                .WithMany()
                .HasForeignKey(t => t.FromDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreTransferHeader>()
                .HasOne(t => t.ToWarehouse)
                .WithMany()
                .HasForeignKey(t => t.ToWarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreTransferHeader>()
                .HasOne(t => t.ToDepartment)
                .WithMany()
                .HasForeignKey(t => t.ToDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StoreTransferHeader>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store Transfer Details Configuration
            modelBuilder.Entity<StoreTransferDetails>().ToTable("StoreTransfer_Details");

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.storeTransferHeader)
                .WithMany()
                .HasForeignKey(t => t.StoreTransferHeaderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.SubCategory)
                .WithMany()
                .HasForeignKey(t => t.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.Item)
                .WithMany()
                .HasForeignKey(t => t.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.UOM)
                .WithMany()
                .HasForeignKey(t => t.UOMId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoreTransferDetails>()
                .HasOne(t => t.SubUOM)
                .WithMany()
                .HasForeignKey(t => t.SubUOMId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for Transfer Details
            modelBuilder.Entity<StoreTransferDetails>()
                .Property(t => t.Quantity).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StoreTransferDetails>()
                .Property(t => t.PriceType).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StoreTransferDetails>()
                .Property(t => t.CostType).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StoreTransferDetails>()
                .Property(t => t.TotalType).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StoreTransferDetails>()
                .Property(t => t.ValueOrUnit).HasColumnType("decimal(18,2)");
        }
    }
}