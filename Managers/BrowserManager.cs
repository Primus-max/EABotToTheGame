namespace EABotToTheGame.Managers
{
    public class BrowserManager
    {
        List<string> _browserPaths;

        public BrowserManager()
        {
            _browserPaths = new List<string>();
        }

        // Запуск браузера
        public List<int> Launch()
        {
            List<int> predefinedPorts = GetPredefinedPorts(); // Получаем заранее определённые порты
            List<int> launchedPorts = new List<int>();

            _browserPaths = BrowserPaths();
            if (_browserPaths.Count == 0) // Если не получили список путей браузеров
            {
                Console.WriteLine($"Не удалось получить пути для браузеров");
                return launchedPorts;
            }

            foreach (string browserPath in _browserPaths)
            {
                if (predefinedPorts.Count == 0)
                {
                    Console.WriteLine("Не заданы заранее определённые порты для RemoteWebDriver");
                    return launchedPorts;
                }

                int openedPort = predefinedPorts[0]; // Берём первый порт из списка
                predefinedPorts.RemoveAt(0); // Удаляем использованный порт из списка

                try
                {
                    // Параметры командной строки для запуска Chrome с флагом --remote-debugging-port=port
                    string chromeOptions = $"--remote-debugging-port={openedPort}";

                    // Создаем процесс для запуска Chrome
                    ProcessStartInfo psi = new()
                    {
                        FileName = browserPath,
                        Arguments = chromeOptions,
                        UseShellExecute = true
                    };

                    // Запускаем процесс
                    Process.Start(psi);
                    Thread.Sleep(5000);

                    if (IsBrowserLaunched(openedPort)) // Проверка запущен браузер или нет
                    {
                        launchedPorts.Add(openedPort);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при запуске Chrome: {ex.Message}");
                }
            }
            return launchedPorts;
        }

        // Получаем заранее определённые порты
        private List<int> GetPredefinedPorts()
        {
            return new List<int> { 9222, 4444 }; // ХАРДКОД!!!
        }


        // Проверяю запустился ли браузер
        private bool IsBrowserLaunched(int port)
        {
            try
            {
                // Создаем URL для проверки, например, "http://localhost:port/json"
                string url = $"http://localhost:{port}/json";

                // Создаем запрос и отправляем его
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000; // Устанавливаем таймаут запроса

                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Если ответ получен, считаем, что браузер запущен успешно
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException)
            {
                // Если возникает исключение, то браузер не запущен
                return false;
            }
        }

        // Получаю пути браузеров
        private List<string> BrowserPaths()
        {
            string filePath = "config.json";

            try
            {
                // Чтение содержимого файла
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Десериализация JSON в объект
                var config = JsonConvert.DeserializeObject<Config>(jsonContent);

                return config?.BrowserPaths;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading config file: {ex.Message}");
            }
            return new List<string>();
        }

        // Поучаю свободный порт для RemoteWebDriver
        private int GetOpenedPort()
        {
            int startingPort = 5000; // Начальный порт для проверки

            while (startingPort < 65535) // Проверяем порты до максимального значения
            {
                if (IsPortAvailable(startingPort) && IsPortAvailableForRemoteWebDriver(startingPort))
                {
                    return startingPort; // Возвращаем свободный порт
                }

                startingPort++; // Увеличиваем порт и проверяем следующий
            }

            Console.WriteLine("Не удалось найти свободный порт для RemoteWebDriver"); // Если свободный порт не найден
            return 0;
        }

        // Проверяю, что порт доступен
        private bool IsPortAvailable(int port)
        {
            try
            {
                using TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("localhost", port); // Пытаемся подключиться к порту
                return false; // Если подключение успешно, порт занят
            }
            catch (SocketException)
            {
                return true; // Если возникает исключение, порт свободен
            }
        }

        // Проверяю, что порт доступен и подходит для RemoteWebDriver
        private bool IsPortAvailableForRemoteWebDriver(int port)
        {
            try
            {
                var uri = new Uri($"http://localhost:{port}/status");
                using var client = new WebClient();
                string response = client.DownloadString(uri);
                return response.Contains("\"ready\":true"); // Проверяем, что порт доступен для RemoteWebDriver
            }
            catch (WebException)
            {
                return false; // Если возникает исключение, порт не подходит
            }
        }

    }
}
