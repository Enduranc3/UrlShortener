namespace UrlShortenerApi.Models.Requests;

public class ShortenRequest : INullCheck
{
	public required string OriginalUrl { get; set; }
	public required string Description { get; set; }

	public bool IsNull()
	{
		return string.IsNullOrEmpty(OriginalUrl);
	}
}