namespace EABotToTheGame.Managers
{
    public class WhoIAmManager
    {
        private Dictionary<long, UserState> UserStates;
        public WhoIAmManager()
        {
            UserStates = new Dictionary<long, UserState>();
        }

        public UserState TellMeWhoIAm(long userId)
        {
            if (UserStates.TryGetValue(userId, out var state))
            {
                return state;
            }

            // Если состояния нет в коллекции, по умолчанию можно асинхронно получить его из вашего сервиса
            var defaultState = GetDefaultWhoIAm(userId);
            return defaultState;
        }

        public void WhiteWhoIAm(long userId, UserState state)
        {
            try
            {
                UserStates[userId] = state;

                Console.WriteLine($"Установил состояние {state} для : {userId}");
            }
            catch (Exception)
            {
                // Обработка ошибок записи состояния пользователя
            }
        }

        private UserState GetDefaultWhoIAm(long userId)
        {
            // Здесь можно асинхронно запросить состояние по умолчанию, например, из базы данных
            // и вернуть его
            return WhoIAm.Customer; // Простой пример
        }
    }


    public enum WhoIAm
    {
        Customer, // Заказчик
        Performer // Исполнитель
    }
}
