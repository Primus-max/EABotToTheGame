using EABotToTheGame.Services.SiteServices;

namespace EABotToTheGame.Services
{
    public class AutoMode : IBotMode
    {
        private readonly string EASportUrl = "https://signin.ea.com/p/juno/login?execution=e921889951s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fhide_create%3Dtrue%26display%3Dweb2%252Flogin%26scope%3Dbasic.identity%2Boffline%2Bsignin%2Bbasic.entitlement%2Bbasic.persona%26release_type%3Dprod%26response_type%3Dtoken%26redirect_uri%3Dhttps%253A%252F%252Fwww.ea.com%252Fea-sports-fc%252Fultimate-team%252Fweb-app%252Fauth.html%26accessToken%3D%26locale%3Den_US%26prompt%3Dlogin%26client_id%3DFC24_JS_WEB_APP";
        private readonly string BlazeTrackUrl = "https://blaze-track.com/";
        private IWebDriver _driver = null!;
        private AuthData _authData;
        private TaskCompletionSource<string> _codeReceivedTaskCompletionSource = null!;
        private TaskCompletionSource<AuthData> _authDataReceivedTaskCompletionSource = null!;
        private readonly UserStateManager _userStateManager;
        private string _copiedPathProfile = string.Empty;

        public AutoMode(UserStateManager userStateManager)
        {
            _codeReceivedTaskCompletionSource = new TaskCompletionSource<string>(); // Код для ожидания завершения задачи, в нашем случае ожидание кода от юзера
            _authDataReceivedTaskCompletionSource = new TaskCompletionSource<AuthData>();
            _userStateManager = userStateManager;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!)
        {
            if (botClient == null || update == null) return;

            _driver = InitializeDriver(); // Получаю драйвер
            _authData = authData; // Полученные данные записываю глобально

            TabManager tabManager = new TabManager(_driver); // Создаю конструктор менеджера вкладок

            long userId = update.CallbackQuery.From.Id; // Id юзера

            EASportSiteService eASportSiteService = new(_driver); // Сервис работы с EASports
            tabManager.OpenOrSwitchTab(EASportUrl);// Переключаюсь на EASports

            if (_authData != null)
            {
                bool isAuth = eASportSiteService.Authorizations(_authData.Email, _authData.Password); // Авторизуюсь

                // Если не авторизовался, отрпавляю сообщение и ставлю ожидание на новые данные
                if (!isAuth)
                {
                    string wrongAuthMessage = $"Проблема с авторизацией: Your credentials are incorrect or have expired. Please try again or...";
                    await botClient.SendTextMessageAsync(userId, wrongAuthMessage);

                    _userStateManager.SetUserState(userId, UserState.ExpectedEmailAuthorizationsData); // Устанавливаю состояние ожидания кода авторизации

                    _authData = await _authDataReceivedTaskCompletionSource.Task; // Ставлю на ожидание новых регистрационных данных 
                }
                else // Если авторизовался запрашиваю код подтверждения
                {
                    bool isSendedCode = eASportSiteService.SendCodeOnEmail(); // Нажимаю кнопку отправить код на почту, на вский случай проверяю операцию

                    // Повторить если не получилось нажать кнопку отправки кода
                    if (!isSendedCode)
                    {
                        string cantSendCodeOnEmail = $"Что-то пошло не так, не удалось отправить код на почту, жмакни еще раз";
                    }

                    string codeAuthorization = await _codeReceivedTaskCompletionSource.Task; // Ожидаю код атворизации
                    // Если получили код авторизации, продолжаем работу
                    if (!string.IsNullOrEmpty(codeAuthorization)) 
                    {
                        eASportSiteService.SubmitCodeAuthorizations(codeAuthorization); // Отправляю код
                    }
                    
                    // Тут дальше код для скриншота
                    // Тут дальше код для отправки скриншота
                }
            }


            tabManager.OpenOrSwitchTab(BlazeTrackUrl);

            tabManager.OpenOrSwitchTab(EASportUrl);


            tabManager.OpenOrSwitchTab(BlazeTrackUrl);
            // CloseBrowser();
            await WaitCodeAuthAsync(userId);
        }

        private async Task<string> WaitCodeAuthAsync(long userId)
        {

            // Установка статуса ожидания кода
            _userStateManager.SetUserState(userId, UserState.ExpectedCodeAuthorizations);

            // Ожидание получения кода
            string receivedCode = await _codeReceivedTaskCompletionSource.Task;

            // Продолжение выполнения после получения кода
            return receivedCode;
        }


        // Метод для завершения ожидания кода от пользователя, возвращает код
        public void CompleteCodeReceivedTask(string code)
        {
            _codeReceivedTaskCompletionSource.TrySetResult(code);
        }

        // Метод ожидания повторных данных (email, password)
        public void CompleteAuthDataReceivedTask(AuthData authData)
        {
            _authDataReceivedTaskCompletionSource.TrySetResult(authData);
        }

        // Получаю драйвер
        private IWebDriver InitializeDriver()
        {   
            WebDriverManager webDriverManager = new();
            return webDriverManager.GetDriver();
        }

        // Закрываю браузер
        private void CloseBrowser()
        {
            try
            {
                _driver.Quit();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}
