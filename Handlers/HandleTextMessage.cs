using static EABotToTheGame.Managers.BotStateManager;

namespace EABotToTheGame.Handlers
{
    public class HandleTextMessage : IHadlerManager
    {
        private readonly BotStateMachine _botStateMachine;
        private readonly DataWaitService _dataWaitService;
        private readonly UserStateManager _userStateManager;
        private readonly MessageService _messageService;
        private readonly AppModeManager _appModeManager;
        private readonly WhoIAmManager _whoIAmManager;
        private readonly BotStateManager _botStateManager;
        public HandleTextMessage(BotStateMachine botStateMachine,
            DataWaitService dataWaitService,
            UserStateManager userStateManager,
            MessageService messageService,
            AppModeManager appModeManager,
            WhoIAmManager whoIAmManager,
            BotStateManager botStateManager
            )
        {
            _botStateMachine = botStateMachine;
            _dataWaitService = dataWaitService;
            _userStateManager = userStateManager;
            _messageService = messageService;
            _appModeManager = appModeManager;
            _whoIAmManager = whoIAmManager;
            _botStateManager = botStateManager;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            long userId = update?.Message?.From?.Id ?? update?.CallbackQuery?.From?.Id ?? 0;

            if (update.Message.Text.Contains("/start"))
            {
                BotState nextState = BotState.ChoiceRole;
                _botStateManager.SetNextState(nextState);

                // Обнуляю состояния
                _appModeManager.SetAppMode(userId, AppMode.Default);                

                string message = "Выбери кем управлять";
                await _messageService.SendMessageAsync(userId, message, nextState, cancellationToken);
                return; // Выходим, дальше делать нечего
            }



            // Метод для выбора навигации (перемещение по состояниям)
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
            if (currentUserState == UserState.ExpectedCodeAuthorizations)
            {
                string code = update?.Message?.Text;

                _dataWaitService.SetStringData(code);
            }
        }
    }
}
