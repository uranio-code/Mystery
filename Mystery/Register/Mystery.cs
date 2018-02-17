using System.Collections.Generic;
using System.Reflection;

namespace Mystery.Register
{

    public class MysteryApp
    {


        private AssemblyRegister _AssemblyRegister;
        public event flushRequestEventHandler flushRequest;
        public delegate void flushRequestEventHandler();

        public void flushAll()
        {
            IEnumerable<Assembly> registered = default(IEnumerable<Assembly>);
            if (_AssemblyRegister != null)
            {
                registered = _AssemblyRegister.getAssemblyRegistered();
            }
            else {
                registered = new List<Assembly>();
            }

            _AssemblyRegister = new AssemblyRegister();
            foreach (Assembly ass in registered)
            {
                _AssemblyRegister.Register(ass);
            }
            if (flushRequest != null)
            {
                flushRequest();
            }
        }


        private static object _lock = new object();

        public AssemblyRegister AssemblyRegister
        {
            //Assembly register is called so many time than we avoid recursive calls
            get
            {
                if (_AssemblyRegister != null)
                    return _AssemblyRegister;
                lock (_lock)
                {
                    if (_AssemblyRegister != null)
                        return _AssemblyRegister;
                    _AssemblyRegister = new AssemblyRegister();
                }
                return _AssemblyRegister;
            }
        }

    }

}