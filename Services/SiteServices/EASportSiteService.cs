using OpenQA.Selenium.Support.UI;

namespace EABotToTheGame.Services.SiteServices
{
    public class EASportSiteService
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EASportSiteService(IWebDriver webDriver)
        {
            _driver = webDriver;
            _wait = new(_driver, TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Общие метод авторизации
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public bool Authorizations(string email, string password)
        {
            SetEmailToInput(email);
            SetPasswordToInput(password);
            SubmitAuthForm();

            return IsAuth(); // Проверку на успешную(или нет) авторизацию
        }


        /// <summary>
        /// Отправить код на email
        /// </summary>
        /// <returns></returns>
        public bool SendCodeOnEmail()
        {
            try
            {
                IWebElement sendCodeButton = _wait.Until(e => e.FindElement(By.Id("btnSendCode")));

                IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                executor.ExecuteScript("arguments[0].click();", sendCodeButton);
                //sendCodeButton.Click();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Общий метод для проверки кода и авторизации
        /// </summary>
        /// <param name="code"></param>
        public bool SubmitCodeAuthorizations(string code)
        {
            SetCodeToInput(code); // Вставляю код
            SubmitCode(); // Отправляю код

            return IsInvalidSecurityCodeMessagePresent();
        }

        // Вставляю email  в поле
        private void SetEmailToInput(string email)
        {
            try
            {
                IWebElement emailInput = _wait.Until(e => e.FindElement(By.Id("email")));
                emailInput.Clear();
                Thread.Sleep(1500);
                ClearAndEnterText(emailInput, email);
            }
            catch (Exception)
            {

            }
        }

        // Вставляю пароль в поле
        private void SetPasswordToInput(string password)
        {
            try
            {
                IWebElement passwordInput = _wait.Until(e => e.FindElement(By.Id("password")));
                passwordInput.Clear();
                Thread.Sleep(1500);
                ClearAndEnterText(passwordInput, password);
            }
            catch (Exception)
            {

            }
        }

        // Подтверждаю отправку кода на почту
        private void SubmitAuthForm()
        {
            try
            {
                IWebElement formElement = _wait.Until(e => e.FindElement(By.Id("login-form")));
                Thread.Sleep(1500);

                // Попробуйте использовать метод Submit()
                try
                {
                    formElement.Submit();
                }
                catch (Exception)
                {
                    // Если Submit() не сработал, используйте JavaScript для нажатия кнопки
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", formElement);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось нажать кнопку авторизации {ex.Message}");
            }
        }



        // Вставляю код для отправи
        private void SetCodeToInput(string code)
        {
            try
            {
                IWebElement setCodeInput = _wait.Until(e => e.FindElement(By.Id("twoFactorCode")));

                // Очищаем поле ввода
                //IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                //executor.ExecuteScript("arguments[0].value = '';", setCodeInput);

                ClearAndEnterText(setCodeInput, code);

                // Задержка, если это необходимо
                Thread.Sleep(1000);

                // Вводим текст
                // executor.ExecuteScript($"arguments[0].value = '{code.Trim()}';", setCodeInput);
            }
            catch (Exception)
            {
                // Обработка исключений
            }
        }

        // Подтверждаю отправку формы с кодом
        private void SubmitCode()
        {
            try
            {
                // <a role="button" class="otkbtn otkbtn-primary  zero-margin" href="javascript:void(0);" id="btnSubmit" style="float:right;max-width:100%;">Sign in</a>
                IWebElement submitCodeBtn = _wait.Until(e => e.FindElement(By.Id("btnSubmit")));
                Thread.Sleep(1500);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
                executor.ExecuteScript("arguments[0].click();", submitCodeBtn);
                //submitCodeBtn.Click();
            }
            catch (Exception)
            {

            }
        }

        // Проверяю подошёл код или нет
        private bool IsInvalidSecurityCodeMessagePresent()
        {
            try
            {
                // Найти элемент по тексту
                IWebElement errorElement = _wait.Until(e => e.FindElement(By.XPath("//p[@class='otkc otkinput-errormsg' and contains(text(), 'The security code you entered is invalid')]")));

                // Проверить наличие элемента
                return errorElement != null;
            }
            catch (Exception)
            {
                // Если элемент не найден, вернуть false
                return false;
            }
        }


        // Метод ожидания загрузки страницы после того как отправили код и авторизовались
        public bool WaitingDownLoadPage()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(500));
            try
            {
                IWebElement navBarCurrency = wait.Until(e => e.FindElement(By.CssSelector("div.view-navbar-currency")));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Закрываю всплывшее окно
        public void CloseFuckingPopup()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(7));
            try
            {
                IWebElement popUp = wait.Until(e => e.FindElement(By.CssSelector("div.ut-livemessage")));
                var closeButton = popUp.FindElement(By.CssSelector("button.btn-standard.call-to-action"));

                // Добавляем обработчик события нажатия клавиши Escape
                _driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);

                // Ждем, чтобы убедиться, что всплывающее окно закрылось
                Thread.Sleep(1000);

                // Пытаемся кликнуть на кнопку закрытия, если она еще видима
                if (IsElementVisible(closeButton))
                {
                    closeButton.Click();
                }
            }
            catch (Exception)
            {
                // Обработка ошибки
            }
        }

        private bool IsElementVisible(IWebElement element)
        {
            try
            {
                return element.Displayed && element.Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Метод вставки текста в текстовые поля
        private static void ClearAndEnterText(IWebElement element, string text)
        {
            Random random = new Random();
            // Используем JavaScriptExecutor для выполнения JavaScript-кода
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)((IWrapsDriver)element).WrappedDriver;

            // Очищаем поле ввода с помощью JavaScript
            jsExecutor.ExecuteScript("arguments[0].value = '';", element);
            // Установить стиль display элемента в block
            jsExecutor.ExecuteScript("arguments[0].style.display = 'block';", element);
            // Вставляем текст по одному символу без изменений
            foreach (char letter in text)
            {
                if (letter == '\b')
                {
                    // Если символ является символом backspace, удаляем последний введенный символ
                    element.SendKeys(Keys.Backspace);
                }
                else
                {
                    // Вводим символ
                    element.SendKeys(letter.ToString());
                }

                Thread.Sleep(random.Next(50, 150));  // Добавляем небольшую паузу между вводом каждого символа
            }
            Thread.Sleep(random.Next(300, 700));
        }


        // Проверка успешной авторизации для получени кода подтверждения
        private bool IsAuth()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            try
            {
                IWebElement parentDiv = wait.Until(e => e.FindElement(By.XPath("//div[@id='online-general-error']")));

                // Вы можете использовать parentDiv по вашему усмотрению
                // Например, получить p элемент внутри parentDiv
                IWebElement pElement = parentDiv.FindElement(By.XPath(".//p[@class='otkinput-errormsg otkc']"));

                // Затем получить текст из p элемента
                string authErrorText = pElement.Text;
                bool isWrongText = authErrorText.Contains("Your credentials are incorrect or have expired. Please try again or reset your password");
                if (isWrongText)
                    return false;
                return true;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }


    }
}
