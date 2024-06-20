use RecipeDB 
go 

create or alter procedure dbo.CuisineGet(@CuisineId int = 0, @All bit = 0, @CuisineType  varchar(300) = '')
as 
begin 

	select @CuisineType = nullif(@CuisineType, '')
	select c.CuisineId, c.CuisineType
	from Cuisine c 
	where c.CuisineId = @CuisineId 
	or @All = 1
	or c.CuisineType like '%' + @CuisineType + '%'
	order by c.CuisineType
end
go

exec CuisineGet @CuisineType = '' 
exec CuisineGet @CuisineType = 'a'

exec CuisineGet @All = 1

declare @id int
select top 1 @id = c.CuisineId from Cuisine c
exec CuisineGet @CuisineId = @id 