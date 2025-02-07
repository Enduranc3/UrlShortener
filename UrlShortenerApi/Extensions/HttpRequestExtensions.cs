using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using UrlShortenerApi.Logic.Authentication;

namespace UrlShortenerApi.Extensions;

public static class HttpRequestExtensions
{
	public static int TryGetUserId(this HttpRequest request)
	{
		var authorization = request.Headers.Authorization.FirstOrDefault();
		if (string.IsNullOrEmpty(authorization))
		{
			throw new InvalidOperationException("No authorization header present");
		}

		var token = authorization.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

		try
		{
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);

			var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId);
			if (userIdClaim == null)
			{
				throw new InvalidOperationException("No user ID claim present in token");
			}

			return int.Parse(userIdClaim.Value);
		}
		catch (ArgumentException ex)
		{
			throw new InvalidOperationException($"Invalid token format: {ex.Message}");
		}
		catch (SecurityTokenException ex)
		{
			throw new InvalidOperationException($"Invalid token: {ex.Message}");
		}
	}

	public static int TryGetUserRoleId(this HttpRequest request)
	{
		var jwtRaw = request.Headers.Authorization;
		var jwtToken = jwtRaw.First()?[7..];

		var jwtHandler = new JwtSecurityTokenHandler();
		var securityToken = (JwtSecurityToken)jwtHandler.ReadToken(jwtToken);
		var id = securityToken.Claims.FirstOrDefault(existing
			=> existing.Type.Equals(CustomClaimTypes.Role))?.Value;

		return int.Parse(id ?? throw new InvalidOperationException());
	}
}