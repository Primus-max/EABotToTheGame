namespace EABotToTheGame.Interfaces
{
    public interface IInlineKeyboardProvider
    {
        InlineKeyboardMarkup GetInlineKeyboard(params InlineKeyboardButton[] buttons);
    }
}
