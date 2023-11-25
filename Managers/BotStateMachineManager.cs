using Telegram.Bot.Types;

namespace EABotToTheGame.Managers
{
    public class BotStateMachine
    {
        
        private readonly ITelegramBotClient _botClient;
        private readonly IInlineKeyboardProvider _keyboardProvider;
        private readonly AutoMode _autoMode;
        private readonly ManualMode _manualMode;
        private readonly AppModeManager _appModeManager;
        private readonly InlineKeyboardProviderFactory _inlineKeyboardProviderFactory;
        private Dictionary<long, int> _lastMessageIds = new Dictionary<long, int>();

        // Свойства бота
        private BotState _currentState;
        private BotState _previousState;
        private BotState _nextState;
        public BotStateMachine(ITelegramBotClient botClient, IInlineKeyboardProvider keyboardProvider, AutoMode autoMode, ManualMode manualMode, AppModeManager appModeManager, InlineKeyboardProviderFactory inlineKeyboardProviderFactory)
        {
            _botClient = botClient;
            _keyboardProvider = keyboardProvider;
            _autoMode = autoMode;
            _manualMode = manualMode;
            _appModeManager = appModeManager;
            _currentState = BotState.StartScreenState;
            _previousState = BotState.StartScreenState; // Устанавливаем начальное предыдущее состояние
            _inlineKeyboardProviderFactory = inlineKeyboardProviderFactory;
        }

        public async Task ProcessUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            long userId = update?.Message?.From?.Id ?? update?.CallbackQuery?.From?.Id ?? 0;

            switch (_currentState)
            {
                case BotState.StartScreenState:
                    await ProcessStartScreenState(userId, update, cancellationToken);
                    break;
                case BotState.ChoiceRole:
                    await ProcessChoiceRoleState(userId, update, cancellationToken);
                    break;
                case BotState.ChoiceModeState:
                    await ProcessChoiceModeState(userId, update, cancellationToken);
                    break;
                case BotState.AutoModeState:
                    await ProcessAutoModeState(userId, update, cancellationToken);
                    break;
                case BotState.ManualModeState:
                    await ProcessManualModeState(userId, update, cancellationToken);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessStartScreenState(long userId, Update update, CancellationToken cancellationToken)
        {
            // Логика для обработки состояния StartScreenState
            if (update.Message.Text.Contains("/start"))
            {
                _currentState = BotState.StartScreenState;
                _nextState = BotState.ChoiceRole;
                string message = "Выбери кем управлять";
                await SendMessage(userId, message, _nextState, cancellationToken);  
            }            
        }


        private async Task ProcessChoiceRoleState(long userId, Update update, CancellationToken cancellationToken)
        {
            if (_currentState != BotState.ChoiceRole || update == null) return;

            _currentState = _nextState;
            _nextState = BotState.ChoiceModeState;
            _previousState = BotState.ChoiceRole;
            string message = "Выбери режим работы бота";
            await SendMessage(userId, message, _nextState, cancellationToken);
        }

        private async Task ProcessChoiceModeState(long userId, Update update, CancellationToken cancellationToken) 
        {
            // Логика для переходов между состояниями
            if (update?.CallbackQuery?.Data == "autoMode")
            {
                _appModeManager.SetAppMode(userId, AppMode.AutoMode);
                _previousState = _currentState; // Сохраняем текущее состояние
                _currentState = BotState.AutoModeState;
                await _autoMode.ExecuteAsync(_botClient, update, cancellationToken);
            }
            else if (update?.CallbackQuery?.Data == "manualMode")
            {
                _appModeManager.SetAppMode(userId, AppMode.ManualMode);
                _previousState = _currentState; // Сохраняем текущее состояние
                _currentState = BotState.ManualModeState;
                await _botClient.SendTextMessageAsync(userId, "Введи почту и пароль через пробел, пример: [email@email.ru 12345qwerty]");
            }
        }


        private async Task ProcessAutoModeState(long userId, Update update, CancellationToken cancellationToken)
        {
            // Логика для обработки состояния AutoModeState
            // ...

            // Логика для переходов между состояниями
            if (update?.Message?.Text == "команда_возврата_в_главное_меню")
            {
                _currentState = BotState.StartScreenState;
                // Логика возврата в главное меню
                // ...
            }
            else if (update?.Message?.Text == "команда_возврата_назад")
            {
                // Возвращаемся к предыдущему состоянию
                _currentState = _previousState;
            }
        }

        private async Task ProcessManualModeState(long userId, Update update, CancellationToken cancellationToken)
        {
            // Логика для обработки состояния ManualModeState
            // ...

            // Логика для переходов между состояниями
            if (update.Message.Text.Contains("команда_возврата_в_главное_меню"))
            {
                _currentState = BotState.StartScreenState;
                // Логика возврата в главное меню
                // ...
            }
            else if (update.Message.Text == "команда_возврата_назад")
            {
                // Возвращаемся к предыдущему состоянию
                _currentState = _previousState;
            }
        }

        private async Task SendMessage(long userId, string message, BotState nextState, CancellationToken cancellationToken)
        {            
            //if (_currentState != nextState)
            //    return;

            if (!_lastMessageIds.TryGetValue(userId, out int lastMessageId))
                lastMessageId = 0;

            var keyboardProvider = _inlineKeyboardProviderFactory.CreateKeyboardProvider(nextState);
            var buttons = keyboardProvider.GetButtonsInlineKeyboard();

            if (lastMessageId != 0)
            {
                await _botClient.DeleteMessageAsync(userId, lastMessageId, cancellationToken);
            }

            var sentMessage = await _botClient.SendTextMessageAsync(userId, message, cancellationToken: cancellationToken, replyMarkup: buttons);
            _lastMessageIds[userId] = sentMessage.MessageId;

            _currentState = nextState;
        }


        public enum BotState
        {
            StartScreenState,
            AutoModeState,
            ManualModeState,
            ChoiceRole,
            ChoiceModeState,
        }
    }

}
