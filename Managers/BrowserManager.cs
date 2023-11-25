using Newtonsoft.Json;
using System.Net.Sockets;

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
            List<int> lounchedPorts = new List<int>();


            _browserPaths = BrowserPaths();
            if (_browserPaths.Count == 0)
            {
                Console.WriteLine($"Не удалось получить пути для браузеров");
                return lounchedPorts;
            }

            foreach (string browserPath in _browserPaths)
            {
                int openedPort = GetOpenedPort();
                if (openedPort == 0)
                {
                    Console.WriteLine("Не найден не один доступный порт для открытия на нём браузера, проверь это");
                    return lounchedPorts;
                }

                try
                {
                    // Параметры командной строки для запуска Chrome с флагом --remote-debugging-port=9222
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
                    Thread.Sleep(1000);

                    if (IsBrowserLounch()) // Проверка запущен браузер или нет
                    {
                        lounchedPorts.Add(openedPort);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при запуске Chrome: {ex.Message}");
                }
            }
            return lounchedPorts;
        }

        // Проверяю запустился ли браузер
        private bool IsBrowserLounch()
        {
            return true;
        }

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

        // Поучаю свободный порт
        private int GetOpenedPort()
        {
            int startingPort = 5000; // Начальный порт для проверки

            while (startingPort < 65535) // Проверяем порты до максимального значения
            {
                if (IsPortAvailable(startingPort)) // Проверяем доступность порта
                {
                    return startingPort; // Возвращаем свободный порт
                }
                else
                {
                    startingPort++; // Увеличиваем порт и проверяем следующий
                }
            }

            Console.WriteLine("Не удалось найти свободный порт"); // Если свободный порт не найден
            return 0;
        }

        // Проверяю доступность порта
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
    }
}
