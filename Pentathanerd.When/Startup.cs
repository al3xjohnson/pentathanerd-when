using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Pentathanerd.When.Startup))]
namespace Pentathanerd.When
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
