//using EABotToTheGame.Services.SiteServices;

//namespace EABotToTheGame.Services
//{
//    public class ManualMode : IBotMode
//    {
//        private readonly string EASportUrl = "https://signin.ea.com/p/juno/login?execution=e921889951s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fhide_create%3Dtrue%26display%3Dweb2%252Flogin%26scope%3Dbasic.identity%2Boffline%2Bsignin%2Bbasic.entitlement%2Bbasic.persona%26release_type%3Dprod%26response_type%3Dtoken%26redirect_uri%3Dhttps%253A%252F%252Fwww.ea.com%252Fea-sports-fc%252Fultimate-team%252Fweb-app%252Fauth.html%26accessToken%3D%26locale%3Den_US%26prompt%3Dlogin%26client_id%3DFC24_JS_WEB_APP";
//        private readonly string BlazeTrackUrl = "https://blaze-track.com/site/orders?customer_id=1";
//        private IWebDriver _driver = null!;
//        private AuthData _authData;
//        private TaskCompletionSource<string> _codeReceivedTaskCompletionSource = null!;
//        private TaskCompletionSource<AuthData> _authDataReceivedTaskCompletionSource = null!;
//        private readonly UserStateManager _userStateManager;

//        public ManualMode(UserStateManager userStateManager)
//        {
//            _codeReceivedTaskCompletionSource = new TaskCompletionSource<string>(); // Код для ожидания завершения задачи, в нашем случае ожидание кода от юзера
//            _authDataReceivedTaskCompletionSource = new TaskCompletionSource<AuthData>();
//            _userStateManager = userStateManager;
//        }

//        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!)
//        {
//            if (botClient == null || update == null) return;

//            _driver = InitializeDriver(); // Получаю драйвер

//            TabManager tabManager = new TabManager(_driver); // Создаю конструктор менеджера вкладок          

//            BlazeTrackService blazeTrack = new(_driver);
//            tabManager.OpenOrSwitchTab(BlazeTrackUrl);

//            AuthData auth = blazeTrack.GetAuthData();

//            _authData = auth; // Получаю данные с сайта            

//            long userId = update.CallbackQuery.From.Id; // Id юзера

//            EASportSiteService eASportSiteService = new(_driver); // Сервис работы с EASports
//            tabManager.OpenOrSwitchTab(EASportUrl);// Переключаюсь на EASports

//            if (_authData != null)
//            {
//                bool isAuth = eASportSiteService.Authorizations(_authData.Email, _authData.Password); // Авторизуюсь

//                // Если не авторизовался, отрпавляю сообщение и ставлю ожидание на новые данные
//                if (!isAuth)
//                {
//                    string wrongAuthMessage = $"Проблема с авторизацией: Your credentials are incorrect or have expired. Please try again or...";
//                    await botClient.SendTextMessageAsync(userId, wrongAuthMessage);

//                    _userStateManager.SetUserState(userId, UserState.ExpectedEmailAuthorizationsData); // Устанавливаю состояние ожидания кода авторизации

//                    _authData = await _authDataReceivedTaskCompletionSource.Task; // Ставлю на ожидание новых регистрационных данных 

//                    // TODO код повторой отправки данных и всего процесса сначала
//                }
//                else // Если авторизовался запрашиваю код подтверждения
//                {
//                    bool isSendedCode = eASportSiteService.SendCodeOnEmail(); // Нажимаю кнопку отправить код на почту, на вский случай проверяю операцию

//                    // Повторить если не получилось нажать кнопку отправки кода
//                    if (!isSendedCode)
//                    {
//                        string cantSendCodeOnEmail = $"Что-то пошло не так, не удалось отправить код на почту, жмакни еще раз";
//                    }
//                    else
//                    {

//                        string succsessMessage = "Отправь мне код авторизации";
//                        await SendMessage(botClient, userId, cancellationToken, succsessMessage);
//                        // Устанавливаю статус ожидания сообщения
//                        _userStateManager.SetUserState(userId, UserState.ExpectedCodeAuthorizations);

//                        string codeAuthorization = await _codeReceivedTaskCompletionSource.Task; // Ожидаю код атворизации
//                                                                                                 // Если получили код авторизации, продолжаем работу
//                        if (!string.IsNullOrEmpty(codeAuthorization))
//                        {
//                            eASportSiteService.SubmitCodeAuthorizations(codeAuthorization); // Отправляю код

//                            // TODO сделать уведопление если не валидный код
//                        }

//                    }


//                    bool isDownLoadedPage = eASportSiteService.WaitingDownLoadPage();

//                    // Если страница загрузилась, то отправляю правильный скрин в телегу и вставляю на сайт в поле
//                    if (isDownLoadedPage)
//                    {
//                        ScreenshotService screenshotService = new ScreenshotService(_driver);
//                        string screenPath = screenshotService.CaptureAndCropScreenshot();

//                        tabManager.OpenOrSwitchTab(BlazeTrackUrl); // Переключаюсь на блейзера

//                        blazeTrack.ConfirmOrder(screenPath, "START"); // Отправляю скрин и подтверждаю

//                        string succsessMessage = "Авторизация успешно пройдена, скриншот отправил";
//                        await SendMessage(botClient, userId, cancellationToken, succsessMessage, screenPath);
//                    }
//                    else
//                    {
//                        // Если страница не загрузилась отправляю сообщение и скриншот страницы
//                        ScreenshotService screenshotService = new(_driver);
//                        string screenPAth = screenshotService.CaptureAndCropScreenshot(true); // Делаю полный скрин
//                        string wrongMessageText = $"Не удалось сделать скриншот, скорее всего страница не была загружена";
//                        await SendMessage(botClient, userId, cancellationToken, wrongMessageText, screenPAth);
//                    }
//                }
//            }
//            tabManager.OpenOrSwitchTab(EASportUrl);
//        }

//        // Метод для завершения ожидания кода от пользователя, возвращает код
//        public void CompleteCodeReceivedTask(string code)
//        {
//            _codeReceivedTaskCompletionSource.TrySetResult(code);
//        }

//        // Метод ожидания повторных данных (email, password)
//        public void CompleteAuthDataReceivedTask(AuthData authData)
//        {
//            _authDataReceivedTaskCompletionSource.TrySetResult(authData);
//        }

//        // Получаю драйвер
//        private IWebDriver InitializeDriver()
//        {
//            WebDriverManager webDriverManager = new();
//            return new ChromeDriver();
//        }

//        // Закрываю браузер
//        private void CloseBrowser()
//        {
//            try
//            {
//                _driver.Quit();
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        private async Task SendMessage(ITelegramBotClient botClient, long userId, CancellationToken cancellationToken, string messageText, string screenPath = null!)
//        {
//            // Если сообщение со скришотом
//            if (!string.IsNullOrEmpty(screenPath))
//            {
//                // Загружаю локальный файл
//                using var fileStream = new FileStream(screenPath, FileMode.Open, FileAccess.Read);
//                // Создаю объект InputFile, передавая ему файловый поток и имя файла
//                var inputFile = InputFile.FromStream(fileStream, Path.GetFileName(screenPath));

//                // Отправляю файл через метод SendPhotoAsync
//                var message = await botClient.SendPhotoAsync(userId, inputFile, caption: messageText);
//            }
//            else
//            {

//                // Если это обычное сообщение
//                await botClient.SendTextMessageAsync(userId, text: messageText);
//            }

//        }
//    }
//}
