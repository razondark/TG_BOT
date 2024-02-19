using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using BotLib;
using DeepSeekLib;

namespace BotUI
{
	public class Bot
	{
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly DeepSeekService _deepSeek;

        public Bot(string telegramToken, DeepSeekService deepSeek)
        {
            _telegramBotClient = new TelegramBotClient(telegramToken);
            _deepSeek = deepSeek;
		}

        public string Start()
        {
			var cts = new CancellationTokenSource();
			var cancelllatinToken = cts.Token;

			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = { },
				ThrowPendingUpdates = false
			};

			_telegramBotClient.StartReceiving(
				HandlerUpdateAsync,
				HandleErrorAsync,
				receiverOptions,
				cancelllatinToken
			);

			return $"Started bot [{_telegramBotClient.GetMeAsync().Result.FirstName}]";
		}

		private async void ExecCommand(string command, ITelegramBotClient bot, ChatId chatId)
		{
			command = command.ToLower().Substring(1);

			if (Enum.TryParse(command, out BotCommands.Commands commandsEnum))
			{
				switch (commandsEnum) 
				{
					case BotCommands.Commands.start:
						await BotCommands.StartCommand(bot, chatId);
						break;

					case BotCommands.Commands.changemodel:
						await BotCommands.ChangeModelCommand(bot, chatId);
						break;

					case BotCommands.Commands.week:
						await BotCommands.GetWeekNumber(bot, chatId);
						break;

					default:
						break;
				}
			}
			else
			{
				await bot.SendTextMessageAsync(chatId, "Error command");
			}
		}

		private async void TextMessageHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update?.Type != UpdateType.Message)
            {
                return;
            }
			if (update.Message?.Text == null || update.Message.Text == String.Empty)
			{
				return;
			}

			var chatId = update.Message.Chat;
            var message = update.Message.Text;

			if (message.StartsWith('/')) // command
			{
				ExecCommand(message, botClient, chatId);
				return;
			}

			await botClient.SendTextMessageAsync(chatId, await _deepSeek.SendMessage(message));
		}

		private void CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			if (update?.Type != UpdateType.CallbackQuery)
			{
				return;
			}

			var data = update.CallbackQuery?.Data;
			if (data!.Contains("chat", StringComparison.CurrentCultureIgnoreCase))
			{
				_deepSeek.SetModel(DeepSeekModels.Chat);
				return;
			}
			else if (data!.Contains("coder", StringComparison.CurrentCultureIgnoreCase))
			{
				_deepSeek.SetModel(DeepSeekModels.Coder);
				return;
			}
		}

		private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
			var message = update.Message;

            if (update?.Type == UpdateType.Message)
            {
				TextMessageHandler(botClient, update, cancellationToken);
            }
			else if (update?.Type == UpdateType.CallbackQuery)
			{
				CallbackQueryHandler(botClient, update, cancellationToken);
			}
            else
            {
				await botClient.SendTextMessageAsync(message!.Chat, "Error command");
			}
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
		}
    }
}
