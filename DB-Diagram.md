# Database Schema Diagram

## مخطط قاعدة بيانات النظام

```mermaid
erDiagram
    DeffCountry ||--o{ DeffCity : "has many"
    DeffCountry ||--o{ StoreItem : "has many"

    DeffCity ||--o{ Vender : "has many"

    DeffCategory ||--o{ DeffSubCategory : "has many"
    DeffSubCategory ||--o{ StoreItem : "has many"

    DefUOM ||--o{ DefSubUOM : "has many"
    DefSubUOM ||--o{ StoreItem : "has many"

    DeffDepartment ||--o{ UserInfo : "has many"
    DeffJobTitle ||--o{ UserInfo : "has many"
    DeffJobTitle ||--o{ Vender : "has many"

    DeffLocation ||--o{ UserInfo : "has many"

    DefBank ||--o{ Vender : "has many"

    UserInfo ||--o{ StoreItem : "has many"

    DeffCountry {
        int Id PK
        string Code UK
        string Name
        bool Active
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffCity {
        int Id PK
        string Code UK
        string Name
        int CountryId FK
        bool Active
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffCategory {
        int Id PK
        string Code UK
        string Name
        int AccountId
        bool Active
        string Description
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffSubCategory {
        int Id PK
        string Code UK
        string Name
        int CategoryId FK
        int AccountId
        bool Active
        string Description
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DefUOM {
        int Id PK
        string UOMCode
        string UOMName
        bool Active
        string Description
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DefSubUOM {
        int Id PK
        int UOMId FK
        string SubUOMCode
        string SubUOMName
        bool Active
        string Description
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffDepartment {
        int Id PK
        string DepartmentCode
        string DepartmentName
        bool Active
        string Description
        int AccountId
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffJobTitle {
        int Id PK
        string JobCode
        string JobName
        int DepartmentId
        bool Active
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DeffLocation {
        int Id PK
        string LocationCode
        string LocationName
        bool Active
        string Description
        int AccountId
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    UserInfo {
        int Id PK
        string UserCode
        string FullName
        string UserName
        string UserPassword
        int JobTitleId FK
        int DepartmentId FK
        int LocationId FK
        bool PurchaseOrderAuthorise
        bool Active
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    StoreItem {
        int Id PK
        string ItemCode
        string HRCode
        string ItemName
        string ShortItemName
        int SubCategoryId FK
        int SubUOMId FK
        int PackSize
        decimal PurchaseValue
        decimal PurchaseValueDefault
        decimal QuantityInStore
        decimal SaleValue
        decimal SaleValueDefault
        int CountryId FK
        double MaxCity
        double MinCity
        int UserId FK
        bool Active
        string Description
        string OperationsOrOperations
        string OperationWeight
        string HSBarcode
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    DefBank {
        int Id PK
        string BankCode UK
        string BankName
        bool Active
        DateTime CreatedDate
        DateTime ModifiedDate
    }

    Vender {
        int Id PK
        string VenderCode UK
        string VenderName
        int AccounttId
        int CostCenterId
        string BusinessType
        string Address
        int CityId FK
        string Email
        string ContactPerson
        string PhoneNumber
        int JobTitleId FK
        int BankId FK
        string BankAccountNumber
        string BankAccountIBan
        string AdditionalInfo
        bool Active
        string Description
        int AccountId
        int CostCenterIdd
        DateTime CreatedDate
        DateTime ModifiedDate
    }
```

## شرح العلاقات (Relationships)

### 1. علاقات الدول والمدن
- **DeffCountry → DeffCity**: دولة واحدة تحتوي على مدن متعددة (1:N)
- **DeffCountry → StoreItem**: دولة واحدة تحتوي على أصناف متعددة (1:N)
- **DeffCity → Vender**: مدينة واحدة تحتوي على موردين متعددين (1:N)

### 2. علاقات التصنيفات
- **DeffCategory → DeffSubCategory**: تصنيف واحد يحتوي على تصنيفات فرعية متعددة (1:N)
- **DeffSubCategory → StoreItem**: تصنيف فرعي واحد يحتوي على أصناف متعددة (1:N)

