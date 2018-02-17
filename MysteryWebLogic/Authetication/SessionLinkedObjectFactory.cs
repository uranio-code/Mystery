using System.Collections.Generic;
using Mystery.Register;
using Mystery.Web;

namespace MysteryWebLogic.Authetication
{



    public abstract class SessionLinkedObjectFactory<ObjectType> 
    {
        protected abstract ObjectType makeInstance();

        static private Dictionary<MysterySession,ObjectType> map  = new Dictionary<MysterySession, ObjectType>();
        static private object _lock = new object();

        static SessionLinkedObjectFactory(){
            MysterySession.SessionEnd += flushSession;
        }

        static void flushSession(MysterySession session){
            lock (_lock) map.Remove(session);
        }

        public ObjectType getInstance() {
            var session = this.getGlobalObject<MysterySession>();
            if (map.ContainsKey(session))
                return map[session];

            lock (_lock) {
                if (map.ContainsKey(session))
                    return map[session];

                var result = makeInstance();
                map[session] = result;
                return result;
            }

        }

    }
}
