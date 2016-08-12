using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterSentiments.Startup))]
namespace TwitterSentiments
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
