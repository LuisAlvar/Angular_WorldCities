-- remove foreign keys first 
while(exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY'))
begin 
  declare @sql nvarchar(max)
  set @sql = (select top 1 ('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
  from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
  where CONSTRAINT_TYPE='FOREIGN KEY')
  EXEC (@sql)
  SELECT @sql
end

-- droping all of the tbales within azure db 
while(exists(select 1 from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'))
begin 
  declare @sql nvarchar(max)
  set @sql = (select top 1 ('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
  from INFORMATION_SCHEMA.TABLES
  where TABLE_TYPE='BASE TABLE')
  EXEC (@sql)
  SELECT @sql
end