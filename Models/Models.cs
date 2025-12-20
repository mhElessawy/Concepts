using System;
using System.Collections.Generic;
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
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<DeffCity> Cities { get; set; } = new List<DeffCity>();
        public virtual ICollection<StoreItem> Items { get; set; } = new List<StoreItem>();

    }
    public class DeffCity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int CountryId { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CountryId")]
        public virtual DeffCountry? Country { get; set; }

    
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

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<DeffSubCategory>? SubCategories { get; set; } = new List<DeffSubCategory>();
    }
    public class DeffSubCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? AccountId { get; set; }
        public bool Active { get; set; } = true;
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual DeffCategory? Category { get; set; }

        public virtual ICollection<StoreItem>? Items { get; set; } = new List<StoreItem>();
    }
    public class DefUOM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UOMCode { get; set; }

        [Required]
        public string UOMName { get; set; }

        public bool Active { get; set; } = true;

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<DefSubUOM>? SubUOMs { get; set; } = new List<DefSubUOM>();
    }
    public class DefSubUOM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UOMId { get; set; }

        [Required]
        public string SubUOMCode { get; set; }

        [Required]
        public string SubUOMName { get; set; }

        public bool Active { get; set; } = true;

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UOMId")]
        public virtual DefUOM? UOM { get; set; }

        public virtual ICollection<StoreItem>? Items { get; set; } = new List<StoreItem>();
    }
    public class DeffDepartment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DepartmentCode { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string Description { get; set; }

        public int? AccountId { get; set; }

        public virtual ICollection<UserInfo> Users { get; set; } = new List<UserInfo>();
    }
    public class DeffJobTitle
    {
       
        [Key]
        public int Id { get; set; }

        [Required]
        public string JobCode { get; set; }

        [Required]
        public string JobName { get; set; }

        // احتفظ بالحقل بس اجعله عادي بدون Foreign Key
        public int? DepartmentId { get; set; }

        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<UserInfo> Users { get; set; } = new List<UserInfo>();
        
    }
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserCode { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public int? JobTitleId { get; set; }
        public int? DepartmentId { get; set; }

        public int? LocationId { get; set; }

        public bool PurchaseOrderAuthorise { get; set; } = false;
        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("JobTitleId")]
        public virtual DeffJobTitle JobTitle { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DeffDepartment? Department { get; set; }

        [ForeignKey("LocationId")]
        public virtual DeffLocation? Location { get; set; }


        public virtual ICollection<StoreItem> Items { get; set; } = new List<StoreItem>();
    
    }
    public class StoreItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ItemCode { get; set; }

        public string HRCode { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
       public string ShortItemName { get; set; }

        public int? SubCategoryId { get; set; }

        public int? SubUOMId { get; set; }

        public int? PackSize { get; set; }


        [Column(TypeName = "money")]
        public decimal PurchaseValue { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal PurchaseValueDefault { get; set; } = 0;


        [Column(TypeName = "decimal(18,3)")]
        public decimal QuantityInStore { get; set; } = 0;


        [Column(TypeName = "money")]
        public decimal SaleValue { get; set; } = 0;

        [Column(TypeName = "money")]
        public decimal SaleValueDefault { get; set; } = 0;

        public int? CountryId { get; set; }

        public double? MaxCity { get; set; }
        public double? MinCity { get; set; }


        
        public int? UserId { get; set; }
        public bool Active { get; set; } = true;

        public string Description { get; set; }

        public string OperationsOrOperations { get; set; }
        public string OperationWeight { get; set; }
        public string HSBarcode { get; set; }

       

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        [ForeignKey("SubCategoryId")]
        public virtual DeffSubCategory? SubCategory { get; set; }

        [ForeignKey("CountryId")]
        public virtual DeffCountry? Country { get; set; }


        [ForeignKey("SubUOMId")]
        public virtual DefSubUOM? SubUOM { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }
    }

    public class Vender
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string VenderCode { get; set; }
        [Required]
        public string VenderName { get; set; }

        public int? AccounttId { get; set; }

        public int? CostCenterId { get; set; }

        public string BusinessType { get; set; }

        public string Address { get; set; }
        public int? CityId { get; set; }

        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
       
        public int? JobTitleId { get; set; }
        public int? BankId { get; set; }
        public string BankAccountNumber { get; set; } = string.Empty;

        public string BankAccountIBan { get; set; } = string.Empty;

        public string AdditionalInfo { get; set; } = string.Empty;

        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string Description { get; set; } = string.Empty;

        public int? AccountId { get; set; }

        public int? CostCenterIdd { get; set; }

        // Navigation Properties

        [ForeignKey("CityId")]
        public virtual DeffCity City { get; set; }

        [ForeignKey("JobTitleId")]
        public virtual DeffJobTitle JobTitle { get; set; }

        [ForeignKey("BankId")]
        public virtual DefBank Bank { get; set; }

    }   

    public class DefBank
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BankCode { get; set; }
        [Required]
        public string BankName { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
    public class DeffLocation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LocationCode { get; set; }
        [Required]
        public string LocationName { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string Description { get; set; }

        public int? AccountId { get; set; }

        public virtual ICollection<UserInfo> Users { get; set; } = new List<UserInfo>();
    }


}