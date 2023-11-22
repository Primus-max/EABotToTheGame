using EABotToTheGame.Models;

namespace EABotToTheGame.Services.BaseModeService
{
    public class BaseModeService : IBotMode
    {      
        public Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, AuthData authData = null)
        {
            throw new NotImplementedException();
        }
    }
}
