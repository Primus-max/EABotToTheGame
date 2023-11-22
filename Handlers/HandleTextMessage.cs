
namespace EABotToTheGame.Handlers
{

    public class HandleTextMessage : IHadlerManager
    {
        private readonly IInlineKeyboardProvider _keyboardProvider;
        private readonly AppModeManager _appModeManager;
        private readonly AutoMode _autoMode;
        private readonly ManualMode _manualMode;
        private readonly UserStateManager _userStateManager;

        public HandleTextMessage(IInlineKeyboardProvider keyedServiceProvider, AppModeManager appModeManager, AutoMode autoMode, UserStateManager userStateManager, ManualMode manualMode)
        {
            _keyboardProvider = keyedServiceProvider;
            _appModeManager = appModeManager;
            _autoMode = autoMode;
            _userStateManager = userStateManager;
            _manualMode = manualMode;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            long userId = update.Message.From.Id; // Id юзера

            // Начало
            if (update.Message.Text.Contains("/start"))
            {
                // Отправляю кнопки
                string message = "Выбери режим работы бота";
                var buttons = _keyboardProvider.GetButtonsInlineKeyboard();
                await botClient.SendTextMessageAsync(userId, message, replyMarkup: buttons);
            }


            AppMode currentMode = _appModeManager.GetCurrentAppMode(userId);// Получаю текущий мод
            UserState userState = _userStateManager.GetUserState(userId); // Получаю состояние 

            if (userState == UserState.ExpectedCodeAuthorizations) // Ожидаем код подтверждения
            {
                // Возвращаю полученный код и продолжаю действия в браузере
                if (string.IsNullOrEmpty(update.Message.Text)) return;

                string codeAuth = update.Message.Text;

                // В зависимости от мода вызываю метод 
                switch (AppMode.AutoMode)
                {
                    case AppMode.AutoMode:
                        _autoMode.CompleteCodeReceivedTask(codeAuth);
                        break;
                    case AppMode.ManualMode:
                        // Здесь код для ручного режима

                }

            }

            if(userState == UserState.ExpectedEmailAuthorizationsData)
            {
                // Логика для получения новых данных и передачи обратно в код
            }

            if (currentMode == AppMode.AutoMode)
            {

            }

            if (currentMode == AppMode.ManualMode)
            {
                string resiveAuthData = update.Message.Text;
                string email = update.Message.Text.Split(" ")[0];
                string password = update.Message.Text.Split(" ")[1];

                AuthData authData = new() { Email = email, Password = password };

                // Здесь вызываем метод для заполнения данных, делания скриншота и отправки
            }
        }
    }
}
