using EABotToTheGame.Services.SiteServices;

namespace EABotToTheGame.Services
{
    public class AutoMode : IBotMode
    {
        private readonly string EASportUrl = "https://signin.ea.com/p/juno/login?execution=e921889951s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fhide_create%3Dtrue%26display%3Dweb2%252Flogin%26scope%3Dbasic.identity%2Boffline%2Bsignin%2Bbasic.entitlement%2Bbasic.persona%26release_type%3Dprod%26response_type%3Dtoken%26redirect_uri%3Dhttps%253A%252F%252Fwww.ea.com%252Fea-sports-fc%252Fultimate-team%252Fweb-app%252Fauth.html%26accessToken%3D%26locale%3Den_US%26prompt%3Dlogin%26client_id%3DFC24_JS_WEB_APP";
        private readonly string BlazeTrackUrl = "https://blaze-track.com/site/orders?customer_id=1";
        private IWebDriver _driver = null!;
        private AuthData _authData;
        private readonly DataWaitService _dataWaitService;        
        private readonly UserStateManager _userStateManager;
        private readonly WebDriverManager _webDriverManager = null!;
        private readonly WhoIAmManager _whoIAmManager = null!;
        private readonly AppModeManager _appModeManager;

        public AutoMode(UserStateManager userStateManager, 
            WebDriverManager webDriverManager, 
            WhoIAmManager whoIAmManager,
            DataWaitService dataWaitService,             
            AppModeManager appModeManager
            )
        {           
            _userStateManager = userStateManager;
            _webDriverManager = webDriverManager;
            _whoIAmManager = whoIAmManager;
            _dataWaitService = dataWaitService;           
            _appModeManager = appModeManager;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!)
        {
            if (botClient == null || update == null) return;

            long userId = update?.Message?.From?.Id ?? update?.CallbackQuery?.From?.Id ?? 0; // ID юзера

            WhoIAm whoIAm = _whoIAmManager.TellMeWhoIAm(userId); // Получаю статус юзера

            _driver = _webDriverManager.GetDriver(whoIAm); // Получаю драйвер для юзера

            TabManager tabManager = new TabManager(_driver); // Создаю конструктор менеджера вкладок  
            BlazeTrackService blazeTrack =null!;

            // Если передали данные значит в ручном режиме
            if (authData != null)
            {
                _authData = authData;
            }
            else
            {
                 blazeTrack = new(_driver);
                tabManager.OpenOrSwitchTab(BlazeTrackUrl);
                AuthData auth = blazeTrack.GetAuthData(); // Получаю данные для авторизации
                _authData = auth;
            }

            await Task.Delay(2000);

            EASportSiteService eASportSiteService = new(_driver); // Сервис работы с EASports
            tabManager.OpenOrSwitchTab(EASportUrl);// Переключаюсь на EASports

            if (_authData != null)
            {
                bool isAuth = false;
                do
                {
                    isAuth = eASportSiteService.Authorizations(_authData.Email, _authData.Password); // Авторизуюсь                   

                    if (!isAuth)
                    {
                        // Если не авторизовался, отрпавляю сообщение и ставлю ожидание на новые данные
                        string wrongAuthMessage = $"Проблема с авторизацией, отправь email  и пароль, я попробую снова авторизоваться, пример: [email@email.ru 12345qwerty]";
                        await botClient.SendTextMessageAsync(userId, wrongAuthMessage);

                        _userStateManager.SetUserState(userId, UserState.ExpectedEmailAuthorizationsData); // Устанавливаю состояние ожидания кода авторизации

                        _authData = await _dataWaitService.WaitForAuthDataAsync(); // Ставлю на ожидание новых регистрационных данных 

                        // Уведомляю о полученных данных
                        string gotAuthDataMessage = $"Данные получены, пробую повторную авторизацию";
                        await botClient.SendTextMessageAsync(userId, gotAuthDataMessage);
                    }

                    _userStateManager.SetUserState(userId, UserState.Start); // Возвращаю статус

                } while (!isAuth);


                if (isAuth) // Если авторизовался запрашиваю код подтверждения
                {
                    bool isSendedCode = eASportSiteService.SendCodeOnEmail(); // Нажимаю кнопку отправить код на почту, на вский случай проверяю операцию

                    // Повторить если не получилось нажать кнопку отправки кода
                    if (!isSendedCode)
                    {
                        string cantSendCodeOnEmail = $"Что-то пошло не так, не удалось отправить код на почту, жмакни еще раз";
                    }
                    else
                    {
                        // Информирую
                        string getMeCodeMessage = "Отправь мне код авторизации";
                        await SendMessage(botClient, userId, cancellationToken, getMeCodeMessage);

                        // Устанавливаю статус ожидания сообщения
                        _userStateManager.SetUserState(userId, UserState.ExpectedCodeAuthorizations);

                        string codeAuthorization = await _dataWaitService.WaitForStringDataAsync(); // Ожидаю код атворизации

                        // Информирую
                        string infoMessage = "Код получил, продолжаю работу";
                        await SendMessage(botClient, userId, cancellationToken, infoMessage);

                        if (!string.IsNullOrEmpty(codeAuthorization))
                        {
                            eASportSiteService.SubmitCodeAuthorizations(codeAuthorization); // Отправляю код

                            // TODO сделать уведопление если не валидный код
                        }

                    }


                    bool isDownLoadedPage = eASportSiteService.WaitingDownLoadPage(); // Ожидание полной загрузки страницы

                    eASportSiteService.CloseFuckingPopup(); // Проверяю если открылся PopUp
                    await Task.Delay(500);

                    // Если страница загрузилась, то отправляю правильный скрин в телегу и вставляю на сайт в поле
                    if (isDownLoadedPage)
                    {
                        ScreenshotService screenshotService = new (_driver);
                        string screenPath = screenshotService.CaptureAndCropScreenshot();

                        AppMode currentAppMode = _appModeManager.GetCurrentAppMode(userId); // Получаю текущий мод

                        string succsessMessage = "Авторизация успешно пройдена, скриншот отправил";
                        // В автомоде отправляю скрин на сайт и в телегу
                        if (currentAppMode == AppMode.AutoMode)
                        {
                            tabManager.OpenOrSwitchTab(BlazeTrackUrl); // Переключаюсь на блейзера
                            blazeTrack.ConfirmOrder(screenPath); // Отправляю скрин и подтверждаю
                            
                            await SendMessage(botClient, userId, cancellationToken, succsessMessage, screenPath);
                        }
                        
                        await SendMessage(botClient, userId, cancellationToken, succsessMessage, screenPath);
                    }
                    else
                    {
                        // Если страница не загрузилась отправляю сообщение и скриншот страницы
                        ScreenshotService screenshotService = new(_driver);
                        string screenPAth = screenshotService.CaptureAndCropScreenshot(true); // Делаю полный скрин
                        string wrongMessageText = $"Не удалось сделать скриншот, скорее всего страница не была загружена";
                        await SendMessage(botClient, userId, cancellationToken, wrongMessageText, screenPAth);
                    }
                }
            }
            tabManager.OpenOrSwitchTab(EASportUrl);
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

        private async Task SendMessage(ITelegramBotClient botClient, long userId, CancellationToken cancellationToken, string messageText, string screenPath = null!)
        {
            // Если сообщение со скришотом
            if (!string.IsNullOrEmpty(screenPath))
            {
                // Загружаю локальный файл
                using var fileStream = new FileStream(screenPath, FileMode.Open, FileAccess.Read);
                // Создаю объект InputFile, передавая ему файловый поток и имя файла
                var inputFile = InputFile.FromStream(fileStream, Path.GetFileName(screenPath));

                // Отправляю файл через метод SendPhotoAsync
                var message = await botClient.SendPhotoAsync(userId, inputFile, caption: messageText);
            }
            else
            {

                // Если это обычное сообщение
                await botClient.SendTextMessageAsync(userId, text: messageText);
            }

        }
    }

}
