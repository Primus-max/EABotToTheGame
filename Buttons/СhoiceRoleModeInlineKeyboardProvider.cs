namespace EABotToTheGame.Buttons
{
    public class СhoiceRoleModeInlineKeyboardProvider:IInlineKeyboardProvider
    {
        public InlineKeyboardMarkup GetButtonsInlineKeyboard()
        {
            var button1 = InlineKeyboardButton.WithCallbackData("Заказчик", "customer");
            var button2 = InlineKeyboardButton.WithCallbackData("Исполнитель", "performer");

            var inlineKeyboard = new InlineKeyboardMarkup(new[] { new[] { button1, button2 } });
            return inlineKeyboard;
        }
    }
}
