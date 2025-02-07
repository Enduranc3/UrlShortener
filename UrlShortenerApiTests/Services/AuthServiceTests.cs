using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories;

namespace UrlShortenerApiTests.Services;

[TestFixture]
public class UrlRepositoryTests
{
    private DbContextOptions<UrlShortenerDbContext> _options;
    private UrlShortenerDbContext _context;
    private UrlRepository _repository;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
            .UseInMemoryDatabase(databaseName: $"UrlShortenerTest_{Guid.NewGuid()}")
            .Options;
        _context = new UrlShortenerDbContext(_options);
        _repository = new UrlRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddAsync_ValidUrl_AddsToDatabase()
    {
        // Arrange
        var url = new Url
        {
            OriginalUrl = "https://example.com",
            ShortUrl = "mydomain.com/abc",
            CreatedByUserId = 1,
            Description = "Test URL"
        };

        // Act
        var result = await _repository.AddAsync(url);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OriginalUrl, Is.EqualTo(url.OriginalUrl));
        var savedUrl = await _context.Urls.FindAsync(result.Id);
        Assert.That(savedUrl, Is.Not.Null);
    }

    [Test]
    public async Task FindByShortUrlAsync_ExistingUrl_ReturnsUrl()
    {
        // Arrange
        var url = new Url
        {
            OriginalUrl = "https://example.com",
            ShortUrl = "mydomain.com/abc",
            CreatedByUserId = 1,
            Description = "Test find URL"
        };
        await _context.Urls.AddAsync(url);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindByShortUrlAsync(url.ShortUrl);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OriginalUrl, Is.EqualTo(url.OriginalUrl));
    }

    [Test]
    public async Task GetAllByUserIdAsync_ReturnsUserUrls()
    {
        // Arrange
        const int userId = 1;
        var urls = new List<Url>
        {
            new() {
                OriginalUrl = "https://example1.com",
                ShortUrl = "mydomain.com/abc",
                CreatedByUserId = userId,
                Description = "First test URL"
            },
            new() {
                OriginalUrl = "https://example2.com",
                ShortUrl = "mydomain.com/def",
                CreatedByUserId = userId,
                Description = "Second test URL"
            }
        };
        await _context.Urls.AddRangeAsync(urls);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByUserIdAsync(userId);

        // Assert
        Assert.That(result.Count, Is.EqualTo(urls.Count));
        Assert.That(result, Is.All.Matches<Url>(url => url.CreatedByUserId == userId));
    }

    [Test]
    public async Task DeleteAsync_ExistingUrl_RemovesFromDatabase()
    {
        // Arrange
        var url = new Url
        {
            OriginalUrl = "https://example.com",
            ShortUrl = "mydomain.com/abc",
            CreatedByUserId = 1,
            Description = "URL to delete"
        };
        await _context.Urls.AddAsync(url);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(url);

        // Assert
        var deletedUrl = await _context.Urls.FindAsync(url.Id);
        Assert.That(deletedUrl, Is.Null);
    }
}
