namespace EABotToTheGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            // Создание контейнера DI и регистрация зависимостей
            var serviceProvider = new ServiceCollection()
                
                .AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("6844658250:AAGPSNBy3Opa6zrW_5Yyn58JyFDpSdcoRg0"))
                
                .AddLogging()
                .BuildServiceProvider();

            

            // Создание главного класса и вызов начального метода
            var botController = serviceProvider.GetRequiredService<TelegramBotController>();
            await botController.StartBotAsync();


            Console.WriteLine($"НЕ нажимайте любую кнопку! Это остановит работу бота");
            Console.ReadKey();

        }
    }
}