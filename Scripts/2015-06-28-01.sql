/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     28.06.15 23:54:31                            */
/*==============================================================*/


alter table "User"
   add "First Name" nvarchar(100)        null
go

alter table "User"
   add "Last Name" nvarchar(100)        null
go

execute sp_rename "[User].Name", Username, 'COLUMN'
go

