namespace EABotToTheGame.Managers
{
    public class UserStateManager
    {
        private Dictionary<long, UserState> UserStates;
        public UserStateManager()
        {
            UserStates = new Dictionary<long, UserState>();
        }

        public UserState GetUserState(long userId)
        {
            if (UserStates.TryGetValue(userId, out var state))
            {
                return state;
            }

            // Если состояния нет в коллекции, по умолчанию можно асинхронно получить его из вашего сервиса
            var defaultState =  GetDefaultUserState(userId);
            return defaultState;
        }

        public void SetUserState(long userId, UserState state)
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

        private UserState GetDefaultUserState(long userId)
        {
            // Здесь можно асинхронно запросить состояние по умолчанию, например, из базы данных
            // и вернуть его
            return UserState.Start; // Простой пример
        }
    }


    public enum UserState
    {
        Start,
        ExpectedCodeAuthorizations,
        ExpectedEmailAuthorizationsData,
    }
}
