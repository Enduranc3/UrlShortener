using UrlShortenerApi.Models;
using UrlShortenerApi.Models.Requests;

namespace UrlShortenerApi.Services;

public interface IAuthService
{
	Task<string> GenerateToken(UserLoginRequest userLoginRequest);
	Task<User?> RegisterAsync(UserRegisterRequest userRegisterRequest);
	Task<User?> LoginAsync(string email, string password);
	Task<User?> GetUserAsync(int id);
	Task<Role> GetRoleUserAsync();
}