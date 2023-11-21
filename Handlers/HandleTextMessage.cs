namespace EABotToTheGame.Handlers
{
    
    public class HandleTextMessage : IHadlerManager
    {
        private readonly IInlineKeyboardProvider _keyboardProvider;

        public HandleTextMessage(IInlineKeyboardProvider keyedServiceProvider)
        {
            _keyboardProvider = keyedServiceProvider;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            long? userId = update.Message.From.Id; // Id юзера

            // Начало
            if (update.Message.Text.Contains("/start")) 
            {
                // Отправляю кнопки
                string message = "Выбери режим работы бота";
                var buttons = _keyboardProvider.GetButtonsInlineKeyboard();
                await botClient.SendTextMessageAsync(userId, message, replyMarkup: buttons);
            }
        }
    }
}
