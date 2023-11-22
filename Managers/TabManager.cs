namespace EABotToTheGame.Managers
{
    /// <summary>
    /// Класс-менеджер сохранения открытых вкладок и переключения между ними
    /// </summary>
    public class TabManager
    {
        private readonly IWebDriver _driver;
        private readonly Dictionary<string, string> _tabs;

        public TabManager(IWebDriver driver)
        {
            if (driver == null) return;

            _driver = driver;
            _tabs = new Dictionary<string, string>();
        }

        /// <summary>
        /// Открываю и запоминаю вкладку
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="url"></param>
        public void OpenOrSwitchTab(string url)
        {
            // Извлечение названия сайта из URL
            try
            {
                Uri uri = new Uri(url);
                string siteName = uri.Host;

                // Если вкладка с таким именем уже существует, переключаемся на неё
                if (_tabs.ContainsKey(siteName))
                {
                    _driver.SwitchTo().Window(_tabs[siteName]);
                }
                else
                {
                    // Открываем новую вкладку через WebDriver
                    IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                    js.ExecuteScript("window.open();");

                    // Переключаемся на новую вкладку
                    _driver.SwitchTo().Window(_driver.WindowHandles.Last());

                    // Навигация по URL
                    _driver.Navigate().GoToUrl(url);

                    // Сохраняем вкладку
                    _tabs[siteName] = _driver.CurrentWindowHandle;
                }
            }
            catch (Exception)
            {
                // Обработка ошибки
            }
        }

        /// <summary>
        /// Переключаюсь между вкладками
        /// </summary>
        /// <param name="siteName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SwitchToTab(string siteName)
        {
            if (_tabs.TryGetValue(siteName, out var tab))
            {
                _driver.SwitchTo().Window(tab);
            }
            else
            {
                throw new InvalidOperationException($"Tab for site '{siteName}' not found.");
            }
        }
    }

}
