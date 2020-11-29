/*
Bereitstellungsskript für Gaestebuch

Dieser Code wurde von einem Tool generiert.
Änderungen an dieser Datei führen möglicherweise zu falschem Verhalten und gehen verloren, falls
der Code neu generiert wird.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Gaestebuch"
:setvar DefaultFilePrefix "Gaestebuch"
:setvar DefaultDataPath "C:\Users\Sebastian\AppData\Local\Microsoft\VisualStudio\SSDT\Gaestebuch"
:setvar DefaultLogPath "C:\Users\Sebastian\AppData\Local\Microsoft\VisualStudio\SSDT\Gaestebuch"

GO
:on error exit
GO
/*
Überprüfen Sie den SQLCMD-Modus, und deaktivieren Sie die Skriptausführung, wenn der SQLCMD-Modus nicht unterstützt wird.
Um das Skript nach dem Aktivieren des SQLCMD-Modus erneut zu aktivieren, führen Sie folgenden Befehl aus:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'Der SQLCMD-Modus muss aktiviert sein, damit dieses Skript erfolgreich ausgeführt werden kann.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'[dbo].[FK_Eintraege_Admin] wird gelöscht....';


GO
ALTER TABLE [dbo].[Eintraege] DROP CONSTRAINT [FK_Eintraege_Admin];


GO
PRINT N'Das erneute Erstellen der Tabelle "[dbo].[Eintraege]" wird gestartet....';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Eintraege] (
    [Id]             INT        IDENTITY (1, 1) NOT NULL,
    [IP]             NCHAR (32) NOT NULL,
    [Name]           NCHAR (40) NULL,
    [Besuchsdatum]   DATE       NOT NULL,
    [Kommentar]      TEXT       NOT NULL,
    [Autorisiert_ID] INT        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Eintraege])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Eintraege] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Eintraege] ([Id], [IP], [Name], [Besuchsdatum], [Kommentar], [Autorisiert_ID])
        SELECT   [Id],
                 [IP],
                 [Name],
                 [Besuchsdatum],
                 [Kommentar],
                 [Autorisiert_ID]
        FROM     [dbo].[Eintraege]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Eintraege] OFF;
    END

DROP TABLE [dbo].[Eintraege];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Eintraege]', N'Eintraege';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'[dbo].[FK_Eintraege_Admin] wird erstellt....';


GO
ALTER TABLE [dbo].[Eintraege] WITH NOCHECK
    ADD CONSTRAINT [FK_Eintraege_Admin] FOREIGN KEY ([Autorisiert_ID]) REFERENCES [dbo].[Admin] ([Id]);


GO
PRINT N'Vorhandene Daten werden auf neu erstellte Einschränkungen hin überprüft.';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Eintraege] WITH CHECK CHECK CONSTRAINT [FK_Eintraege_Admin];


GO
PRINT N'Update abgeschlossen.';


GO
