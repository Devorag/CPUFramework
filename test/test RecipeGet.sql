
declare @id int

select top 1 @id = r.recipeId from Recipe r 
order by r.RecipeName desc 

--select @id

exec RecipeGet @recipeId = @id