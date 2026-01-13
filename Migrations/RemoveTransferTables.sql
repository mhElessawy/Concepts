-- Remove Transfer Tables Script
-- Run this in SQL Server Management Studio or Azure Data Studio

USE [ConceptWeb]
GO

-- Drop StoreTransfer_Details table first (has FK to Header)
IF OBJECT_ID('dbo.StoreTransfer_Details', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping StoreTransfer_Details table...'
    DROP TABLE dbo.StoreTransfer_Details;
    PRINT 'StoreTransfer_Details table dropped successfully.'
END
ELSE
BEGIN
    PRINT 'StoreTransfer_Details table does not exist.'
END
GO

-- Drop StoreTransfer_Header table
IF OBJECT_ID('dbo.StoreTransfer_Header', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping StoreTransfer_Header table...'
    DROP TABLE dbo.StoreTransfer_Header;
    PRINT 'StoreTransfer_Header table dropped successfully.'
END
ELSE
BEGIN
    PRINT 'StoreTransfer_Header table does not exist.'
END
GO

PRINT 'All Transfer tables removed successfully. You can now run: dotnet ef migrations add AddTransferModule'
GO
