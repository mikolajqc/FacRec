
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2018 20:56:33
-- Generated from EDMX file: D:\Studia\Inzynierka\Server\Models\FaceRecognitionModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FaceRecognitionDatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AverageVectors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AverageVectors];
GO
IF OBJECT_ID(N'[dbo].[EigenFaces]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EigenFaces];
GO
IF OBJECT_ID(N'[dbo].[Wages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Wages];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AverageVectors'
CREATE TABLE [dbo].[AverageVectors] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Value] varchar(max)  NOT NULL
);
GO

-- Creating table 'EigenFaces'
CREATE TABLE [dbo].[EigenFaces] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Value] varchar(max)  NOT NULL
);
GO

-- Creating table 'Wages'
CREATE TABLE [dbo].[Wages] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Value] varchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'AverageVectors'
ALTER TABLE [dbo].[AverageVectors]
ADD CONSTRAINT [PK_AverageVectors]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'EigenFaces'
ALTER TABLE [dbo].[EigenFaces]
ADD CONSTRAINT [PK_EigenFaces]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Wages'
ALTER TABLE [dbo].[Wages]
ADD CONSTRAINT [PK_Wages]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------