namespace EABotToTheGame.Interfaces
{
    public interface IHadlerManager
    {
        Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}
