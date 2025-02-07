using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class User
{
	public int Id { get; set; }

	[MaxLength(255)] [EmailAddress] public required string Email { get; set; }

	[MaxLength(100)] public required string Username { get; set; }

	public required string PasswordHash { get; set; }

	public required string PasswordSalt { get; set; }

	public Role.UserRole RoleId { get; set; }

	public required Role Role { get; set; }
}