using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Data;

public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : DbContext(options)
{
	public DbSet<Url> Urls { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Role> Roles { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Url>()
			.HasOne(url => url.CreatedByUser)
			.WithMany()
			.HasForeignKey(url => url.CreatedByUserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<User>()
			.HasOne(user => user.Role)
			.WithMany()
			.HasForeignKey(user => user.RoleId)
			.IsRequired();

		modelBuilder.Entity<Role>().HasData(
			new Role { Id = Role.UserRole.User, Name = "User" },
			new Role { Id = Role.UserRole.Admin, Name = "Admin" }
		);
	}
}