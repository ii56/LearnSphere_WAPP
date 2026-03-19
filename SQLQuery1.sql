CREATE TABLE [dbo].[Syslog] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [userid]   INT           NOT NULL,
    [action]   NVARCHAR (50) NOT NULL,
    [dateTime] DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);