using UrlShortenerApi.Exceptions;
using UrlShortenerApi.Logic.Authentication;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.Requests;
using UrlShortenerApi.Repositories;
using UrlShortenerApi.Services.Helpers;

namespace UrlShortenerApi.Services;

public class AuthService(IUserRepository userRepository, TokenGenerator tokenGenerator) : IAuthService
{
	public async Task<User?> LoginAsync(string email, string password)
	{
		var user = await userRepository.GetByEmailAsync(email);
		if (user == null)
		{
			throw new UserNotFoundException();
		}

		return SecurityHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt) ? user : null;
	}

	public async Task<string> GenerateToken(UserLoginRequest userLoginRequest)
	{
		var user = await LoginAsync(userLoginRequest.Email, userLoginRequest.Password);
		if (user == null)
		{
			throw new UserNotFoundException();
		}

		var token = tokenGenerator.GenerateTokenForSuccessLoginResult(new SuccessLoginEntity(user.Id, user.Email,
			user.Role.Name));
		return token;
	}

	public async Task<User?> RegisterAsync(UserRegisterRequest userRegisterRequest)
	{
		var user = await userRepository.GetByEmailAsync(userRegisterRequest.Email);
		if (user != null)
		{
			throw new UserAlreadyExistsException();
		}

		var role = await GetRoleUserAsync();
		var salt = SecurityHelper.GenerateSalt();
		var passwordHash = SecurityHelper.HashPassword(userRegisterRequest.Password, salt);

		var newUser = new User
		{
			Email = userRegisterRequest.Email.ToLower(),
			Username = userRegisterRequest.Username,
			PasswordHash = passwordHash,
			PasswordSalt = salt,
			Role = role
		};

		await userRepository.AddAsync(newUser);
		return newUser;
	}

	public async Task<User?> GetUserAsync(int id)
	{
		return await userRepository.GetByIdAsync(id);
	}


	public async Task<Role> GetRoleUserAsync()
	{
		return await userRepository.GetRoleUserAsync();
	}
}