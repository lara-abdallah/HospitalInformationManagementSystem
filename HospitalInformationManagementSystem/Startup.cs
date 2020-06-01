using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HospitalInformationManagementSystem.Startup))]
namespace HospitalInformationManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
