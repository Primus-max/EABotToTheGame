namespace EABotToTheGame.Services
{
    public class DataWaitService
    {
        private TaskCompletionSource<string> _stringDataCompletionSource;
        private TaskCompletionSource<AuthData> _authDataCompletionSource;
        private TaskCompletionSource<int> _indexDataCompletionSource;

        public DataWaitService()
        {
            _stringDataCompletionSource = new TaskCompletionSource<string>();
            _authDataCompletionSource = new TaskCompletionSource<AuthData>();
            _indexDataCompletionSource = new TaskCompletionSource<int>();
        }

        public Task<string> WaitForStringDataAsync()
        {
            return _stringDataCompletionSource.Task;
        }

        public void SetStringData(string data)
        {
            _stringDataCompletionSource.TrySetResult(data);
            _stringDataCompletionSource = new TaskCompletionSource<string>(); // сбрасываем для повторного использования
        }

        public Task<AuthData> WaitForAuthDataAsync()
        {
            return _authDataCompletionSource.Task;
        }

        public void SetAuthData(AuthData data)
        {
            _authDataCompletionSource.TrySetResult(data);
            _authDataCompletionSource = new TaskCompletionSource<AuthData>(); // сбрасываем для повторного использования
        }

        public Task<int> WaitForIndexDataAsync()
        {
            return _indexDataCompletionSource.Task;
        }

        public void SetIndexData(int data)
        {
            _indexDataCompletionSource.TrySetResult(data);
            _indexDataCompletionSource = new TaskCompletionSource<int>(); // сбрасываем для повторного использования
        }
    }

}
