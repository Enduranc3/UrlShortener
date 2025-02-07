using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using UrlShortenerApi.Exceptions;

namespace UrlShortenerApi.Logic.Authentication;

public class TokenGenerator(Options options)
{
	public string GenerateTokenForSuccessLoginResult(SuccessLoginEntity loginEntity)
	{
		var handler = new JwtSecurityTokenHandler();
		var key = options.GetKeyBytes();

		var credentials = new SigningCredentials(
			new SymmetricSecurityKey(key),
			SecurityAlgorithms.HmacSha256);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = GenerateClaims(loginEntity),
			Expires = DateTime.UtcNow.AddMinutes(15),
			SigningCredentials = credentials
		};

		var token = handler.CreateToken(tokenDescriptor);

		return handler.WriteToken(token);
	}

	private ClaimsIdentity GenerateClaims(SuccessLoginEntity loginEntity)
	{
		var claims = new ClaimsIdentity();

		claims.AddClaim(new Claim(CustomClaimTypes.UserId, loginEntity.Id.ToString()));
		claims.AddClaim(new Claim(CustomClaimTypes.Email, loginEntity.Email));
		claims.AddClaim(new Claim(CustomClaimTypes.Role, loginEntity.Role));

		return claims;
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];

		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(randomNumber);
		}

		return Convert.ToBase64String(randomNumber);
	}

	public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(options.GetKeyBytes()),
			ValidateLifetime = false
		};
		var tokenHandler = new JwtSecurityTokenHandler();

		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
		    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
			    StringComparison.InvariantCultureIgnoreCase))
		{
			throw new InvalidTokenException();
		}

		return principal;
	}
}