using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;
using Mystery.Web;
using Mystery;

namespace Mystery.Applications
{
    /// <summary>
    /// each module should have 1 implementation of this interface which will provide the list
    /// of application in the module
    /// </summary>
    public interface IApplicationProvider
    {
        IEnumerable<MysteryApplication> getApplications();
    }

    public class EnsureApplicationsAction : BaseMysteryAction<IEnumerable<string>,IEnumerable<MysteryApplication>>, ICanRunWithOutLogin
    {
        protected override ActionResult<IEnumerable<MysteryApplication>> ActionImplemetation()
        {
            if (input == null) input = new HashSet<string>();
            var result = new List<MysteryApplication>();

            var providers = this.getMystery().AssemblyRegister.getChildTypes<IApplicationProvider>();
            var cd = this.getGlobalObject<IContentDispatcher>();

            var application_in_db = new HashSet<MysteryApplication>( cd.GetAll<MysteryApplication>());
            var application_in_dlls = new HashSet<MysteryApplication>();
            foreach (var provider_type in providers) {
                if (!provider_type.IsClass || provider_type.IsAbstract)
                    continue;
                var provider = (IApplicationProvider)Activator.CreateInstance(provider_type);
                //we mark the origins
                var applications = new HashSet<MysteryApplication>(provider.getApplications());
                foreach (var app in applications) {
                    app.dll_name = provider_type.Assembly.GetName().Name;
                }
                application_in_dlls.AddRange(applications);
            }

            //new, we add to the db
            foreach(var app in application_in_dlls.Except(application_in_db))
            {
                //those required to be active they shall
                if (input.Contains(app.name))
                    app.active = true;
                cd.Add(app);
                result.Add(app);
            }

            //old we deactivate it
            foreach (var app in application_in_db.Except(application_in_dlls))
            {
                if (!app.active)
                    continue;
                app.active = false;
                cd.Add(app);
                result.Add(app);
            }

            
            


            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            //an experiment, if this comment is still here in a year it might be working ;)
            //I want to try to have this action only be fired on boot
            //while booting mystery should not have user or if yes it is the system
            return user == null || user.account_type == Users.UserType.system;
        }
    }

}
