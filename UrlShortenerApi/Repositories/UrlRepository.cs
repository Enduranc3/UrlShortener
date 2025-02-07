using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Repositories;

public class UrlRepository(UrlShortenerDbContext context) : IUrlRepository
{
	public async Task<Url?> GetByIdAsync(int id)
	{
		return await context.Urls.FindAsync(id);
	}

	public async Task<List<Url>> GetAllByUserIdAsync(int userId)
	{
		return await context.Urls.Where(u => u.CreatedByUserId == userId).ToListAsync();
	}

	public async Task<Url?> FindByUrlAsync(string url)
	{
		return await context.Urls.FirstOrDefaultAsync(u => u.OriginalUrl == url);
	}

	public async Task<List<Url?>> GetAllAsync()
	{
		return await context.Urls.ToListAsync();
	}

	public async Task<Url?> AddAsync(Url url)
	{
		var entry = await context.Urls.AddAsync(url);
		await context.SaveChangesAsync();
		return entry.Entity;
	}

	public async Task UpdateAsync(Url url)
	{
		context.Urls.Update(url);
		await context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Url url)
	{
		context.Urls.Remove(url);
		await context.SaveChangesAsync();
	}

	public async Task<Url?> FindByShortUrlAsync(string shortUrl)
	{
		return await context.Urls.FirstOrDefaultAsync(url => url.ShortUrl == Uri.UnescapeDataString(shortUrl));
	}

	public async Task<bool> ExistsByOriginalUrlAsync(string originalUrl)
	{
		return await context.Urls.AnyAsync(url => url.OriginalUrl == originalUrl);
	}
}