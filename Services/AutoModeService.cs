namespace EABotToTheGame.Services
{
    public class AutoMode : IBotMode
    {
        private readonly IWebDriver _webDriver;

        public AutoMode(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public async Task ExecuteAsync()
        {
            // Общий код для скриншота и входа на сайты
            // ...

            // Логика для автоматического получения данных
            // ...
        }
    }
}
