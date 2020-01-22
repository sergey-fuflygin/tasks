USE [test]
GO

/****** Object: Table [dbo].[ArticleTag] Script Date: 22/01/2020 12:40:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ArticleTag] (
    [Id]         INT NOT NULL,
    [Article_Id] INT NOT NULL,
    [Tag_Id]     INT NOT NULL
);


