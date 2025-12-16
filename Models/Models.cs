
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Concept.Models
{
    public class DeffCountry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Code { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual required ICollection<DeffCity> Cities { get; set; }
    }

    public class DeffCity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Code { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public int CountryId { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CountryId")]
        public virtual required DeffCountry Country { get; set; }

        public virtual required ICollection<StoreItem> Items { get; set; }
    }

    public class DeffCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? AccountId { get; set; }
        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public virtual  ICollection<DeffSubCategory>? SubCategories { get; set; }
    }

    public class DeffSubCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Code { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? AccountId { get; set; }
        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual required DeffCategory Category { get; set; }

        public virtual required ICollection<StoreItem> Items { get; set; }
    }

    public class DefUOM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UOMCode { get; set; }

        [Required]
        public required string UOMName { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual required ICollection<DefSubUOM> SubUOMs { get; set; }
    }

    public class DefSubUOM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UOMId { get; set; }

        [Required]
        public required string SubUOMCode { get; set; }

        [Required]
        public required string SubUOMName { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UOMId")]
        public virtual required DefUOM UOM { get; set; }

        public virtual required ICollection<StoreItem> Items { get; set; }
    }

    public class DeffDepartment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string DepartmentCode { get; set; }

        [Required]
        public required string DepartmentName { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual required ICollection<DeffJobTitle> JobTitles { get; set; }
    }

    public class DeffJobTitle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string JobCode { get; set; }

        [Required]
        public required string JobName { get; set; }

        public int? DepartmentId { get; set; }
        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("DepartmentId")]
        public virtual required DeffDepartment Department { get; set; }

        public virtual required ICollection<UserInfo> Users { get; set; }
    }

    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UserCode { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string UserPassword { get; set; }

        public int? JobTitleId { get; set; }
        public int? PurchaseDefaultStatus { get; set; }
        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("JobTitleId")]
        public virtual required DeffJobTitle JobTitle { get; set; }

        public virtual required ICollection<StoreItem> Items { get; set; }
    }

    // =============================================
    // Main Store Item Model
    // =============================================

    public class StoreItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ItemCode { get; set; }

        public required string HRCode { get; set; }

        [Required]
        public required string ItemName { get; set; }

        public int? SubCategoryId { get; set; }
        public int? LiveDRMMaxcode { get; set; }
        public int? SubdueVOMMaxcode { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal QuantityInStore { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal PurchaseValue { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal PurchaseValueDefault { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal SaleValue { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal SaleValueDefault { get; set; } = 0;

        public int? CityId { get; set; }
        public int? SubUOMId { get; set; }

        public double? MaxCity { get; set; }
        public double? MinCity { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }

        public bool Active { get; set; } = true;

        public required string Description { get; set; }
        public required     string OperationsOrOperations { get; set; }
        public required string OperationWeight { get; set; }
        public required     string HSBarcode { get; set; }

        public int? UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("SubCategoryId")]
        public virtual required DeffSubCategory SubCategory { get; set; }

        [ForeignKey("CityId")]
        public virtual required DeffCity City { get; set; }

        [ForeignKey("SubUOMId")]
        public virtual required DefSubUOM SubUOM { get; set; }

        [ForeignKey("UserId")]
        public virtual required UserInfo User { get; set; }
    }
}
