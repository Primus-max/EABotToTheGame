

namespace EABotToTheGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            // Создание контейнера DI и регистрация зависимостей
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IWebDriver>(provider => new ChromeDriver()) // Пример для Selenium WebDriver
                .AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("")) // Токен для бота
                .AddSingleton<IBotMode, AutoMode>() // Выберите режим, который нужно использовать
                .BuildServiceProvider();

            // Создание главного класса и вызов начального метода
            var botController = serviceProvider.GetRequiredService<TelegramBotController>();
            await botController.StartBotAsync();

            Console.WriteLine($"НЕ нажимайте любую кнопку! Это остановит работу бота");
            Console.ReadKey();

        }
    }
}