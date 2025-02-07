using System.Text;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories;

namespace UrlShortenerApi.Services;

public class UrlShortenerService(IUrlRepository urlRepository, IServiceProvider serviceProvider) : IUrlShortenerService
{
	public async Task<Url?> CreateShortUrlAsync(string originalUrl, string description, int userId)
	{
		var url = new Url
		{
			OriginalUrl = originalUrl,
			ShortUrl = GenerateShortUrl(),
			CreatedByUserId = userId,
			CreatedDate = DateTime.UtcNow,
			Description = description,
			ClickCount = 0,
			CreatedByUser = serviceProvider.GetService<IUserRepository>()?.GetByIdAsync(userId).Result ??
			                throw new InvalidOperationException()
		};

		return await urlRepository.AddAsync(url);
	}

	public async Task<Url?> GetUrlAsync(string shortUrl)
	{
		return await urlRepository.FindByShortUrlAsync(shortUrl);
	}

	public async Task<IEnumerable<Url?>> GetUrlsAsync(int userId)
	{
		return await urlRepository.GetAllByUserIdAsync(userId);
	}

	public async Task<IEnumerable<Url?>> GetAllUrlsAsync()
	{
		return await urlRepository.GetAllAsync();
	}

	public async Task IncrementClickCountAsync(string shortUrl)
	{
		var url = await urlRepository.FindByShortUrlAsync(shortUrl);
		if (url != null)
		{
			url.ClickCount++;
			await urlRepository.UpdateAsync(url);
		}
	}

	public async Task DeleteUrlAsync(string shortUrl)
	{
		var url = await urlRepository.FindByShortUrlAsync(shortUrl);
		if (url != null)
		{
			await urlRepository.DeleteAsync(url);
		}
	}

	public async Task UpdateUrlAsync(Url? url)
	{
		if (url != null)
		{
			await urlRepository.UpdateAsync(url);
		}
	}

	public Task<bool> ExistsByOriginalUrlAsync(string shortenRequestOriginalUrl)
	{
		return urlRepository.ExistsByOriginalUrlAsync(shortenRequestOriginalUrl);
	}

	public async Task<Url?> GetShortUrlByIdAsync(string url)
	{
		return await urlRepository.FindByUrlAsync(url);
	}

	private static string GenerateShortUrl()
	{
		var random = new Random();
		var number = random.NextInt64(1, long.MaxValue);
		return $"mydomain.com/{ToBase62(number)}";
	}

	private static string ToBase62(long number)
	{
		const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		var result = new StringBuilder();
		while (number > 0)
		{
			result.Insert(0, chars[(int)(number % 62)]);
			number /= 62;
		}

		return result.ToString();
	}
}