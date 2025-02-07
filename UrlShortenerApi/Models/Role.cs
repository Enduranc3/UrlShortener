using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class Role
{
	public enum UserRole
	{
		User = 1,
		Admin
	}

	public UserRole Id { get; set; }

	[MaxLength(100)] public required string Name { get; set; }
}