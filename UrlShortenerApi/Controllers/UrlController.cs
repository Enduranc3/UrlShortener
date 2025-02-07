using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Extensions;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.Requests;
using UrlShortenerApi.Services;

namespace UrlShortenerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController(IServiceProvider serviceProvider, IUrlShortenerService urlShortenerService) : Controller
{
	/// <summary>
	///     Shortens a given URL.
	/// </summary>
	/// <param name="shortenRequest">The request containing the original URL and description.</param>
	/// <returns>The shortened URL or an error message.</returns>
	[HttpPost("shorten")]
	[ProducesResponseType(typeof(Url), 200)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[Authorize]
	public async Task<IActionResult> Shorten([FromBody] ShortenRequest shortenRequest)
	{
		if (shortenRequest.IsNull())
		{
			return BadRequest("Missing parameters in body.");
		}

		int userId;
		try
		{
			userId = Request.TryGetUserId();
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Error getting user ID.");
		}

		var user = await serviceProvider.GetService<IAuthService>()?.GetUserAsync(userId)!;

		if (user == null)
		{
			return BadRequest("User does not exist.");
		}

		if (await urlShortenerService.ExistsByOriginalUrlAsync(shortenRequest.OriginalUrl))
		{
			return BadRequest("URL already shortened.");
		}

		var url = await urlShortenerService.CreateShortUrlAsync(shortenRequest.OriginalUrl, shortenRequest.Description,
			userId);

		if (url == null)
		{
			return BadRequest("Failed to shorten URL.");
		}


		return Ok(url.ShortUrl);
	}

	/// <summary>
	///     Deletes a shortened URL.
	/// </summary>
	/// <param name="shortUrl">The shortened URL to delete.</param>
	/// <returns>OK if deleted, or an error message.</returns>
	[HttpDelete("{shortUrl}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[Authorize]
	public async Task<IActionResult> Delete(string shortUrl)
	{
		if (string.IsNullOrEmpty(shortUrl))
		{
			return BadRequest("Missing parameters in query.");
		}

		var url = await urlShortenerService.GetUrlAsync(shortUrl);

		if (url == null)
		{
			return NotFound("URL not found.");
		}

		int userId;

		try
		{
			userId = Request.TryGetUserId();
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Error getting user ID.");
		}

		var user = await serviceProvider.GetService<IAuthService>()?.GetUserAsync(userId)!;

		if (url.CreatedByUserId != Request.TryGetUserId() && user?.Role.Name != "Admin")
		{
			return Forbid();
		}

		await urlShortenerService.DeleteUrlAsync(shortUrl);

		return Ok();
	}

	/// <summary>
	///     Gets all URLs created by the authenticated user.
	/// </summary>
	/// <returns>A list of URLs created by the user.</returns>
	[HttpGet("/url/my")]
	[ProducesResponseType(typeof(IEnumerable<Url>), 200)]
	[Authorize]
	public async Task<IActionResult> GetMyUrls()
	{
		int userId;
		try
		{
			userId = Request.TryGetUserId();
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Error getting user ID.");
		}

		var urls = await urlShortenerService.GetUrlsAsync(userId);

		return Ok(urls);
	}

	/// <summary>
	///     Gets information about a specific shortened URL.
	/// </summary>
	/// <param name="shortUrl">The shortened URL to get information about.</param>
	/// <returns>The URL information or an error message.</returns>
	[HttpGet("{shortUrl}/info")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[Authorize]
	public async Task<IActionResult> GetUrlInfo(string shortUrl)
	{
		if (string.IsNullOrEmpty(shortUrl))
		{
			return BadRequest("Missing parameters in query.");
		}

		int userId;

		try
		{
			userId = Request.TryGetUserId();
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Error getting user ID.");
		}

		var url = await urlShortenerService.GetUrlAsync(shortUrl);

		if (url == null)
		{
			return NotFound("URL not found.");
		}

		if (url.CreatedByUserId != userId &&
		    (await serviceProvider.GetService<IAuthService>()?.GetUserAsync(userId)!)?.Role.Name != "Admin")
		{
			return Forbid();
		}

		return Ok(url);
	}

	/// <summary>
	///     Gets all shortened URLs. Only accessible by admin users.
	/// </summary>
	/// <returns>A list of all shortened URLs.</returns>
	[HttpGet("all")]
	[ProducesResponseType(typeof(IEnumerable<Url>), 200)]
	[Authorize]
	public async Task<IActionResult> GetAllUrls()
	{
		int userId;

		try
		{
			userId = Request.TryGetUserId();
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Error getting user ID.");
		}

		var user = await serviceProvider.GetService<IAuthService>()?.GetUserAsync(userId)!;

		if (user == null || user.Role.Name != "Admin")
		{
			return Forbid();
		}

		var urls = await urlShortenerService.GetAllUrlsAsync();

		return Ok(urls);
	}
}