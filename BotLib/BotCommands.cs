using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLib
{
	public static class BotCommands
	{
		public enum Commands
		{
			start,
			changemodel,
			week
		}

		public async static Task<Message> StartCommand(ITelegramBotClient botClient, ChatId chatId)
		{
			var keyboardSelectModel = new InlineKeyboardMarkup(new[]
			{
				new[]
				{
					InlineKeyboardButton.WithCallbackData("DeepSeek Chat"),
					InlineKeyboardButton.WithCallbackData("DeepSeek Coder")
				}
			});

			return await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: "Hello\nI use DeepSeek API\nSelect model and write your question",
				replyMarkup: keyboardSelectModel);
		}

		public async static Task<Message> ChangeModelCommand(ITelegramBotClient botClient, ChatId chatId)
		{
			var keyboardSelectModel = new InlineKeyboardMarkup(new[]
			{
				new[]
				{
					InlineKeyboardButton.WithCallbackData("DeepSeek Chat"),
					InlineKeyboardButton.WithCallbackData("DeepSeek Coder")
				}
			});

			return await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: "Select model",
				replyMarkup: keyboardSelectModel);
		}

		private static string CalculateWeekNumber()
		{
			var currentDate = DateTime.Today;
			DateTime startSemesterDate;

			if (currentDate.Month > 8)
			{
				startSemesterDate = new DateTime(currentDate.Year, 9, 1);
			}
			else
			{
				startSemesterDate = new DateTime(currentDate.Year, 1, 4); // ???
			}

			// monday start
			while (startSemesterDate.DayOfWeek == DayOfWeek.Sunday)
			{
				startSemesterDate.AddDays(1);
			}

			var cal = new GregorianCalendar();
			var currentWeekNumber = cal.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			var startWeekNumber = cal.GetWeekOfYear(startSemesterDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

			if (Math.Abs(currentWeekNumber - startWeekNumber) % 2 == 1)
			{
				return "Числитель";
			}

			return "Знаменатель";
		}

		public async static Task<Message> GetWeekNumber(ITelegramBotClient botClient, ChatId chatId)
		{
			return await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: CalculateWeekNumber());
		}
	}
}
