namespace EABotToTheGame.Buttons
{
    public class СhoiceModeInlineKeyboardProvider : IInlineKeyboardProvider
    {
        public InlineKeyboardMarkup GetButtonsInlineKeyboard()
        {
            var button1 = InlineKeyboardButton.WithCallbackData("АвтоРежим", "AutoMode");
            var button2 = InlineKeyboardButton.WithCallbackData("РучнойРежим", "ManualMode");

            var inlineKeyboard = new InlineKeyboardMarkup(new[] { new[] { button1, button2 } });
            return inlineKeyboard;
        }
    }
}
