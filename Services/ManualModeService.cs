namespace EABotToTheGame.Services
{
    public class ManualMode : IBotMode
    {
        private readonly IWebDriver _webDriver;
        private readonly ITelegramBotClient _telegramBotClient;

        public ManualMode(IWebDriver webDriver, ITelegramBotClient telegramBotClient)
        {
            _webDriver = webDriver;
            _telegramBotClient = telegramBotClient;
        }

        public async Task ExecuteAsync()
        {
            // Общий код для скриншота и входа на сайты
            // ...

            // Логика для запроса данных у хозяина через бота
            // ...
        }
    }
}
