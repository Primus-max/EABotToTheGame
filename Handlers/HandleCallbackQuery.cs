namespace EABotToTheGame.Handlers
{   
    public class HandleCallbackQuery : IHadlerManager
    {
        private readonly AppModeManager _appModeManager;
        private readonly AutoMode _autoMode;
        public HandleCallbackQuery(AppModeManager appModeManager, AutoMode autoMode) 
        {
            _appModeManager = appModeManager;
            _autoMode = autoMode;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            long userId = update.CallbackQuery.From.Id; // Id юзера

            // Если авторежим
            if (update.CallbackQuery.Data == "autoMode")
            {
                // Устанавливаю состояние
                _appModeManager.SetAppMode(userId, AppMode.AutoMode);

                // Запускаю работу в автоматическом режиме
                await _autoMode.ExecuteAsync(botClient, update, cancellationToken);
            }

            // Если ручной режим
            if (update.CallbackQuery.Data == "manualMode")
            {
                // Устанавливаю состояние
                _appModeManager.SetAppMode(userId, AppMode.ManualMode);

                // Отправляю сообщение с запросом данных
                string message = "Введи почту и пароль чере пробел, пример: [email@email.ru 12345qwerty]";               
                await botClient.SendTextMessageAsync(userId, message);
            }
        }
    }
}
