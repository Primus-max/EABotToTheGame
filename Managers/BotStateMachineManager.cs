﻿using static EABotToTheGame.Managers.BotStateManager;

namespace EABotToTheGame.Managers
{
    public class BotStateMachine
    {

        private readonly ITelegramBotClient _botClient;
        private readonly IInlineKeyboardProvider _keyboardProvider;
        private readonly SitesWorkerService _autoMode;
        private readonly AppModeManager _appModeManager;
        private readonly InlineKeyboardProviderFactory _inlineKeyboardProviderFactory;
        private readonly WhoIAmManager _whoIAmManager;
        private Dictionary<long, int> _lastMessageIds = new Dictionary<long, int>();
        private readonly DataWaitService _authDataWaitService;
        private readonly UserStateManager _userStateManager;
        private readonly BotStateManager _botStateManager;
        private readonly MessageService _messageService;


        public BotStateMachine(ITelegramBotClient botClient,
            IInlineKeyboardProvider keyboardProvider,
            SitesWorkerService autoMode,
            AppModeManager appModeManager,
            InlineKeyboardProviderFactory inlineKeyboardProviderFactory,
            WhoIAmManager whoIAmManager,
            DataWaitService authDataWaitService,
            UserStateManager userStateManager,
            BotStateManager botStateManager,
            MessageService messageService
            )
        {
            _botClient = botClient;
            _keyboardProvider = keyboardProvider;
            _autoMode = autoMode;
            _appModeManager = appModeManager;
            _inlineKeyboardProviderFactory = inlineKeyboardProviderFactory;
            _whoIAmManager = whoIAmManager;
            _authDataWaitService = authDataWaitService;
            _userStateManager = userStateManager;
            _botStateManager = botStateManager;
            _messageService = messageService;
        }

        public async Task ProcessUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            long userId = update?.Message?.From?.Id ?? update?.CallbackQuery?.From?.Id ?? 0;

            BotState currentBotState = _botStateManager.GetNextState();

            switch (currentBotState)
            {
                //case BotState.StartScreenState:
                //    await ProcessStartScreenState(userId, update, cancellationToken);
                //    break;
                case BotState.ChoiceRole:
                    await ProcessChoiceRoleState(userId, update, cancellationToken);
                    break;
                case BotState.ChoiceModeState:
                    await ProcessChoiceModeState(userId, update, cancellationToken);
                    break;
                //case BotState.AutoModeState:
                //    await ProcessAutoModeState(userId, update, cancellationToken);
                //    break;
                //case BotState.ManualModeState:
                //    await ProcessManualModeState(userId, update, cancellationToken);
                //    break;
                default:
                    break;
            }
        }

        private async Task ProcessStartScreenState(long userId, Update update, CancellationToken cancellationToken)
        {
            // Логика для обработки состояния StartScreenState
            //if (update.Message.Text.Contains("/start"))
            //{
            //    _currentState = BotState.StartScreenState;
            //    _nextState = BotState.ChoiceRole;

            //    // Обнуляю состояния
            //    _appModeManager.SetAppMode(userId, AppMode.Default);
            //    _whoIAmManager.WhiteWhoIAm(userId, WhoIAm.Default);

            //    string message = "Выбери кем управлять";
            //    await SendMessage(userId, message, _nextState, cancellationToken);
            //}
            //else if (update.Message.Text.ToLower() == "/r") // Добавляем обработку кнопки "Старт"
            //{
            //    // Выполняем действия для начала сценария заново
            //   // _currentState = BotState.InitialState; // Устанавливаем начальное состояние
            //    _nextState = BotState.StartScreenState; // Устанавливаем следующее состояние

            //    // Дополнительные действия, если нужно

            //    string message = "Давай начнем заново!";
            //    await SendMessage(userId, message, _nextState, cancellationToken);
            //}
        }

