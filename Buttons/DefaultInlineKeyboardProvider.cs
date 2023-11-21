namespace EABotToTheGame.Buttons
{
    public class DefaultInlineKeyboardProvider : IInlineKeyboardProvider
    {
        public InlineKeyboardMarkup GetButtonsInlineKeyboard()
        {
            var button1 = InlineKeyboardButton.WithCallbackData("АвтоРежим", "Button1Callback");
            var button2 = InlineKeyboardButton.WithCallbackData("РучнойРежим", "Button2Callback");

            var inlineKeyboard = new InlineKeyboardMarkup(new[] { new[] { button1, button2 } });
            return inlineKeyboard;
        }
    }
}
