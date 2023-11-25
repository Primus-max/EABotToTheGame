using EABotToTheGame.Buttons;
using EABotToTheGame.Handlers;

namespace EABotToTheGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
            // Создание контейнера DI и регистрация зависимостей
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IWebDriver>(provider => new ChromeDriver()) // Пример для Selenium WebDriver
                .AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("6926422184:AAEHQycwfIUMT7gSAxx5OFoj2h1isJPG3Lk")) // Токен для бота
                .AddSingleton<TelegramBotController>()
                .AddSingleton<WebDriverManager>() // Открываю браузеры на портах и подключа к ним драйвера. Созданю один экземпляр который хранит список
                .AddSingleton<AutoMode>() // Режим работы
                .AddSingleton<ManualMode>() // Режим работы
                .AddScoped<IInlineKeyboardProvider, DefaultInlineKeyboardProvider>() // Кнопки
                .AddScoped<HandleCallbackQuery>() // Перехватчик сообщений с CallBackQuery
                .AddScoped<HandleTextMessage>() // Перехватчик текстовых сообщений
                .AddScoped<AppModeManager>() // Хранитель состояний для режима приложения
                .AddScoped<UserStateManager>() // Хранитель состояний для юзеров
                .BuildServiceProvider();

            // Сразу создаю экземпляр для создания драйверов
            var webDriverManager = serviceProvider.GetRequiredService<WebDriverManager>(); 

            // Создание главного класса и вызов начального метода
            var botController = serviceProvider.GetRequiredService<TelegramBotController>();
            await botController.StartBotAsync();

            Console.WriteLine($"НЕ нажимайте любую кнопку! Это остановит работу бота");
            Console.ReadKey();

        }
    }
}