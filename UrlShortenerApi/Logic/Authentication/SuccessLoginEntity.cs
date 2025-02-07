namespace UrlShortenerApi.Logic.Authentication;

public class SuccessLoginEntity(int id, string email, string role)
{
	public string Email { get; init; } = email;
	public int Id { get; init; } = id;
	public string Role { get; init; } = role;
}