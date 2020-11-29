CREATE TABLE [dbo].[Admin]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NCHAR(40) NULL, 
    [Vorname] NCHAR(40) NULL, 
    [Password] NCHAR(256) NULL
)
