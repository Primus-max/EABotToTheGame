namespace EABotToTheGame
{
    public class AppModeManager
    {
        private Dictionary<long, AppMode> UserModes;

        public AppModeManager()
        {
            UserModes = new Dictionary<long, AppMode>();
        }

        public AppMode GetCurrentAppMode(long userId)
        {
            if (UserModes.TryGetValue(userId, out var mode))
            {
                return mode;
            }

            return GetDefaultAppMode();
        }

        public void SetAppMode(long userId, AppMode mode)
        {
            UserModes[userId] = mode;
        }

        private AppMode GetDefaultAppMode()
        {
            return AppMode.AutoMode; // Мод по умолчанию
        }
    }

    public enum AppMode
    {
        Default,
        AutoMode,
        ManualMode        
    }

}
