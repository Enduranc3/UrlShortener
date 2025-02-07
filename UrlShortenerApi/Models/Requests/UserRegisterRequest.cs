using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models.Requests;

public class UserRegisterRequest : INullCheck
{
	[Required] [MaxLength(100)] public required string Username { get; set; }

	[Required]
	[MaxLength(100)]
	[EmailAddress]
	public required string Email { get; set; }

	[Required] [MaxLength(100)] public required string Password { get; set; }

	public bool IsNull()
	{
		return string.IsNullOrWhiteSpace(Username)
		       || string.IsNullOrWhiteSpace(Email)
		       || string.IsNullOrWhiteSpace(Password);
	}
}