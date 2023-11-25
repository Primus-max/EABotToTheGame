using System.Net.Sockets;

namespace EABotToTheGame.Managers
{
    public class WebDriverManager
    {
        private Dictionary<WhoIAm, IWebDriver> _drivers = new Dictionary<WhoIAm, IWebDriver>();

        public IWebDriver GetDriver(WhoIAm whoIam)
        {
            if (!_drivers.ContainsKey(whoIam))
            {
                _drivers[whoIam] = CreateDriver(whoIam);
            }

            return _drivers[whoIam];
        }

        // Создаю драйвер
        private IWebDriver CreateDriver(WhoIAm whoIam)
        {
            //int port = GetPortForRole();

            if (true)
            {
                try
                {
                    var options = new ChromeOptions();
                    options.AddArgument("--silent");
                    options.AddArgument("--disable-notifications");
                    options.AddArgument("--disable-extensions");
                    options.AddArgument("--disable-extensions-file-access-check");
                    options.AddArgument("--disable-extensions-http-throttling");
                    options.AddArgument($"--remote-debugging-port={9222}");
                    options.AddArgument("--disable-popup-blocking");

                    var service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    using var driver = new ChromeDriver(service, options);

                    return driver;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось создать драйвер {ex.Message}");
                    return null;
                }
            }
            else
            {
               // Console.WriteLine($"Порт {port} уже используется");
                return null;
            }

        }

       
    }
}
