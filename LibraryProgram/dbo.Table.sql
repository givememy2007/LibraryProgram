CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [BookName] NVARCHAR(50) NOT NULL, 
    [AutorName] NVARCHAR(50) NOT NULL, 
    [PublisherName] NVARCHAR(50) NOT NULL, 
    [PublishingYear] NVARCHAR(50) NOT NULL, 
    [Availability] INT NOT NULL DEFAULT 1
)
