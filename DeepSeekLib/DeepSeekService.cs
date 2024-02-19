using DeepSeekLib;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace BotUI
{
	public class DeepSeekService : IDisposable
	{
		private const string _baseUrl = "https://api.deepseek.com/v1/chat/completions";

		private readonly string _apiKey;

		public string model { get; private set; } = DeepSeekModels.Chat;

		private HttpClient _httpClient;

		public DeepSeekService(string deepSeekApiKey)
		{
			_apiKey = deepSeekApiKey;

			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
		}

		public void Dispose()
		{
			this._httpClient.Dispose();
		}

		public void SetModel(string model)
		{
			this.model = model;
		}

		public async Task<string> SendMessage(string message)
		{
			var requestBody = new
			{
				model = this.model,
				messages = new[]
				{
					new { content = message, role = "user" }
				}
			};

			string jsonRequest = JsonSerializer.Serialize(requestBody);

			var response = await _httpClient.PostAsync(_baseUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
			}

			var jsonResult = JObject.Parse(await response.Content.ReadAsStringAsync());
			var answer = (string)jsonResult["choices"]![0]!["message"]!["content"]!;

			return answer;
		}
	}
}
