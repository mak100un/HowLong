using HowLong.DependencyServices;
using HowLong.Droid.DependencyServices;
using System.IO;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(GetSqLitePath))]
namespace HowLong.Droid.DependencyServices
{
    public class GetSqLitePath : IGetSqLitePath
    {
        public string GetDatabasePath(string sqLiteFilename) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), sqLiteFilename);
    }
}