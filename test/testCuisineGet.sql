declare @id int
select top 1 @id = c.CuisineId from cuisine c 
exec CuisineGet @cuisineId = @id 