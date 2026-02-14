using Microsoft.AspNetCore.Routing.Constraints;
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

        public virtual ICollection<UserInfo>? Users { get; set; } = new List<UserInfo>();

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

        public bool CanApprovePurchaseOrders { get; set; } = false;

        // Navigation Properties
        [ForeignKey("JobTitleId")]
        public virtual DeffJobTitle JobTitle { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DeffDepartment Department { get; set; }

        [ForeignKey("LocationId")]
        public virtual DeffLocation Location { get; set; }

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
        public string VenderCode { get; set; } = string.Empty;
        [Required]
        public string VenderName { get; set; }

        public int? AccounttId { get; set; }

        public int? CostCenterId { get; set; }

        [Required]
        public string AccountNo { get; set; } = string.Empty;

        [Required]
        public int PaymentTerms { get; set; }

        public string? BusinessType { get; set; }

        public string? Address { get; set; }
        public int? CityId { get; set; }

        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? PhoneNumber { get; set; }

        public int? JobTitleId { get; set; }
        public int? BankId { get; set; }
        public string? BankAccountNumber { get; set; }

        public string? BankAccountIBan { get; set; }

        public string? AdditionalInfo { get; set; }

        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        public int? AccountId { get; set; }

        public int? CostCenterIdd { get; set; }

        // Navigation Properties

        [ForeignKey("CityId")]
        public virtual DeffCity City { get; set; }

        [ForeignKey("JobTitleId")]
        public virtual DeffJobTitle JobTitle { get; set; }

        [ForeignKey("BankId")]
        public virtual DefBank Bank { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual DeffCostCenter CostCenter { get; set; }

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
    public class DeffCostCenter
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CostCenterCode { get; set; }
        [Required]
        public string CostCenterName { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public string Description { get; set; }
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
    public class PurchaseRequestHeader
    {
        [Key]
        public int Id { get; set; }
        public string RequestCode { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public TimeOnly RequestTime { get; set; }

        public string RequestNo { get; set; }
        public int DepartmentId { get; set; }

        public int UserId { get; set; }
        public int RequestedStatus { get; set; } = 0;

        public int VenderId { get; set; }

        public string AdditionalNotes { get; set; }

        public int Approved { get; set; } = 0;
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        // Navigation Properties    
        [ForeignKey("DepartmentId")]
        public virtual DeffDepartment Department { get; set; }
        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }

        [ForeignKey("VenderId")]
        public virtual Vender Vender { get; set; }
    }
    public class PurchaseRequestDetails
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseRequestHeaderId { get; set; }

        public int SubCategoryId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public int SubUnitId { get; set; }
        public int? PackSize { get; set; }
        public string Notes { get; set; }

        // Navigation Properties

        [ForeignKey("PurchaseRequestHeaderId")]
        public virtual PurchaseRequestHeader PurchaseRequestHeader { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual DeffSubCategory SubCategory { get; set; }
        [ForeignKey("ItemId")]
        public virtual StoreItem Item { get; set; }
        [ForeignKey("SubUnitId")]
        public virtual DefSubUOM SubUOM { get; set; }

    }
    public class PurchaseOrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string PurchaseCode { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public TimeOnly PurchaseTime { get; set; }

        public string PurchaseNo { get; set; }
        public int DepartmentId { get; set; }

        public int UserId { get; set; }
        public int PurchaseStatus { get; set; } = 0;

        public int VenderId { get; set; }

        public string AdditionalNotes { get; set; }

        public int Approved { get; set; } = 0;
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        // Navigation Properties    
        [ForeignKey("DepartmentId")]
        public virtual DeffDepartment Department { get; set; }
        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }

        [ForeignKey("VenderId")]
        public virtual Vender Vender { get; set; }
    }
    public class PurchaseOrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseOrderHeaderId { get; set; }

        public int SubCategoryId { get; set; }
        public int ItemId { get; set; }
        public decimal AvQuantity { get; set; }
        public decimal AvMoney { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public int SubUnitId { get; set; }
        public int? PackSize { get; set; }
        public decimal ValueOrUnit { get; set; }
        public int freeQuantity { get; set; }



        // Navigation Properties

        [ForeignKey("PurchaseOrderHeaderId")]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }


        [ForeignKey("SubCategoryId")]
        public virtual DeffSubCategory SubCategory { get; set; }
        [ForeignKey("ItemId")]
        public virtual StoreItem Item { get; set; }
        [ForeignKey("SubUnitId")]
        public virtual DefSubUOM SubUOM { get; set; }

    }
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? WarehouseCode { get; set; }
        [Required]
        public string? WarehouseName { get; set; }
        public bool Active { get; set; } = true;

        public int AccountId { get; set; }

        public int CostId { get; set; }

        public string? Description { get; set; }

        public int LocationId { get; set; }

        public int UserId { get; set; }

        public string? IVM { get; set; }

        public int WarehouseType { get; set; }

        public int CountryId { get; set; }

        public string? ManagerName { get; set; }
        public string? ManagerNumber { get; set; }

        public string? AdditionalNote { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties  
        [ForeignKey("LocationId")]
        public virtual DeffLocation? Location { get; set; }
        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }

        [ForeignKey("CountryId")]
        public virtual DeffCountry? Country { get; set; }

    }
    public class PurchaseRecievedHeader
    {
        [Key]
        public int Id { get; set; }
        public DateTime RecieveDate { get; set; } = DateTime.Now;
        public TimeOnly RecieveTime { get; set; }
        public string? RecieveNo { get; set; }
        public int BatchNo { get; set; }
        public int WarehouseId { get; set; }
        public int VenderId { get; set; }
        public int PurchaseOrderHeaderId { get; set; }

        public int VenderInvoiceNo { get; set; }

        public int PaymentTerms { get; set; }


        public int UserId { get; set; }


        public string? AdditionalNotes { get; set; }
        public int Approved { get; set; } = 0;
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        // Navigation Properties    
        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }
        [ForeignKey("VenderId")]
        public virtual Vender? Vender { get; set; }

    }
    public class PurchaseRecievedDetails
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseRecievedHeaderId { get; set; }
        public int SubCategoryId { get; set; }
        public int ItemId { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal RecieveQuantity { get; set; }
        public decimal PendingQuantity { get; set; }
        public decimal FreeQuantity { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public int SubUOMId { get; set; }
        public int? PackSize { get; set; }
        public decimal ValueOrUnit { get; set; }
        public DateOnly ExpiredDate { get; set; }
        // Navigation Properties
        [ForeignKey("PurchaseRecievedHeaderId")]
        public virtual PurchaseRecievedHeader? PurchaseRecievedHeader { get; set; }
        [ForeignKey("SubUOMId")]
        public virtual DefSubUOM? SubUOM { get; set; }
        [ForeignKey("ItemId")]
        public virtual StoreItem? StoreItem { get; set; }

    }

    public class StoreTransferHeader
    {
        [Key]
        public int Id { get; set; }
        public string? TransferCode { get; set; }
        public int? TransferNo { get; set; }
        public int TransferType { get; set; }
        public int RequestedBy{ get; set; }
        public int AprovedBy { get; set; }
        public int TransferStatus { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.Now;
        public TimeOnly TransferTime { get; set; }
        public int FromWarehouseId { get; set; } = 0;
        public int FromDepartmentId { get; set; } = 0;

        public int ToWarehouseId { get; set; } = 0;
        public int ToDepartmentId { get; set; } = 0;
        public int UserId { get; set; }
        public string? AdditionalNotes { get; set; }
       public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        // Navigation Properties (No Foreign Key Constraints for From/To fields)
        public virtual Warehouse? FromWarehouse { get; set; }
        public virtual DeffDepartment? FromDepartment { get; set; }
        public virtual Warehouse? ToWarehouse { get; set; }
        public virtual DeffDepartment? ToDepartment { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }
    }

    public class StoreTransferDetails
    {
        [Key]
        public int Id { get; set; }

        public int StoreTransferHeaderId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int ItemId { get; set; }
        public int UOMId { get; set; }
        public string BatchNo  { get; set; }
        public decimal Quantity { get; set; }

        public decimal PriceType { get; set; }
        public decimal CostType { get; set; }
        public decimal TotalType { get; set; }
        public int SubUOMId { get; set; }
        public int? PackSize { get; set; }
        public decimal ValueOrUnit { get; set; }
        public DateOnly ExpiredDate { get; set; }
        public String  Remark { get; set; }

        // Navigation Properties
        [ForeignKey("StoreTransferHeaderId")]
        public virtual StoreTransferHeader? storeTransferHeader { get; set; }
        [ForeignKey("CategoryId")]
        public virtual DeffCategory Category { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual DeffSubCategory SubCategory { get; set; }
        [ForeignKey("ItemId")]
        public virtual StoreItem Item { get; set; }
        [ForeignKey("UOMId")]
        public virtual DefUOM UOM { get; set; }
      
        [ForeignKey("SubUOMId")]
        public virtual DefSubUOM? SubUOM { get; set; }
      
    }

    public class StoreReturnHeader
    {
        [Key]
        public int Id { get; set; }
        public string? ReturnCode { get; set; }
        public int? ReturnNo { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public TimeOnly ReturnTime { get; set; }
        public int ReturnType { get; set; }

        public int FromWarehouseId { get; set; } = 0;
        public int FromDepartmentId { get; set; } = 0;

        public int ToWarehouseId { get; set; } = 0;
        public int ToDepartmentId { get; set; } = 0;
        public int ReturnStatus { get; set; } = 0; // 0=Pending, 1=Approved, 2=Rejected
        public int? ApprovedBy { get; set; }
        public int UserId { get; set; }
        public string? AdditionalNotes { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public virtual Warehouse? FromWarehouse { get; set; }
        public virtual DeffDepartment? FromDepartment { get; set; }
        public virtual Warehouse? ToWarehouse { get; set; }
        public virtual DeffDepartment? ToDepartment { get; set; }
        public virtual UserInfo? User { get; set; }

    }
    public class StoreReturnDetails
    {
        [Key]
        public int Id { get; set; }
        public int StoreReturnHeaderId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int ItemId { get; set; }
        public int UOMId { get; set; }
        public string BatchNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceType { get; set; }
        public decimal CostType { get; set; }
        public decimal TotalType { get; set; }
        public int SubUOMId { get; set; }
        public int? PackSize { get; set; }
        public decimal ValueOrUnit { get; set; }
        public DateOnly ExpiredDate { get; set; }
        public String Remark { get; set; }
        // Navigation Properties
        [ForeignKey("StoreReturnHeaderId")]
        public virtual StoreReturnHeader? storeReturnHeader { get; set; }
        [ForeignKey("CategoryId")]
        public virtual DeffCategory Category { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual DeffSubCategory SubCategory { get; set; }
        [ForeignKey("ItemId")]
        public virtual StoreItem Item { get; set; }
        [ForeignKey("UOMId")]
        public virtual DefUOM UOM { get; set; }
        [ForeignKey("SubUOMId")]
        public virtual DefSubUOM? SubUOM { get; set; }
    }

}