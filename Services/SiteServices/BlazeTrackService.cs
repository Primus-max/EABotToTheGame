using OpenQA.Selenium.Support.UI;
namespace EABotToTheGame.Services.SiteServices
{
    public class BlazeTrackService
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public BlazeTrackService(IWebDriver webDriver)
        {
            _driver = webDriver;
            _wait = new(_driver, TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Метод получения регистрационных данных
        /// </summary>
        /// <param name="blazeUrl"></param>
        /// <returns></returns>
        public AuthData GetAuthData()
        {
            OpenOrderCard(); // Котрываю карточку
            AuthData authData = new()
            {
                Email = GetEmail(),
                Password = GetPassword(),
            };

            return authData;
        }

        /// <summary>
        /// Метод принятия заказа
        /// </summary>
        /// <param name="screenShotPath"></param>
        public void ConfirmOrder(string status, string screenShotPath = null!)
        {
            // Если есть путь, значит выгружаем скрин
            if (screenShotPath != null)
                UploadScreenShot(screenShotPath); // Выгружаю скриншот

            Thread.Sleep(1000);
            SelectStatus(status); // Выбираю статус
            Thread.Sleep(1000);
            ConfirmOrder(); // Подтверждаю заказ
        }

        // Открываю карточку 
        private void OpenOrderCard()
        {
            try
            {
                // Карточка с данными
                IWebElement orderCard = _wait.Until(e => e.FindElement(By.CssSelector("table.table-striped.table-bordered")));

                // Кнопка редактирования
                IWebElement editButton = orderCard.FindElement(By.CssSelector("a.table-action-link"));

                // Получаем значение атрибута href
                string hrefValue = editButton.GetAttribute("href");

                // Если значение атрибута не пустое, выполняем скрипт для перехода по ссылке в текущей вкладке
                if (!string.IsNullOrEmpty(hrefValue))
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"window.location.href = '{hrefValue}';");
                }
            }
            catch (Exception)
            {

            }
        }


        // Получаю email
        private string GetEmail()
        {
            try
            {
                IWebElement emailInput = _wait.Until(e => e.FindElement(By.Id("orders-name")));

                string email = emailInput.GetAttribute("value");

                return email;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // Получаю пароль
        private string GetPassword()
        {
            try
            {
                IWebElement passworsInput = _wait.Until(e => e.FindElement(By.Id("orders-p")));

                string password = passworsInput.GetAttribute("value");

                return password;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // Выбираю статус
        public void SelectStatus(string status)
        {
            string statusText = status.Trim();
            try
            {
                IWebElement dropdownToggle = _wait.Until(e => e.FindElement(By.CssSelector(".select__header.init-select")));
                dropdownToggle.Click();

                // Задержка, чтобы дать выпадающему списку полностью отобразиться
                Thread.Sleep(500);

                // Теперь выбираем элемент по тексту
                IWebElement statusItem = _wait.Until(e => e.FindElement(By.XPath($"//div[@class='select__item' and text()='{statusText}']")));
                statusItem.Click();
            }
            catch (Exception)
            {
                // Обработка исключений
            }
        }


        // Выгружаю скриншот
        private void UploadScreenShot(string screenShotPath)
        {
            try
            {
                IWebElement uploadInput = _wait.Until(e => e.FindElement(By.Id("w1")));

                // Преобразовываем относительный путь в абсолютный
                string absolutePath = Path.GetFullPath(screenShotPath);

                uploadInput.SendKeys(absolutePath);
            }
            catch (Exception)
            {
                // Обработка исключений
            }
        }


        // Принимаю заказ
        private void ConfirmOrder()
        {
            try
            {
                IWebElement confirmButton = _wait.Until(e => e.FindElement(By.CssSelector("button.btn.btn-primary")));

                Thread.Sleep(500);
                confirmButton.Click();
            }
            catch (Exception)
            {

            }
        }
    }
}
