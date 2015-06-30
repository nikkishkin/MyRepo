/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     29.06.15 13:27:39                            */
/*==============================================================*/


/*==============================================================*/
/* Table: Comment                                               */
/*==============================================================*/
create table Comment (
   Id                   int                  not null,
   Content              nvarchar(1000)       null,
   Date                 datetime             null,
   TaskId               int                  null,
   UserId               int                  null,
   constraint PK_COMMENT primary key (Id)
)
go

alter table Task
   add IsDone bit                  null
go

alter table Task
   add Date datetime             null
go

alter table Comment
   add constraint FK_COMMENT_REFERENCE_TASK foreign key (TaskId)
      references Task (Id)
go

alter table Comment
   add constraint FK_COMMENT_REFERENCE_USER foreign key (UserId)
      references "User" (Id)
go

