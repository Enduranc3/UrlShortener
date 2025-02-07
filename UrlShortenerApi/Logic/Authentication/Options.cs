using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace UrlShortenerApi.Logic.Authentication;

public class Options
{
	private SecureString? _privateSecureKey;

	public SecureString PrivateSecureKey
	{
		get => _privateSecureKey ?? throw new InvalidOperationException("Secret key not set");
		set => _privateSecureKey = value;
	}

	private string ConvertSecureStringToString(SecureString secureString)
	{
		var unmanagedString = IntPtr.Zero;
		try
		{
			unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
			return Marshal.PtrToStringUni(unmanagedString) ?? string.Empty;
		}
		finally
		{
			Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
		}
	}

	public byte[] GetKeyBytes()
	{
		var keyString = ConvertSecureStringToString(PrivateSecureKey);
		var keyBytes = Encoding.UTF8.GetBytes(keyString);

		if (keyBytes.Length < 32)
		{
			Array.Resize(ref keyBytes, 32);
		}

		return keyBytes;
	}
}