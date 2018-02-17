using System;
using Mystery.Register;

namespace Mystery.Content
{

    [GlobalAvalibleObject()]
    interface IUidProvider
    {
        string getUid();
        void registerUid(string uid, Guid guid);
    }


}