CREATE TABLE [dbo].[Eintraege]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [IP] NCHAR(32) NOT NULL, 
    [Name] NCHAR(40) NULL, 
    [Besuchsdatum] DATE NOT NULL, 
    [Kommentar] TEXT NOT NULL, 
    [Autorisiert_ID] INT NULL, 
    CONSTRAINT [FK_Eintraege_Admin] FOREIGN KEY ([Autorisiert_ID]) REFERENCES [Admin]([ID])
)
