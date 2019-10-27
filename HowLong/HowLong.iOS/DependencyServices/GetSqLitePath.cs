using System;
using System.IO;
using Xamarin.Forms;
using HowLong.DependencyServices;
using HowLong.iOS.DependencyServices;

[assembly: Dependency(typeof(GetSqLitePath))]
namespace HowLong.iOS.DependencyServices
{
	public class GetSqLitePath : IGetSqLitePath
	{
		public string GetDatabasePath(string sqLiteFilename) => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", sqLiteFilename);
	}
}