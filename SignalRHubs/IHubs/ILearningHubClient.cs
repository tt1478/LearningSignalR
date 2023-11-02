namespace SignalRServer.IHubs
{
    public interface ILearningHubClient
    {
        Task ReceiveMessage(string message);
    }
}
