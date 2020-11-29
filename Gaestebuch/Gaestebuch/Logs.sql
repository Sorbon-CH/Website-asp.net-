CREATE TABLE [dbo].[Logs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Admin_ID] INT NULL, 

    [Aktion] NCHAR(50) NULL, 
    [Zeit] DATETIME NULL DEFAULT getdate(), 
    [IP] NCHAR(32) NULL, 
    CONSTRAINT [FK_Logs_Admin] FOREIGN KEY ([Admin_ID]) REFERENCES [Admin]([ID])
)
