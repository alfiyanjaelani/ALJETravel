USE [ALJEproject]
GO
/****** Object:  User [sa_alje]    Script Date: 11/28/2024 19:31:32 ******/
CREATE USER [sa_alje] FOR LOGIN [sa_alje] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [sa_alje]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[RoleID] [int] NOT NULL,
	[CompanyName] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[FullName] [nvarchar](100) NULL,
	[EmailAddress] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_UserRoles]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_UserRoles] AS
SELECT 
    u.UserId,
    u.UserName,
    u.FullName,
    u.CompanyName,
    u.EmailAddress,
    u.Phone,
    u.RoleID,
    r.RoleName,
	u.CreatedBy,
	u.CreatedDate,
	u.UpdatedBy,
	u.UpdatedDate
FROM 
    Users u
INNER JOIN 
    Roles r ON u.RoleID = r.RoleID;
GO
/****** Object:  Table [dbo].[Menus]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menus](
	[MenuID] [int] IDENTITY(1,1) NOT NULL,
	[ControllerName] [nvarchar](100) NOT NULL,
	[MenuName] [nvarchar](100) NOT NULL,
	[MenuDesc] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[Active] [bit] NULL,
	[MenuURL] [nvarchar](max) NULL,
	[MenuOrder] [int] NULL,
	[ParentMenuID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAccess]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccess](
	[UserAccessID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[MenuID] [int] NOT NULL,
	[Views] [bit] NOT NULL,
	[Inserts] [bit] NOT NULL,
	[Edits] [bit] NOT NULL,
	[Deletes] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserAccessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_UserAccess]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_UserAccess]
AS
SELECT        TOP (100) PERCENT uc.UserAccessID, uc.RoleID, r.RoleName, uc.MenuID, m.MenuName, uc.Views, uc.Inserts, uc.Edits, uc.Deletes, uc.CreatedBy, uc.CreatedDate, uc.UpdatedBy, uc.UpdatedDate
FROM            dbo.UserAccess AS uc INNER JOIN
                         dbo.Menus AS m ON m.MenuID = uc.MenuID INNER JOIN
                         dbo.Roles AS r ON r.RoleID = uc.RoleID
ORDER BY uc.UserAccessID
GO
/****** Object:  Table [dbo].[Options]    Script Date: 11/28/2024 19:31:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Options](
	[OptionsID] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](100) NOT NULL,
	[FieldValue] [nvarchar](255) NOT NULL,
	[LongName] [nvarchar](255) NULL,
	[ShortName] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OptionsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Menus] ON 
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (2, N'Menu', N'Menu', N'Menu', CAST(N'2024-11-02T08:27:00.000' AS DateTime), N'System', CAST(N'2024-11-09T06:13:06.573' AS DateTime), N'System', 1, N'Menu', 1, 1006)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1002, N'User', N'User', N'User Management', CAST(N'2024-11-04T20:05:00.000' AS DateTime), N'System', CAST(N'2024-11-09T06:14:00.917' AS DateTime), N'System', 1, N'User', 2, 1006)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1003, N'UserAccess', N'UserAccess', N'User Access Management', CAST(N'2024-11-09T06:14:39.000' AS DateTime), N'System', CAST(N'2024-11-09T06:14:54.670' AS DateTime), N'System', 1, N'UserAccess', 3, 1006)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1004, N'Role', N'Role', N'Role Management', CAST(N'2024-11-09T06:15:29.157' AS DateTime), N'System', NULL, NULL, 1, N'Role', 4, 1006)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1005, N'Option', N'Option', N'Option Management', CAST(N'2024-11-09T06:16:38.427' AS DateTime), N'System', NULL, NULL, 1, N'Option', 5, 1006)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1006, N'Home', N'Administrator', N'Administrator', CAST(N'2024-11-09T06:30:31.993' AS DateTime), N'System', NULL, NULL, 1, N'Home', 1, NULL)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1009, N'tes', N'tes', N'Admin', CAST(N'2024-11-09T06:56:40.617' AS DateTime), N'System', NULL, NULL, 1, N'tes', 2, 1002)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1010, N'tes', N'1', N'Admin', CAST(N'2024-11-09T06:57:21.507' AS DateTime), N'System', NULL, NULL, 1, N'tes', 1, 1008)
GO
INSERT [dbo].[Menus] ([MenuID], [ControllerName], [MenuName], [MenuDesc], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active], [MenuURL], [MenuOrder], [ParentMenuID]) VALUES (1012, N'approval', N'approval', N'approval', CAST(N'2024-11-09T07:11:38.203' AS DateTime), N'System', NULL, NULL, 1, N'approval', 1, 1011)
GO
SET IDENTITY_INSERT [dbo].[Menus] OFF
GO
SET IDENTITY_INSERT [dbo].[Options] ON 
GO
INSERT [dbo].[Options] ([OptionsID], [FieldName], [FieldValue], [LongName], [ShortName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active]) VALUES (2004, N'tes4', N'tes4', N'tes4', N'tes4', CAST(N'2024-11-27T15:40:07.000' AS DateTime), NULL, CAST(N'2024-11-27T15:47:06.637' AS DateTime), N'alfiyan', 1)
GO
INSERT [dbo].[Options] ([OptionsID], [FieldName], [FieldValue], [LongName], [ShortName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Active]) VALUES (2005, N'tes1', N'tes1', N'tes1', N'tes1', CAST(N'2024-11-27T15:47:32.477' AS DateTime), N'alfiyan', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Options] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Admin', CAST(N'2024-10-30T20:31:24.000' AS DateTime), NULL, CAST(N'2024-10-30T20:32:08.967' AS DateTime), NULL)
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Customer', CAST(N'2024-10-30T21:00:05.000' AS DateTime), NULL, CAST(N'2024-11-03T18:00:32.057' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[UserAccess] ON 
GO
INSERT [dbo].[UserAccess] ([UserAccessID], [RoleID], [MenuID], [Views], [Inserts], [Edits], [Deletes], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, 1, 1, 0, 0, 0, 0, CAST(N'2024-10-06T10:22:20.413' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[UserAccess] ([UserAccessID], [RoleID], [MenuID], [Views], [Inserts], [Edits], [Deletes], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, 1, 4, 1, 1, 1, 1, CAST(N'2024-11-02T10:38:11.567' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[UserAccess] ([UserAccessID], [RoleID], [MenuID], [Views], [Inserts], [Edits], [Deletes], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, 1, 2, 1, 1, 1, 1, CAST(N'2024-11-02T10:40:10.113' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[UserAccess] ([UserAccessID], [RoleID], [MenuID], [Views], [Inserts], [Edits], [Deletes], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1002, 3, 1002, 1, 1, 0, 0, CAST(N'2024-11-04T20:46:06.000' AS DateTime), NULL, CAST(N'2024-11-27T15:12:53.757' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[UserAccess] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [RoleID], [CompanyName], [Phone], [FullName], [EmailAddress], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'alfiyan', N'AQAAAAEAACcQAAAAELpFWO5Hj3xiavhQ/6ySyrzE92ciHIgm6oIQ7tHOCaKOry8TLS4/3ly+3TwvU12TPw==', 1, N'PT udin sejahtera', N'08159538096', N'jaelani', N'alfiyan.jaelani@gmail.com', CAST(N'2024-11-03T18:23:47.180' AS DateTime), N'alfiyan', NULL, NULL)
GO
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [RoleID], [CompanyName], [Phone], [FullName], [EmailAddress], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'adi', N'AQAAAAEAACcQAAAAEBof0sffCD+9LsVBbmG5uJWZyO12pgegf72iBUaBQJmfMxVY1h10nrPD1xbanCvz6Q==', 3, N'PT Adi Contraction', N'08159538096', N'adi atmaja', N'adi@gmail.com', CAST(N'2024-11-27T15:23:59.020' AS DateTime), N'adi', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Menus] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Options] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Options] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ((0)) FOR [Views]
GO
ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ((0)) FOR [Inserts]
GO
ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ((0)) FOR [Edits]
GO
ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ((0)) FOR [Deletes]
GO
ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "r"
            Begin Extent = 
               Top = 6
               Left = 249
               Bottom = 136
               Right = 419
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "uc"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "m"
            Begin Extent = 
               Top = 6
               Left = 457
               Bottom = 136
               Right = 631
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_UserAccess'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_UserAccess'
GO
