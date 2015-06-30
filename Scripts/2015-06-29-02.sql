/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     29.06.15 17:12:12                            */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Comment') and o.name = 'FK_COMMENT_REFERENCE_TASK')
alter table Comment
   drop constraint FK_COMMENT_REFERENCE_TASK
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Comment') and o.name = 'FK_COMMENT_REFERENCE_USER')
alter table Comment
   drop constraint FK_COMMENT_REFERENCE_USER
go

alter table Comment
   drop constraint PK_COMMENT
go

if exists (select 1
            from  sysobjects
           where  id = object_id('tmp_Comment')
            and   type = 'U')
   drop table tmp_Comment
go

execute sp_rename Comment, tmp_Comment
go

/*==============================================================*/
/* Table: Comment                                               */
/*==============================================================*/
create table Comment (
   Id                   int                  identity,
   Content              nvarchar(1000)       null,
   Date                 datetime             null,
   TaskId               int                  null,
   UserId               int                  null,
   constraint PK_COMMENT primary key (Id)
)
go

set identity_insert Comment on
go

insert into Comment (Id, Content, Date, TaskId, UserId)
select Id, Content, Date, TaskId, UserId
from tmp_Comment
go

set identity_insert Comment off
go

if exists (select 1
            from  sysobjects
           where  id = object_id('tmp_Comment')
            and   type = 'U')
   drop table tmp_Comment
go

alter table Comment
   add constraint FK_COMMENT_REFERENCE_TASK foreign key (TaskId)
      references Task (Id)
go

alter table Comment
   add constraint FK_COMMENT_REFERENCE_USER foreign key (UserId)
      references "User" (Id)
go

