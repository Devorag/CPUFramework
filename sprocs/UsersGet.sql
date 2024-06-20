
create or alter procedure dbo.UsersGet(@UsersId int = 0, @All bit = 0, @UsersName varchar(100) = '')
as  
begin 

	select @UsersName = nullif(@UsersName, '')
	select u.UserName, u.FirstName, u.FirstName, u.UserName
	from Users u 
	where u.UsersId = @UsersId 
	or @All = 1
	or u.UserName like '%' + @UsersName + '%'
	order by u.LastName, u.FirstName, u.UserName
end
go

exec UsersGet @UsersName = '' 
exec UsersGet @UsersName = 'a'

exec UsersGet @All = 1

declare @id int 
select top 1 @id = u.UsersId from Users u 
exec UsersGet @UsersId = @id