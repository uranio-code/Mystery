

using Mystery.Register;

namespace Mystery.Messaging
{
    [GlobalAvalibleObject]
    public interface IMessageManager
    {
        ICodifiedMessage getCodifiedMessage(string code, string language);
    }
}
