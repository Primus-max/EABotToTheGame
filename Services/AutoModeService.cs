using EABotToTheGame.Managers;
using EABotToTheGame.Models;

namespace EABotToTheGame.Services
{
    public class AutoMode : IBotMode
    {
        private IWebDriver _driver = null!;
        private AuthData _authData;
        private TaskCompletionSource<string> _codeReceivedTaskCompletionSource = null!;
        private readonly UserStateManager _userStateManager;
        private string _copiedPathProfile = string.Empty;

        public AutoMode(AppModeManager appModeManager, UserStateManager userStateManager)
        {
            _codeReceivedTaskCompletionSource = new TaskCompletionSource<string>(); // Код для ожидания завершения задачи, в нашем случае ожидание кода от юзера
            _userStateManager = userStateManager;
            
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!)
        {
            if (botClient == null || update == null) return;

            _driver = InitializeDriver(); // Получаю драйвер

            long userId = update.CallbackQuery.From.Id; // Id юзера
            _authData = authData; // Объект с почтой и паролем

            _driver.Navigate().GoToUrl("https://signin.ea.com/p/juno/login?execution=e1657413137s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fhide_create%3Dtrue%26display%3Dweb2%252Flogin%26scope%3Dbasic.identity%2Boffline%2Bsignin%2Bbasic.entitlement%2Bbasic.persona%26release_type%3Dprod%26response_type%3Dtoken%26redirect_uri%3Dhttps%253A%252F%252Fwww.ea.com%252Fea-sports-fc%252Fultimate-team%252Fweb-app%252Fauth.html%26accessToken%3D%26locale%3Den_US%26prompt%3Dlogin%26client_id%3DFC24_JS_WEB_APP\r\n");
            CloseBrowser();
            //https://signin.ea.com/p/juno/login?execution=e1657413137s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fhide_create%3Dtrue%26display%3Dweb2%252Flogin%26scope%3Dbasic.identity%2Boffline%2Bsignin%2Bbasic.entitlement%2Bbasic.persona%26release_type%3Dprod%26response_type%3Dtoken%26redirect_uri%3Dhttps%253A%252F%252Fwww.ea.com%252Fea-sports-fc%252Fultimate-team%252Fweb-app%252Fauth.html%26accessToken%3D%26locale%3Den_US%26prompt%3Dlogin%26client_id%3DFC24_JS_WEB_APP
            await WaitCodeAuthAsync(userId);
        }

        private async Task<string> WaitCodeAuthAsync(long userId)
        {
            // Реализуйте вашу логику работы в этом методе
            // ...


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

        // Получаю драйвер
        private IWebDriver InitializeDriver()
        {
            string originalPathProfile = @"C:\Users\FedoTT\AppData\Local\Google\Chrome\User Data";

            //ProfilePathManager profilePathManager = new();
            //_copiedPathProfile = profilePathManager.CreateTempProfile(originalPathProfile);

            WebDriverManager webDriverManager = new WebDriverManager();
            return webDriverManager.GetDriver();
        }

        // Закрываю браузер
        private void CloseBrowser()
        {
            try
            {
                _driver.Quit();

                // Удаляю временный профиль
                ProfilePathManager profilePathManager = new();
                profilePathManager.DeleteDirectory(_copiedPathProfile);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}
