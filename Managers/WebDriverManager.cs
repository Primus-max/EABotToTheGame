using System.Runtime.InteropServices;

public class WebDriverManager
{
    private Dictionary<WhoIAm, IWebDriver> _drivers = new Dictionary<WhoIAm, IWebDriver>();

    public WebDriverManager()
    {
        CreateRemoteDrivers();
        OpenAndArrangeBrowsers();
    }

    public IWebDriver GetDriver(WhoIAm whoIam)
    {
        if (_drivers.ContainsKey(whoIam))
        {
            return _drivers[whoIam];
        }

        Console.WriteLine($"Драйвер для {whoIam} не найден.");
        return null;
    }

    private void CreateRemoteDrivers()
    {
        BrowserManager browserManager = new BrowserManager();
        List<int> ports = browserManager.Launch();

        if (ports.Count > 0)
        {
            foreach (var port in ports)
            {
                try
                {
                    var options = new ChromeOptions();
                    options.AddArgument("--silent");
                    options.AddArgument("--disable-notifications");
                    options.AddArgument("--disable-extensions");
                    options.AddArgument("--disable-extensions-file-access-check");
                    options.AddArgument("--disable-extensions-http-throttling");
                    options.DebuggerAddress = $"localhost:{port}";
                    options.AddArgument("--disable-popup-blocking");

                    var service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true; // Скрыть окно командной строки драйвера Chrome

                    var driver = new ChromeDriver(service, options);

                    _drivers[(WhoIAm)ports.IndexOf(port)] = driver;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось создать драйвер {ex.Message}");
                    // Логирование ошибки или продолжение итерации
                }
            }
        }
        else
        {
            Console.WriteLine("Не найдены открытые порты для браузеров");
        }
    }

    public void OpenAndArrangeBrowsers()
    {
        // Получаем размер экрана
        int screenWidth = GetSystemMetrics(0);  // SM_CXSCREEN
        int screenHeight = GetSystemMetrics(1); // SM_CYSCREEN

        // Получаем список драйверов
        var drivers = _drivers.Values.ToList();

        for (int i = 0; i < drivers.Count; i++)
        {
            // Устанавливаем позиции и размеры окон для каждого браузера
            try
            {
                int windowWidth = screenWidth / drivers.Count;

                // Первый браузер
                if (i == 0)
                {
                    windowWidth -= 30; // Уменьшаем ширину на 30 пикселей
                }
                // Второй браузер
                else if (i == 1)
                {
                    int additionalWidth = 30; // Увеличиваем ширину на 30 пикселей
                    windowWidth += additionalWidth;

                    int previousWidth = screenWidth / drivers.Count - 30;
                    int offsetX = i * previousWidth; // Смещение вправо относительно предыдущего окна

                    windowWidth = Math.Min(windowWidth, screenWidth - offsetX); // Не даем окну выходить за границы экрана
                }

                int windowX = i * (screenWidth / drivers.Count) + (i == 1 ? -30 : 0); // Смещение вправо для второго окна
                int windowY = 0; // Позиция Y

                drivers[i].Manage().Window.Position = new System.Drawing.Point(windowX, windowY);
                drivers[i].Manage().Window.Size = new System.Drawing.Size(windowWidth, screenHeight - 10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось установить размер окна {ex.Message}");
            }
        }
    }


    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex); // Индексы для размеров окна
}

