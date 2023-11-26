namespace EABotToTheGame.Services
{
    public class DataWaitService<T> : IDataWaitService<T>
    {
        private TaskCompletionSource<T> _dataReceivedTaskCompletionSource;

        public DataWaitService()
        {
            _dataReceivedTaskCompletionSource = new TaskCompletionSource<T>();
        }

        public async Task<T> WaitForDataAsync()
        {
            return await _dataReceivedTaskCompletionSource.Task;
        }

        public void SetData(T data)
        {
            _dataReceivedTaskCompletionSource.TrySetResult(data);
        }
    }
}
