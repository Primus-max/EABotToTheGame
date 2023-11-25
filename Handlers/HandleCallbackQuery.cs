namespace EABotToTheGame.Handlers
{
    public class HandleCallbackQuery : IHadlerManager
    {
        private readonly BotStateMachine _botStateMachine;

        public HandleCallbackQuery(BotStateMachine botStateMachine)
        {
            _botStateMachine = botStateMachine;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            await _botStateMachine.ProcessUpdateAsync(update, cancellationToken);
        }
    }
}
