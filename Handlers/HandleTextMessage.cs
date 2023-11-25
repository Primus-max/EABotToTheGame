namespace EABotToTheGame.Handlers
{

    public class HandleTextMessage : IHadlerManager
    {
        private readonly BotStateMachine _botStateMachine;

        public HandleTextMessage(BotStateMachine botStateMachine)
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
