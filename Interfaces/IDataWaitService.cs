namespace EABotToTheGame.Interfaces
{
    public interface IDataWaitService<T>
    {
        Task<T> WaitForDataAsync();
        void SetData(TaskCompletionSource<T> dataReceivedTaskCompletionSource, T data);
    }
}
