namespace EABotToTheGame.Handlers
{   
    public class HandleCallbackQuery : IHadlerManager
    {
        public HandleCallbackQuery() { }

        public Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
