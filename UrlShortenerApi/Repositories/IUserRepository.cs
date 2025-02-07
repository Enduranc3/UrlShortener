﻿using UrlShortenerApi.Models;

namespace UrlShortenerApi.Repositories;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(int id);
	Task<User?> GetByEmailAsync(string email);
	Task<User?> AddAsync(User user);
	Task UpdateAsync(User user);
	Task DeleteAsync(User user);
	Task<Role> GetRoleUserAsync();
}