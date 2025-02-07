using Moq;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories;
using UrlShortenerApi.Services;

namespace UrlShortenerApiTests.Services;

[TestFixture]
public class UrlShortenerServiceTests
{
    private Mock<IUrlRepository> _urlRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private UrlShortenerService _urlShortenerService;

    [SetUp]
    public void Setup()
    {
        _urlRepositoryMock = new Mock<IUrlRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IUserRepository)))
            .Returns(_userRepositoryMock.Object);

        _urlShortenerService = new UrlShortenerService(_urlRepositoryMock.Object, _serviceProviderMock.Object);
    }

    [Test]
    public async Task CreateShortUrlAsync_ValidInput_ReturnsUrl()
    {
        // Arrange
        const string originalUrl = "https://example.com";
        const string description = "Test URL";
        const int userId = 1;
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Username = "testuser",
            PasswordHash = "hash",
            PasswordSalt = "salt",
            RoleId = Role.UserRole.User,
            Role = new Role { Id = Role.UserRole.User, Name = "User" }
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _urlRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Url>()))
            .ReturnsAsync((Url url) => url);

        // Act
        var result = await _urlShortenerService.CreateShortUrlAsync(originalUrl, description, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.OriginalUrl, Is.EqualTo(originalUrl));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.CreatedByUserId, Is.EqualTo(userId));
            Assert.That(result.ShortUrl, Does.Contain("mydomain.com/"));
        });
    }

    [Test]
    public async Task GetUrlAsync_ExistingUrl_ReturnsUrl()
    {
        // Arrange
        var shortUrl = "mydomain.com/abc123";
        var url = new Url
        {
            ShortUrl = shortUrl,
            OriginalUrl = "https://example.com",
            Description = "Test URL"
        };

        _urlRepositoryMock.Setup(x => x.FindByShortUrlAsync(shortUrl))
            .ReturnsAsync(url);

        // Act
        var result = await _urlShortenerService.GetUrlAsync(shortUrl);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ShortUrl, Is.EqualTo(url.ShortUrl));
            Assert.That(result.OriginalUrl, Is.EqualTo(url.OriginalUrl));
        });
    }

    [Test]
    public async Task GetUrlAsync_NonExistingUrl_ReturnsNull()
    {
        // Arrange
        const string shortUrl = "mydomain.com/nonexistent";

        _urlRepositoryMock.Setup(x => x.FindByShortUrlAsync(shortUrl))
            .ReturnsAsync((Url?)null);

        // Act
        var result = await _urlShortenerService.GetUrlAsync(shortUrl);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task IncrementClickCountAsync_ExistingUrl_IncreasesCount()
    {
        // Arrange
        const string shortUrl = "mydomain.com/abc123";
        var url = new Url
        {
            ShortUrl = shortUrl,
            ClickCount = 0,
            OriginalUrl = "https://example.com",
            Description = "Test URL"
        };

        _urlRepositoryMock.Setup(x => x.FindByShortUrlAsync(shortUrl))
            .ReturnsAsync(url);

        // Act
        await _urlShortenerService.IncrementClickCountAsync(shortUrl);

        // Assert
        Assert.That(url.ClickCount, Is.EqualTo(1));
        _urlRepositoryMock.Verify(x => x.UpdateAsync(url), Times.Once);
    }

    [Test]
    public async Task DeleteUrlAsync_ExistingUrl_CallsDelete()
    {
        // Arrange
        const string shortUrl = "mydomain.com/abc123";
        var url = new Url
        {
            ShortUrl = shortUrl,
            OriginalUrl = "https://example.com",
            Description = "Test URL"
        };

        _urlRepositoryMock.Setup(x => x.FindByShortUrlAsync(shortUrl))
            .ReturnsAsync(url);

        // Act
        await _urlShortenerService.DeleteUrlAsync(shortUrl);

        // Assert
        _urlRepositoryMock.Verify(x => x.DeleteAsync(url), Times.Once);
    }

    [Test]
    public async Task GetUrlsAsync_ReturnsUserUrls()
    {
        // Arrange
        var userId = 1;
        var urls = new List<Url>
        {
            new() {
                ShortUrl = "mydomain.com/abc",
                CreatedByUserId = userId,
                OriginalUrl = "https://example1.com",
                Description = "First test URL"
            },
            new() {
                ShortUrl = "mydomain.com/def",
                CreatedByUserId = userId,
                OriginalUrl = "https://example2.com",
                Description = "Second test URL"
            }
        };

        _urlRepositoryMock.Setup(x => x.GetAllByUserIdAsync(userId))
            .ReturnsAsync(urls);

        // Act
        var result = await _urlShortenerService.GetUrlsAsync(userId);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(urls.Count));
            Assert.That(result, Is.All.Matches<Url>(url => url.CreatedByUserId == userId));
        });
    }

    [Test]
    public async Task ExistsByOriginalUrlAsync_ExistingUrl_ReturnsTrue()
    {
        // Arrange
        const string originalUrl = "https://example.com";

        _urlRepositoryMock.Setup(x => x.ExistsByOriginalUrlAsync(originalUrl))
            .ReturnsAsync(true);

        // Act
        var result = await _urlShortenerService.ExistsByOriginalUrlAsync(originalUrl);

        // Assert
        Assert.That(result, Is.True);
    }
}
