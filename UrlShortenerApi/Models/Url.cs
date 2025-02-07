using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UrlShortenerApi.Models;

public class Url
{
	public int Id { get; set; }

	[MaxLength(1024)] public required string OriginalUrl { get; set; }

	[MaxLength(255)] public required string ShortUrl { get; set; }

	public int CreatedByUserId { get; set; }

	public DateTime CreatedDate { get; set; }

	[MaxLength(255)] public required string Description { get; set; }

	public int ClickCount { get; set; }

	[JsonIgnore] public User? CreatedByUser { get; set; }
}