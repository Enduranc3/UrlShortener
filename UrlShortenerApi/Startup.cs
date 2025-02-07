using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UrlShortenerApi.Data;
using UrlShortenerApi.Extensions;
using UrlShortenerApi.Filters;
using UrlShortenerApi.Logic.Authentication;
using UrlShortenerApi.Repositories;
using UrlShortenerApi.Services;

namespace UrlShortenerApi;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.WithOrigins("http://localhost:3000") // Add your React frontend URL
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
			});
		});

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddRouting(options => options.LowercaseUrls = true);
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Your API",
				Version = "v1"
			});

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Token (without Bearer prefix)",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			c.OperationFilter<AuthOperationFilter>();
		});


		services.AddDbContext<UrlShortenerDbContext>(options =>
			options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"))
				.UseSnakeCaseNamingConvention());

		var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY")!.ToReadonlySecureString();

		var authOptions = new Options
		{
			PrivateSecureKey = secretKey
		};

		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(authOptions.GetKeyBytes()),
					ClockSkew = TimeSpan.Zero
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var token = context.Request.Headers["Authorization"].FirstOrDefault();
						if (!string.IsNullOrEmpty(token))
						{
							// Ensure we're using a clean token string
							token = token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
							// Validate token format
							if (token.Count(c => c == '.') == 2) // JWT has 3 parts separated by dots
							{
								context.Token = token;
							}
						}

						return Task.CompletedTask;
					},
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Append("Token-Expired", "true");
						}

						return Task.CompletedTask;
					}
				};
			});

		services.AddAuthorization();

		services.AddScoped<IUrlShortenerService, UrlShortenerService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IUrlRepository, UrlRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<TokenGenerator>();

		services.AddSingleton(authOptions).AddSingleton<TokenGenerator>();
	}

	public static void Configure(WebApplication app, IWebHostEnvironment env)
	{
		app.UseCors();

		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortenerApi v1");
				options.RoutePrefix = string.Empty;
				options.ConfigObject.DisplayRequestDuration = true;
			});
		}

		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
	}
}