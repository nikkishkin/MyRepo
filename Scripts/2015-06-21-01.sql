/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     21.06.15 14:25:06                            */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Task') and o.name = 'FK_TASK_REFERENCE_USER')
alter table Task
   drop constraint FK_TASK_REFERENCE_USER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('WorkerTask') and o.name = 'FK_WORKERTA_REFERENCE_USER')
alter table WorkerTask
   drop constraint FK_WORKERTA_REFERENCE_USER
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('WorkerTask') and o.name = 'FK_WORKERTA_REFERENCE_TASK')
alter table WorkerTask
   drop constraint FK_WORKERTA_REFERENCE_TASK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Task')
            and   type = 'U')
   drop table Task
go

if exists (select 1
            from  sysobjects
           where  id = object_id('"User"')
            and   type = 'U')
   drop table "User"
go

if exists (select 1
            from  sysobjects
           where  id = object_id('WorkerTask')
            and   type = 'U')
   drop table WorkerTask
go

/*==============================================================*/
/* Table: Task                                                  */
/*==============================================================*/
create table Task (
   Id                   int                  identity,
   Percentage           int                  null,
   Content              nvarchar(1000)       null,
   ManagerId            int                  null,
   constraint PK_TASK primary key (Id)
)
go

/*==============================================================*/
/* Table: "User"                                                */
/*==============================================================*/
create table "User" (
   Id                   int                  identity,
   Name                 nvarchar(100)        null,
   Password             nvarchar(100)        null,
   Email                nvarchar(50)         null,
   constraint PK_USER primary key nonclustered (Id)
)
go

/*==============================================================*/
/* Table: WorkerTask                                            */
/*==============================================================*/
create table WorkerTask (
   WorkerId             int                  not null,
   TaskId               int                  not null,
   constraint PK_WORKERTASK primary key (WorkerId, TaskId)
)
go

alter table Task
   add constraint FK_TASK_REFERENCE_USER foreign key (ManagerId)
      references "User" (Id)
go

alter table WorkerTask
   add constraint FK_WORKERTA_REFERENCE_USER foreign key (WorkerId)
      references "User" (Id)
go

alter table WorkerTask
   add constraint FK_WORKERTA_REFERENCE_TASK foreign key (TaskId)
      references Task (Id)
go

