
--CREATE LOGIN WorldCities
--	WITH PASSWORD = 'MyVeryOwn' 
--GO


-- =======================================================================================
-- Create User as DBO template for Azure SQL Database and Azure Synapse Analytics Database
-- =======================================================================================
-- For login login_name, create a user in the database
CREATE USER WorldCities
	FOR LOGIN WorldCities
	WITH DEFAULT_SCHEMA = dbo
GO

-- =======================================================================================
-- Create Azure Active Directory User for Azure SQL Database and Azure Synapse Analytics Database
-- =======================================================================================
-- For login <login_name, sysname, login_name>, create a user in the database
-- CREATE USER <Azure_Active_Directory_Principal_User, sysname, user_name>
--    [   { FOR | FROM } LOGIN <Azure_Active_Directory_Principal_Login, sysname, login_name>  ]  
--    | FROM EXTERNAL PROVIDER
--    [ WITH DEFAULT_SCHEMA = <default_schema, sysname, dbo> ]
-- GO


-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'WorldCities'
GO
