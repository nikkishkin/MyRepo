/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     30.06.15 15:02:46                            */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Task') and o.name = 'FK_TASK_REFERENCE_USER')
alter table Task
   drop constraint FK_TASK_REFERENCE_USER
go

alter table Task
   drop column ManagerId
go

alter table Task
   add TeamId int                  null
go

/*==============================================================*/
/* Table: Team                                                  */
/*==============================================================*/
create table Team (
   Id                   int                  identity,
   Name                 nvarchar(100)        null,
   ManagerId            int                  null,
   constraint PK_TEAM primary key (Id)
)
go

alter table "User"
   add IsManager bit                  null
go

alter table "User"
   add TeamId int                  null
go

alter table Task
   add constraint FK_TASK_REFERENCE_TEAM foreign key (TeamId)
      references Team (Id)
go

alter table Team
   add constraint FK_TEAM_REFERENCE_USER foreign key (ManagerId)
      references "User" (Id)
go

alter table "User"
   add constraint FK_USER_REFERENCE_TEAM foreign key (TeamId)
      references Team (Id)
go

