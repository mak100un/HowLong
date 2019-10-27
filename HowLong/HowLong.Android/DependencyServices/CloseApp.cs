using HowLong.DependencyServices;
using HowLong.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApp))]
namespace HowLong.Droid.DependencyServices
{
    public class CloseApp : ICloseApp
    {
        public void Close() => Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
    }
}