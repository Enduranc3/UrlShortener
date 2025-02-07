using DotNetEnv;

namespace UrlShortenerApi;

public static class Program
{
	public static void Main(string[] args)
	{
		Env.Load();

		var builder = WebApplication.CreateBuilder(args);

		var startup = new Startup();
		startup.ConfigureServices(builder.Services);

		var app = builder.Build();
		Startup.Configure(app, app.Environment);

		try
		{
			app.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}