using UrlShortenerApi.Models;

namespace UrlShortenerApi.Repositories;

public interface IUrlRepository
{
	Task<Url?> GetByIdAsync(int id);
	Task<List<Url>> GetAllByUserIdAsync(int userId);
	Task<List<Url?>> GetAllAsync();
	Task<Url?> AddAsync(Url url);
	Task UpdateAsync(Url url);
	Task DeleteAsync(Url url);
	Task<Url?> FindByShortUrlAsync(string shortUrl);
	Task<bool> ExistsByOriginalUrlAsync(string originalUrl);
	Task<Url?> FindByUrlAsync(string url);
}