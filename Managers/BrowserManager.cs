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
            _browserPaths = BrowserPaths();
            List<int> lounchedPorts = new List<int>();
            foreach (string browserPath in _browserPaths)
            {
                int openedPort = GetOpenedPort();

                try
                {
                    // Параметры командной строки для запуска Chrome с флагом --remote-debugging-port=9222
                    string chromeOptions = $"--remote-debugging-port={openedPort}";

                    // Создаем процесс для запуска Chrome
                    ProcessStartInfo psi = new ProcessStartInfo
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

        }

        // Проверяю запустился ли браузер
        private bool IsBrowserLounch()
        {
            return true;
        }

        // Получаю пути из файла
        private List<string> BrowserPaths()
        {
            return new List<string>();
            // ЛОгика получения путей
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
