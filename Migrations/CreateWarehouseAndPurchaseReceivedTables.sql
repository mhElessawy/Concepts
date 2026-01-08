-- SQL Script to create Warehouse and Purchase Received tables
-- Run this script in your SQL Server database

-- =============================================
-- Create Warehouse Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Warehouse]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Warehouse](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [WarehouseCode] [nvarchar](450) NOT NULL,
        [WarehouseName] [nvarchar](max) NOT NULL,
        [Active] [bit] NOT NULL,
        [AccountId] [int] NOT NULL,
        [CostId] [int] NOT NULL,
        [Description] [nvarchar](max) NULL,
        [LocationId] [int] NOT NULL,
        [UserId] [int] NOT NULL,
        [IVM] [nvarchar](max) NULL,
        [WarehouseType] [int] NOT NULL,
        [CountryId] [int] NOT NULL,
        [ManagerName] [nvarchar](max) NULL,
        [ManagerNumber] [nvarchar](max) NULL,
        [AdditionalNote] [nvarchar](max) NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) ON [PRIMARY]

    -- Create indexes
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Warehouse_WarehouseCode] ON [dbo].[Warehouse]
    (
        [WarehouseCode] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_Warehouse_CountryId] ON [dbo].[Warehouse]
    (
        [CountryId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_Warehouse_LocationId] ON [dbo].[Warehouse]
    (
        [LocationId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_Warehouse_UserId] ON [dbo].[Warehouse]
    (
        [UserId] ASC
    )

    -- Add foreign keys
    ALTER TABLE [dbo].[Warehouse] WITH CHECK ADD CONSTRAINT [FK_Warehouse_Deff_Country_CountryId]
    FOREIGN KEY([CountryId]) REFERENCES [dbo].[Deff_Country] ([Id]) ON DELETE NO ACTION

    ALTER TABLE [dbo].[Warehouse] WITH CHECK ADD CONSTRAINT [FK_Warehouse_Deff_Location_LocationId]
    FOREIGN KEY([LocationId]) REFERENCES [dbo].[Deff_Location] ([Id]) ON DELETE NO ACTION

    ALTER TABLE [dbo].[Warehouse] WITH CHECK ADD CONSTRAINT [FK_Warehouse_UserInfo_UserId]
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserInfo] ([Id]) ON DELETE NO ACTION

    PRINT 'Warehouse table created successfully'
END
ELSE
BEGIN
    PRINT 'Warehouse table already exists'
END
GO

-- =============================================
-- Create PurchaseRecieved_Header Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseRecieved_Header]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseRecieved_Header](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [RecieveDate] [datetime2](7) NOT NULL,
        [RecieveTime] [time](7) NOT NULL,
        [RecieveNo] [nvarchar](450) NULL,
        [BatchNo] [int] NOT NULL,
        [WarehouseId] [int] NOT NULL,
        [VenderId] [int] NOT NULL,
        [PurchaseOrderHeaderId] [int] NOT NULL,
        [VenderInvoiceNo] [int] NOT NULL,
        [PaymentTerms] [int] NOT NULL,
        [UserId] [int] NOT NULL,
        [AdditionalNotes] [nvarchar](max) NULL,
        [Approved] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_PurchaseRecieved_Header] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) ON [PRIMARY]

    -- Create indexes
    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Header_RecieveNo] ON [dbo].[PurchaseRecieved_Header]
    (
        [RecieveNo] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Header_UserId] ON [dbo].[PurchaseRecieved_Header]
    (
        [UserId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Header_VenderId] ON [dbo].[PurchaseRecieved_Header]
    (
        [VenderId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Header_WarehouseId] ON [dbo].[PurchaseRecieved_Header]
    (
        [WarehouseId] ASC
    )

    -- Add foreign keys
    ALTER TABLE [dbo].[PurchaseRecieved_Header] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Header_UserInfo_UserId]
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserInfo] ([Id]) ON DELETE NO ACTION

    ALTER TABLE [dbo].[PurchaseRecieved_Header] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Header_Vender_VenderId]
    FOREIGN KEY([VenderId]) REFERENCES [dbo].[Vender] ([Id]) ON DELETE NO ACTION

    ALTER TABLE [dbo].[PurchaseRecieved_Header] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Header_Warehouse_WarehouseId]
    FOREIGN KEY([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE NO ACTION

    PRINT 'PurchaseRecieved_Header table created successfully'
END
ELSE
BEGIN
    PRINT 'PurchaseRecieved_Header table already exists'
END
GO

-- =============================================
-- Create PurchaseRecieved_Details Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseRecieved_Details]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseRecieved_Details](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [PurchaseRecievedHeaderId] [int] NOT NULL,
        [SubCategoryId] [int] NOT NULL,
        [ItemId] [int] NOT NULL,
        [OrderQuantity] [decimal](18, 2) NOT NULL,
        [RecieveQuantity] [decimal](18, 2) NOT NULL,
        [PendingQuantity] [decimal](18, 2) NOT NULL,
        [FreeQuantity] [decimal](18, 2) NOT NULL,
        [TotalQuantity] [decimal](18, 2) NOT NULL,
        [UnitPrice] [decimal](18, 2) NOT NULL,
        [TotalPrice] [decimal](18, 2) NOT NULL,
        [Discount] [decimal](18, 2) NOT NULL,
        [NetPrice] [decimal](18, 2) NOT NULL,
        [SubUOMId] [int] NOT NULL,
        [PackSize] [int] NULL,
        [ValueOrUnit] [decimal](18, 2) NOT NULL,
        [ExpiredDate] [date] NOT NULL,
        CONSTRAINT [PK_PurchaseRecieved_Details] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) ON [PRIMARY]

    -- Create indexes
    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Details_ItemId] ON [dbo].[PurchaseRecieved_Details]
    (
        [ItemId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Details_PurchaseRecievedHeaderId] ON [dbo].[PurchaseRecieved_Details]
    (
        [PurchaseRecievedHeaderId] ASC
    )

    CREATE NONCLUSTERED INDEX [IX_PurchaseRecieved_Details_SubUOMId] ON [dbo].[PurchaseRecieved_Details]
    (
        [SubUOMId] ASC
    )

    -- Add foreign keys
    ALTER TABLE [dbo].[PurchaseRecieved_Details] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Details_Def_SubUOM_SubUOMId]
    FOREIGN KEY([SubUOMId]) REFERENCES [dbo].[Def_SubUOM] ([Id]) ON DELETE NO ACTION

    ALTER TABLE [dbo].[PurchaseRecieved_Details] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Details_PurchaseRecieved_Header_PurchaseRecievedHeaderId]
    FOREIGN KEY([PurchaseRecievedHeaderId]) REFERENCES [dbo].[PurchaseRecieved_Header] ([Id]) ON DELETE CASCADE

    ALTER TABLE [dbo].[PurchaseRecieved_Details] WITH CHECK ADD CONSTRAINT [FK_PurchaseRecieved_Details_Store_Item_ItemId]
    FOREIGN KEY([ItemId]) REFERENCES [dbo].[Store_Item] ([Id]) ON DELETE NO ACTION

    PRINT 'PurchaseRecieved_Details table created successfully'
END
ELSE
BEGIN
    PRINT 'PurchaseRecieved_Details table already exists'
END
GO

-- =============================================
-- Insert Migration History Record
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260108000000_AddWarehouseAndPurchaseReceivedTables')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260108000000_AddWarehouseAndPurchaseReceivedTables', N'8.0.11')

    PRINT 'Migration history record added'
END
GO

PRINT 'All tables created successfully!'
GO
