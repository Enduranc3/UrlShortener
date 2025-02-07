using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models.Requests;

public class UserLoginRequest : INullCheck
{
	[Required] [EmailAddress] public required string Email { get; set; }

	[Required] public required string Password { get; set; }

	public bool IsNull()
	{
		return string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password);
	}
}