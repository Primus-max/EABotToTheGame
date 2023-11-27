using static EABotToTheGame.Managers.BotStateManager;

namespace EABotToTheGame.Buttons.Factory
{
    public class InlineKeyboardProviderFactory
    {
        public IInlineKeyboardProvider CreateKeyboardProvider(BotState botState)
        {
            switch (botState)
            {
                case BotState.ChoiceRole:
                    return new СhoiceRoleModeInlineKeyboardProvider();
                case BotState.ChoiceModeState:
                    return new СhoiceModeInlineKeyboardProvider();
                // Добавьте другие состояния и соответствующие провайдеры
                default:
                    throw new InvalidOperationException($"Не удалось определить InlineKeyboardProvider для состояния {botState}");
            }
        }
    }
}
