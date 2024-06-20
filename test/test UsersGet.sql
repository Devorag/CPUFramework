declare @id int 
select top 1 @id = u.UsersId from Users u 

exec UsersGet @UsersId = @id