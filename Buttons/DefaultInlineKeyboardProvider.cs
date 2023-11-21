namespace EABotToTheGame.Buttons
{
    public class DefaultInlineKeyboardProvider : IInlineKeyboardProvider
    {
        public InlineKeyboardMarkup GetInlineKeyboard(params InlineKeyboardButton[] buttons)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(buttons.Select(button => new[] { button }).ToArray());
            return inlineKeyboard;
        }
    }
}
