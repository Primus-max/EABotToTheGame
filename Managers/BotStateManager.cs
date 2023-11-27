namespace EABotToTheGame.Managers
{
    public class BotStateManager
    {
        // Свойства бота
        private BotState _currentState;
        private BotState _previousState;
        private BotState _nextState;
              

        public BotState GetCurrentState()
        {
            return _nextState;
        }

        public BotState GetPreviousState()
        {
            return _previousState;
        }

        public BotState GetNextState()
        {
            return _nextState;
        }

        public void SetCurrentState(BotState newState)
        {
            _previousState = _currentState;
            _currentState = newState;
        }

        public void SetNextState(BotState newState)
        {
            _nextState = newState;  
            _previousState = _currentState;
        }

        public void SetPreviousState(BotState newState)
        {
            _previousState = newState;            
        }

        public enum BotState
        {
            StartScreenState,
            AutoModeState,
            ManualModeState,
            ChoiceRole,
            ChoiceModeState,
        }
    }
}

