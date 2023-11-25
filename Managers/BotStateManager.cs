//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EABotToTheGame.Managers
//{
//    public class BotStateManager
//    {
//        private Dictionary<long, BotState> BotStates;
//        public BotStateManager()
//        {
//            BotStates = new Dictionary<long, BotState>();
//        }

//        public BotState TellMeWhoIAm(long userId)
//        {
//            if (BotStates.TryGetValue(userId, out var state))
//            {
//                return state;
//            }

//            // Если состояния нет в коллекции, по умолчанию можно асинхронно получить его из вашего сервиса
//            var defaultState = GetDefaultWhoIAm(userId);
//            return defaultState;
//        }

//        public void WhiteWhoIAm(long userId, WhoIAm state)
//        {
//            try
//            {
//                WhoIAms[userId] = state;

//                Console.WriteLine($"Установил состояние {state} для : {userId}");
//            }
//            catch (Exception)
//            {
//                // Обработка ошибок записи состояния пользователя
//            }
//        }

//        private WhoIAm GetDefaultWhoIAm(long userId)
//        {
//            // Здесь можно асинхронно запросить состояние по умолчанию, например, из базы данных
//            // и вернуть его
//            return WhoIAm.Customer; // Простой пример
//        }
//    }


//    public enum BotState
//    {
//        StartScreenState,
//        AutoModeState,
//        ManualModeState
//    }
//}

