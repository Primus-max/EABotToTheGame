using EABotToTheGame.Models;

namespace EABotToTheGame.Services
{
    public class ManualMode : IBotMode
    {
        private readonly IWebDriver _webDriver;
        private readonly ITelegramBotClient _telegramBotClient;

        public ManualMode(ITelegramBotClient telegramBotClient)
        {
            //_webDriver = webDriver;
            _telegramBotClient = telegramBotClient;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!)
        {
            // Общий код для скриншота и входа на сайты
            // ...

            // Логика для запроса данных у хозяина через бота
            // ...
        }
    }
}
