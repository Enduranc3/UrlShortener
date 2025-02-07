using System.Security;

namespace UrlShortenerApi.Extensions;

public static class StringExtensions
{
	public static SecureString ToReadonlySecureString(this string input)
	{
		var secureString = new SecureString();
		foreach (var c in input)
		{
			secureString.AppendChar(c);
		}

		secureString.MakeReadOnly();
		return secureString;
	}
}