USE [TManager]
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [Username], [Password], [Email], [First Name], [Last Name], [IsManager], [TeamId]) VALUES (1, N'nik', N'$2a$10$ZGhD.C1muhKRSkc6UQx12.8Rhx9J3OUZ6/jYB2IslfWpUSRiGNiC.', N'nikkishkin@gmail.com', N'Nikita', N'Kishkin', 1, NULL)
INSERT [dbo].[User] ([Id], [Username], [Password], [Email], [First Name], [Last Name], [IsManager], [TeamId]) VALUES (2, N'Jane', N'$2a$10$kiopZYt5q8.N09i9xAh1k.H4Ta13BGFYszbTRFKocuTlzK587LOp6', N'jj@ya.ru', N'Jane', N'Shestuk', 0, 1)
INSERT [dbo].[User] ([Id], [Username], [Password], [Email], [First Name], [Last Name], [IsManager], [TeamId]) VALUES (3, N'norm', N'$2a$10$mlEXrx/sx/Y63GIGDQ0Lv.ZEeSjd3UzkNtVbUePbPdTbFqhtwYT5S', N'aa@ya.ru', N'Ivan', N'Ivanov', 1, NULL)
SET IDENTITY_INSERT [dbo].[User] OFF
