using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BotUI
{
    internal class Program
    {
		public static async Task Main(string[] args)
		{
            try
			{
				var host = Host.CreateDefaultBuilder(args)
					.ConfigureServices((hostContext, services) =>
					{
						services.AddHostedService<BotConfig>();
					})
					.Build();

				await host.RunAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}