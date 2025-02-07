using UrlShortenerApi.Models;

namespace UrlShortenerApi.Services;

public interface IUrlShortenerService
{
	Task<Url?> CreateShortUrlAsync(string originalUrl, string description, int userId);
	Task<Url?> GetUrlAsync(string shortUrl);
	Task<IEnumerable<Url?>> GetUrlsAsync(int userId);
	Task<IEnumerable<Url?>> GetAllUrlsAsync();
	Task IncrementClickCountAsync(string shortUrl);
	Task DeleteUrlAsync(string shortUrl);
	Task UpdateUrlAsync(Url? url);
	Task<bool> ExistsByOriginalUrlAsync(string shortenRequestOriginalUrl);
}