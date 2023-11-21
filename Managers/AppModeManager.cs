namespace EABotToTheGame
{
    public class AppModeManager
    {
        private Dictionary<long, AppMode> UserModes;

        public AppModeManager()
        {
            UserModes = new Dictionary<long, AppMode>();
        }

        public AppMode GetCurrentMode(long userId)
        {
            if (UserModes.TryGetValue(userId, out var mode))
            {
                return mode;
            }

            return GetDefaultUserMode();
        }

        public void SetUserMode(long userId, AppMode mode)
        {
            UserModes[userId] = mode;
        }

        private AppMode GetDefaultUserMode()
        {
            return AppMode.AutoMode; // Мод по умолчанию
        }
    }

    public enum AppMode
    {
        AutoMode,
        ManualMode
    }

}
