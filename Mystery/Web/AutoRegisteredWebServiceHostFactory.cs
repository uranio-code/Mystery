using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Mystery.Web
{
    /// <summary>
    /// as auto registered service are coming for runtime loaded dll, they would not be recognized by name
    /// we create this factory to anyway instance by type
    /// </summary>
    public class AutoRegisteredWebServiceHostFactory : WebServiceHostFactory
    {

        private Type _type;
        public AutoRegisteredWebServiceHostFactory(Type t)
        {
            _type = t;
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            //we ignore constructorString as the type should be passed with the factory
            return base.CreateServiceHost(_type, baseAddresses);
        }

    }
}
