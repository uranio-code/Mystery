using Mystery.Register;

namespace Mystery.Messaging
{
    [GlobalAvalibleObjectImplementation(
        implementation_of = typeof(IMessageManager),
        overrides_exsisting = true,
        singleton = true)]
    public class DefaultMessageManager : IMessageManager
    {

        public DefaultMessageManager() { }


        public ICodifiedMessage getCodifiedMessage(string code, string language)
        {
            ICodifiedMessage _ret = new BaseCodifiedMessage();
            _ret.code = code;
            _ret.language = language;
            return _ret;
        }
    }
}
