using HowLong.DependencyServices;
using HowLong.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApp))]
namespace HowLong.iOS.DependencyServices
{
    public class CloseApp : ICloseApp
    {
        public void Close() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
    }
}