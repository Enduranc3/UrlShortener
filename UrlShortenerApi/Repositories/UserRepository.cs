using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Repositories;

public class UserRepository(UrlShortenerDbContext context) : IUserRepository
{
	public async Task<User?> GetByIdAsync(int id)
	{
		return await context.Users
			.Include(u => u.Role)
			.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await context.Users
			.Include(u => u.Role)
			.FirstOrDefaultAsync(user => user.Email == email);
	}

	public async Task<User?> AddAsync(User user)
	{
		if (user.RoleId == 0)
		{
			user.RoleId = Role.UserRole.User;
		}

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();
		return await GetByIdAsync(user.Id);
	}

	public async Task UpdateAsync(User user)
	{
		context.Users.Update(user);
		await context.SaveChangesAsync();
	}

	public async Task DeleteAsync(User user)
	{
		context.Users.Remove(user);
		await context.SaveChangesAsync();
	}

	public async Task<Role> GetRoleUserAsync()
	{
		return await context.Roles.FirstAsync(role => role.Id == Role.UserRole.User);
	}
}