using static EABotToTheGame.Managers.BotStateManager;

namespace EABotToTheGame.Handlers
{
    public class HandleCallbackQuery : IHadlerManager
    {
        private readonly BotStateMachine _botStateMachine;
        private readonly BotStateManager _botStateManager;

        public HandleCallbackQuery(BotStateMachine botStateMachine, BotStateManager botStateManager)
        {
            _botStateMachine = botStateMachine;
            _botStateManager = botStateManager;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (botClient == null || update == null) return;

            if (update.CallbackQuery.Data == "IAmCustomer" || update.CallbackQuery.Data == "IAmPerformer")
            {
                BotState nextBotState = BotState.ChoiceRole;
                _botStateManager.SetNextState(nextBotState);
                await _botStateMachine.ProcessUpdateAsync(update, cancellationToken);
                return;
            }           

            if (update.CallbackQuery.Data == "AutoMode" || update.CallbackQuery.Data == "ManualMode")
            {
                BotState nextBotState = BotState.ChoiceModeState;
                _botStateManager.SetNextState(nextBotState);
                await _botStateMachine.ProcessUpdateAsync(update, cancellationToken);
                return;
            }          
        }
    }
}
