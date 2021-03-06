/****** Сценарий для команды SelectTopNRows среды SSMS  ******/
SELECT 
	b.Id,
	b.Name,
	a.Name as Author,
	g.Name as Genre,
	b.Raiting,
	b.HasElectronic,
	p.Name as Publisher,
	b.Published
FROM [dbo].[Book] b
LEFT JOIN [dbo].[BookGenre] bg on b.[Id]=bg.[BookId]
LEFT JOIN [dbo].[Genre] g on bg.[GenreId]=g.[Id]
LEFT JOIN [dbo].[BookAuthor] ba ON b.[Id]=ba.[BookId]
LEFT JOIN [dbo].[Author] a ON ba.[AuthorId]=a.[Id]
LEFT JOIN [dbo].[Publisher] p ON b.[PublisherId]=p.[Id]
ORDER BY b.[Name], a.[Name]

select * from [dbo].[Author] a
select * from [dbo].[Genre] g 