namespace EABotToTheGame.Interfaces
{
    public interface IDataWaitService<T>
    {
        Task<T> WaitForDataAsync();
        void SetData(T data);
    }
}
