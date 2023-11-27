using static EABotToTheGame.Managers.BotStateMachine;

namespace EABotToTheGame.Services
{
    public class MessageService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InlineKeyboardProviderFactory _inlineKeyboardProviderFactory;
        private readonly Dictionary<long, int> _lastMessageIds = new Dictionary<long, int>();
        private BotState _currentState;

        public MessageService(ITelegramBotClient botClient, InlineKeyboardProviderFactory inlineKeyboardProviderFactory)
        {
            _botClient = botClient;
            _inlineKeyboardProviderFactory = inlineKeyboardProviderFactory;
        }

        public async Task SendMessageAsync(long userId, string message, BotState nextState, CancellationToken cancellationToken)
        {
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

        public void UpdateBotState(BotState newState)
        {
            _currentState = newState;
        }
    }

}
