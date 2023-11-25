namespace EABotToTheGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string token = GetToken();
            // Создание контейнера DI и регистрация зависимостей
            var serviceProvider = new ServiceCollection()                
                .AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(token)) // Токен для бота
                .AddSingleton<TelegramBotController>()
                .AddSingleton<WebDriverManager>() // Открываю браузеры на портах и подключа к ним драйвера. Созданю один экземпляр который хранит список
                .AddSingleton<AutoMode>() // Режим работы
                .AddSingleton<ManualMode>() // Режим работы
                .AddScoped<IInlineKeyboardProvider, СhoiceModeInlineKeyboardProvider>() // Кнопки
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

        public static string GetToken()
        {
            string filePath = "config.json";

            try
            {
                // Чтение содержимого файла
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Десериализация JSON в объект
                var config = JsonConvert.DeserializeObject<Config>(jsonContent);

                return config?.TelegramToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading config file: {ex.Message}");
            }
            return string.Empty;
        }
    }
}