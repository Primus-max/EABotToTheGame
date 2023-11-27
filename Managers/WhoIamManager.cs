namespace EABotToTheGame.Managers
{
    public class WhoIAmManager
    {
        private Dictionary<long, WhoIAm> WhoIAms;
        public WhoIAmManager()
        {
            WhoIAms = new Dictionary<long, WhoIAm>();
        }

        public WhoIAm TellMeWhoIAm(long userId)
        {
            if (WhoIAms.TryGetValue(userId, out var state))
            {
                return state;
            }

            // Если состояния нет в коллекции, по умолчанию можно асинхронно получить его из вашего сервиса
            var defaultState = GetDefaultWhoIAm(userId);
            return defaultState;
        }

        public void WhiteWhoIAm(long userId, WhoIAm state)
        {
            try
            {
                WhoIAms[userId] = state;

                Console.WriteLine($"Установил состояние {state} для : {userId}");
            }
            catch (Exception)
            {
                // Обработка ошибок записи состояния пользователя
            }
        }

        private WhoIAm GetDefaultWhoIAm(long userId)
        {
            // Здесь можно асинхронно запросить состояние по умолчанию, например, из базы данных
            // и вернуть его
            return WhoIAm.IAmCustomer; // Простой пример
        }
    }


    public enum WhoIAm
    {
        Default,
        IAmCustomer, // Заказчик
        IAmPerformer // Исполнитель
    }
}
