namespace SexoPolisBot.Controllers
{
    /// <summary>
    /// Базовый класс создающий бота и прослушивающий сообщения
    /// </summary>
    public class TelegramBotController
    {
        private readonly ITelegramBotClient _botClient;
       
        public TelegramBotController(ITelegramBotClient botClient)
        {
            _botClient = botClient;            
        }


        /// <summary>
        /// Метод инициализации бота и его запуск. Метод-точка входа в приложение
        /// </summary>
        /// <returns></returns>
        public async Task StartBotAsync()
        {
            using var cts = new CancellationTokenSource();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>(), // receive all update types except ChatMember related updates
                ThrowPendingUpdates = true
            };

            await _botClient.ReceiveAsync(
                 updateHandler: HandleUpdateAsync,
                 pollingErrorHandler: HandlePollingErrorAsync,
                 receiverOptions: receiverOptions,
                 cancellationToken: cts.Token
             );

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Bot Id: {me.Id}, Bot Name: {me.Username}");
        }

        /// <summary>
        /// Метод прослушивающий и обрабатывающий входящие сообщения от пользователей
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {           
            // Если информация пришла из канала где бот администратор
            if (update.ChannelPost != null || update.Type == UpdateType.ChatJoinRequest)
            {
                try
                {
                   // Task.Run(() => _channelPostHandlerManager.HandleChannelPostAsync(botClient, update, cancellationToken));
                }
                catch (Exception)
                {

                    throw;
                }
            }

            // Если пришло текстовое сообщение
            if (update.Message != null)
            {
                try
                {
                   // Task.Run(() => _messageHandlerManager.HandleMessageAsync(botClient, update, cancellationToken));
                }
                catch (Exception)
                {

                    throw;
                }
            }

            // Если пришёл Callback
            if (update.CallbackQuery != null)
            {

                try
                {
                   // Task.Run(() => _callbackQueryHandler.HandleCallbackQueryAsync(botClient, update, cancellationToken));
                }
                catch (Exception)
                {

                    throw;
                }


            }

        }

        /// <summary>
        /// Метод для перехвата и логирования ошибок
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
