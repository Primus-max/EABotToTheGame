using OpenQA.Selenium.Remote;

namespace EABotToTheGame.Managers
{
    public class WebDriverManager
    {
        private IWebDriver _driver = null!;
        public IWebDriver GetDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--silent");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-extensions-file-access-check");
            options.AddArgument("--disable-extensions-http-throttling");
            options.DebuggerAddress = "localhost:9222";

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true; // Скрыть окно командной строки драйвера Chrome

            _driver = new ChromeDriver(service, options);
           

            return _driver;
        }
    }
}