### 3. علاقات وحدات القياس
- **DefUOM → DefSubUOM**: وحدة قياس واحدة تحتوي على وحدات قياس فرعية متعددة (1:N)
- **DefSubUOM → StoreItem**: وحدة قياس فرعية واحدة تحتوي على أصناف متعددة (1:N)

### 4. علاقات الموظفين
- **DeffDepartment → UserInfo**: قسم واحد يحتوي على موظفين متعددين (1:N)
- **DeffJobTitle → UserInfo**: مسمى وظيفي واحد يحتوي على موظفين متعددين (1:N)
- **DeffJobTitle → Vender**: مسمى وظيفي واحد يحتوي على موردين متعددين (1:N)
- **DeffLocation → UserInfo**: موقع واحد يحتوي على موظفين متعددين (1:N)

### 5. علاقات البنوك
- **DefBank → Vender**: بنك واحد يحتوي على موردين متعددين (1:N)

### 6. علاقات أصناف المخزن
- **UserInfo → StoreItem**: موظف واحد يمكن أن يكون مسؤولاً عن أصناف متعددة (1:N)

## ملاحظات على قاعدة البيانات

### Indexes (الفهارس)
- جميع الأكواد (Codes) لها فهارس فريدة (Unique Indexes)
- فهارس على الحقول الأكثر استخداماً في البحث:
  - `StoreItem.ItemCode`
  - `StoreItem.Active`
  - `DeffSubCategory.CategoryId`
  - `DeffCity.CountryId`

### Delete Behaviors (سلوك الحذف)
- **Restrict**: منع الحذف إذا كانت هناك بيانات مرتبطة
  - `DeffCity → Country`
  - `DeffSubCategory → Category`
  - `DefSubUOM → UOM`

- **SetNull**: تعيين القيمة إلى NULL عند الحذف
  - `StoreItem → SubCategory`
  - `StoreItem → Country`
  - `StoreItem → SubUOM`
  - `Vender → City`
  - `Vender → JobTitle`
  - `Vender → Bank`
  - `UserInfo → JobTitle`
  - `UserInfo → Department`
  - `UserInfo → Location`

## الجداول الرئيسية

### جداول التعريفات (Definition Tables)
1. **DeffCountry** - الدول
2. **DeffCity** - المدن
3. **DeffCategory** - التصنيفات
4. **DeffSubCategory** - التصنيفات الفرعية
5. **DefUOM** - وحدات القياس
6. **DefSubUOM** - وحدات القياس الفرعية
7. **DeffDepartment** - الأقسام
8. **DeffJobTitle** - المسميات الوظيفية
9. **DeffLocation** - المواقع
10. **DefBank** - البنوك

### الجداول الرئيسية (Main Tables)
1. **UserInfo** - بيانات المستخدمين
2. **StoreItem** - أصناف المخزن
3. **Vender** - الموردين

## Database Tables Summary

| Table Name | Arabic Name | Record Count |
|------------|-------------|--------------|
| DeffCountry | الدول | Definition |
| DeffCity | المدن | Definition |
| DeffCategory | التصنيفات | Definition |
| DeffSubCategory | التصنيفات الفرعية | Definition |
| DefUOM | وحدات القياس | Definition |
| DefSubUOM | وحدات القياس الفرعية | Definition |
| DeffDepartment | الأقسام | Definition |
| DeffJobTitle | المسميات الوظيفية | Definition |
| DeffLocation | المواقع | Definition |
| DefBank | البنوك | Definition |
| UserInfo | المستخدمين | Transaction |
| StoreItem | أصناف المخزن | Transaction |
| Vender | الموردين | Transaction |

---

### Legend
- **PK** = Primary Key (المفتاح الأساسي)
- **FK** = Foreign Key (المفتاح الخارجي)
- **UK** = Unique Key (مفتاح فريد)
- **1:N** = One to Many (واحد إلى متعدد)
