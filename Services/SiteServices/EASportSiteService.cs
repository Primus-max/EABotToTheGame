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
            SubmitAuthFrom();

            return WrongAuth(); // Проверку на успешную(или нет) авторизацию
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
                sendCodeButton.Click();

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
        public void SubmitCodeAuthorizations(string code)
        {
            SetCodeToInput(code); // Вставляю код
            SubmitCode(); // Отправляю код
        }

        // Вставляю email  в поле
        private void SetEmailToInput(string email)
        {
            try
            {
                IWebElement emailInput = _wait.Until(e => e.FindElement(By.Id("email")));
                emailInput.Clear();
                Thread.Sleep(500);
                emailInput.SendKeys(email);
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
                Thread.Sleep(500);
                passwordInput.SendKeys(password);
            }
            catch (Exception)
            {

            }
        }

        // Подтверждаю отправку кода на почту
        private void SubmitAuthFrom()
        {
            try
            {
                // Найдите элемент формы для отправки и выполните Submit()
                IWebElement formElement = _wait.Until(e => e.FindElement(By.Id("login-form")));
                formElement.Submit();
            }
            catch (Exception)
            {

            }
        }


        // Вставляю код для отправи
        private void SetCodeToInput(string code)
        {
            try
            {
                IWebElement setCodeInput = _wait.Until(e => e.FindElement(By.Id("twoFactorCode")));
                setCodeInput.Clear();
                Thread.Sleep(500);
                setCodeInput.SendKeys(code);
            }
            catch (Exception)
            {
            }
        }

        // Подтверждаю отправку формы с кодом
        private void SubmitCode()
        {
            try
            {
                // <a role="button" class="otkbtn otkbtn-primary  zero-margin" href="javascript:void(0);" id="btnSubmit" style="float:right;max-width:100%;">Sign in</a>
                IWebElement submitCodeBtn = _wait.Until(e => e.FindElement(By.Id("btnSubmit")));
                submitCodeBtn.Click();
            }
            catch (Exception)
            {

            }
        }

        // Проверка успешной авторизации для получени кода подтверждения
        private bool WrongAuth()
        {
            try
            {
                // Поиск элемента по тексту
                IWebElement errorElement = _wait.Until(e => e.FindElement(By.XPath("//p[@class='otkinput-errormsg otkc' and contains(text(), 'Your credentials are incorrect or have expired.')]")));

                // Проверка наличия элемента
                return errorElement != null;
            }
            catch (NoSuchElementException)
            {
                // Если элемент не найден, возвращаем false
                return false;
            }
        }

    }
}
