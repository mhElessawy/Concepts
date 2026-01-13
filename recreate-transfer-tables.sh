#!/bin/bash
# Script to recreate Transfer tables

echo "This script will help you recreate Transfer tables"
echo "=================================================="
echo ""
echo "Step 1: Please run the following SQL on your SQL Server:"
echo "-------------------------------------------------------"
echo "DROP TABLE IF EXISTS dbo.StoreTransfer_Details;"
echo "DROP TABLE IF EXISTS dbo.StoreTransfer_Header;"
echo ""
echo "Step 2: After running the SQL, press Enter to continue..."
read

echo "Step 3: Creating new migration..."
dotnet ef migrations add AddTransferModuleClean

echo ""
echo "Step 4: Updating database..."
dotnet ef database update

echo ""
echo "Done! Transfer tables have been recreated."
