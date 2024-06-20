
create or alter procedure dbo.RecipeGet(@RecipeId int = 0, @All bit = 0, @RecipeName  varchar(100) = '')
as 
begin 

	select @RecipeName = nullif(@RecipeName, '')
	select r.RecipeId, r.CuisineID, r.UsersId, r.RecipeName, r.Calories, r.Datedrafted, r.DatePublished, r.DateArchived, r.RecipeStatus, r.RecipePicture
	from Recipe r
	where r.RecipeId = @RecipeId 
	or @All = 1
	or r.RecipeName like '%' + @RecipeName + '%'
	order by r.recipename, r.datedrafted, r.datepublished, r.datearchived, r.recipestatus, r.calories, r.recipepicture
end
go

exec RecipeGet @RecipeName = '' 
exec RecipeGet @RecipeName = 'ea'

exec RecipeGet @All = 1

declare @id int
select top 1 @id = r.recipeId from Recipe r 
exec RecipeGet @recipeId = @id