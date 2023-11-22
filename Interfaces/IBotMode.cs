using EABotToTheGame.Models;

namespace EABotToTheGame.Intergaces
{
    public interface IBotMode
    {
        Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null!);
    }
}
