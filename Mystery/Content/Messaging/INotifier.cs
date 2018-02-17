using Mystery.Register;

namespace Mystery.Messaging
{
    [GlobalAvalibleObject]
    public interface INotifier
    {
        void sendMessage(IUserMessage message);
    }
}
