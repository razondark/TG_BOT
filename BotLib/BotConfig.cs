using BotUI;
using Microsoft.Extensions.Hosting;

public class BotConfig : IHostedService
{
	private readonly DeepSeekService _deepSeek;
	private readonly Bot _bot;

	public BotConfig()
	{
		_deepSeek = new DeepSeekService(Environment.GetEnvironmentVariable("DEEPSEEK_TOKEN")!);
		_bot = new Bot(Environment.GetEnvironmentVariable("TG_TOKEN")!, _deepSeek);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine(_bot.Start());
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