        private async Task ProcessChoiceRoleState(long userId, Update update, CancellationToken cancellationToken)
        {
           // BotState currentBotState = _botStateManager.GetCurrentState();
            //if (currentBotState != BotState.ChoiceRole || update == null) return;

            // Состояния для навигации
            BotState nexstState = BotState.ChoiceModeState;            
            _botStateManager.SetNextState(nexstState);

            if (!string.IsNullOrEmpty(update?.CallbackQuery?.Data) && Enum.TryParse<WhoIAm>(update.CallbackQuery.Data, out var role))
                _whoIAmManager.WhiteWhoIAm(userId, role); // Устанавливаю текущую роль пользователя

            // Отправляю сообщение
            string message = "Выбери режим работы бота";
            await _messageService.SendMessageAsync(userId, message, nexstState, cancellationToken);
            return;
        }

        private async Task ProcessChoiceModeState(long userId, Update update, CancellationToken cancellationToken)
        {
            
            // Получаю текущий мод
            if (!string.IsNullOrEmpty(update?.CallbackQuery?.Data) && Enum.TryParse<AppMode>(update.CallbackQuery.Data, out var mode))
            {
                _appModeManager.SetAppMode(userId, mode);

                AuthData authData = new();

               // _botStateManager.SetPre
                //_previousState = _currentState; // Сохраняем текущее состояние
                //_currentState = mode = ;
                bool isAutoMode = mode == AppMode.AutoMode;

                // Если ручно режим, запрашиваю данные
                if (!isAutoMode)
                {
                    // Запрашиваю данные
                    await _botClient.SendTextMessageAsync(userId, "Введи почту и пароль через пробел, пример: [email@email.ru 12345qwerty]");
                    // Устанавливаю статус для юзера ожидание регистрационных данных
                    _userStateManager.SetUserState(userId, UserState.ExpectedEmailAuthorizationsData);
                    // Получаю данные
                    authData = await _authDataWaitService.WaitForAuthDataAsync();
                }

                // Запускаю работу с полученными данными или без них
                await _autoMode.ExecuteAsync(_botClient, update, cancellationToken, authData);

                return;
            }
        }


        //private async Task ProcessAutoModeState(long userId, Update update, CancellationToken cancellationToken)
        //{
        //    // Логика для обработки состояния AutoModeState
        //    // ...

        //    // Логика для переходов между состояниями
        //    if (update?.Message?.Text == "команда_возврата_в_главное_меню")
        //    {
        //        _currentState = BotState.StartScreenState;
        //        // Логика возврата в главное меню
        //        // ...
        //    }
        //    else if (update?.Message?.Text == "команда_возврата_назад")
        //    {
        //        // Возвращаемся к предыдущему состоянию
        //        _currentState = _previousState;
        //    }
        //}

        //private async Task ProcessManualModeState(long userId, Update update, CancellationToken cancellationToken)
        //{
        //    // Логика для обработки состояния ManualModeState
        //    // ...

        //    // Логика для переходов между состояниями
        //    if (update.Message.Text.Contains("команда_возврата_в_главное_меню"))
        //    {
        //        _currentState = BotState.StartScreenState;
        //        // Логика возврата в главное меню
        //        // ...
        //    }
        //    else if (update.Message.Text == "команда_возврата_назад")
        //    {
        //        // Возвращаемся к предыдущему состоянию
        //        _currentState = _previousState;
        //    }
        //}

        //private async Task SendMessage(long userId, string message, BotState nextState, CancellationToken cancellationToken)
        //{
        //    //if (_currentState != nextState)
        //    //    return;

        //    if (!_lastMessageIds.TryGetValue(userId, out int lastMessageId))
        //        lastMessageId = 0;

        //    var keyboardProvider = _inlineKeyboardProviderFactory.CreateKeyboardProvider(nextState);
        //    var buttons = keyboardProvider.GetButtonsInlineKeyboard();

        //    if (lastMessageId != 0)
        //    {
        //        await _botClient.DeleteMessageAsync(userId, lastMessageId, cancellationToken);
        //    }

        //    var sentMessage = await _botClient.SendTextMessageAsync(userId, message, cancellationToken: cancellationToken, replyMarkup: buttons);
        //    _lastMessageIds[userId] = sentMessage.MessageId;

        //    _currentState = nextState;
        //}

        //public enum BotState
        //{
        //    StartScreenState,
        //    AutoModeState,
        //    ManualModeState,
        //    ChoiceRole,
        //    ChoiceModeState,
        //}
    }

}
