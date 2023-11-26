namespace EABotToTheGame.Handlers
{
    public class HandleTextMessage : IHadlerManager
    {
        private readonly BotStateMachine _botStateMachine;
        private readonly DataWaitService _dataWaitService;       
        private readonly UserStateManager _userStateManager;
        public HandleTextMessage(BotStateMachine botStateMachine, DataWaitService dataWaitService, UserStateManager userStateManager)
        {
            _botStateMachine = botStateMachine;
            _dataWaitService = dataWaitService;
            _userStateManager = userStateManager;           
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            long userId = update?.Message?.From?.Id ?? (update?.CallbackQuery?.From?.Id ?? 0);

            // Метод для выбора навигации (перемещение по состоянием)
            await _botStateMachine.ProcessUpdateAsync(update, cancellationToken);

            // Получаю текущий статус 
            UserState currentUserState = _userStateManager.GetUserState(userId);

            // Если ожидаем ввода регистрационных данных
            if (currentUserState == UserState.ExpectedEmailAuthorizationsData)
            {
                var gotUserData = update?.Message?.Text?.Split(" ");

                AuthData authData = new()
                {
                    Email = gotUserData[0],
                    Password = gotUserData[1],

                };
                _dataWaitService.SetAuthData(authData);
            }

            // Если ожидаю код авторизации
            if(currentUserState == UserState.ExpectedCodeAuthorizations)
            {
                string code = update?.Message?.Text;

                _dataWaitService.SetStringData(code);
            }
        }
    }
}
