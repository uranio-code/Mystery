using Mystery.Web;
using System;

namespace MysteryWebLogic.Authetication
{

    public class WebActionLinkedObjectException : Exception {
        public WebActionLinkedObjectException() :base() { }
        public WebActionLinkedObjectException(string message) : base(message) { }
    }

    public abstract class WebActionLinkedObjectFactory<ObjectType> where ObjectType:IDisposable
    {
        protected abstract ObjectType makeInstance();

        [ThreadStatic]
        static private ObjectType _instance;

        public ObjectType getInstance() {
            if (WebActionExecutor.current == null)
                throw new WebActionLinkedObjectException("WebActionLinkedObject can be called only while using an " + nameof(WebActionExecutor));
            if (_instance == null)
            {
                _instance = makeInstance();
                WebActionExecutor.current.disposing += () => {
                    _instance.Dispose();
                    _instance = default(ObjectType);
                };
            }
            return _instance;
        }

    }
}
