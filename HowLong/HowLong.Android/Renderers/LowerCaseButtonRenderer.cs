using Android.Content;
using HowLong.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Xamarin.Forms.Button;

[assembly: ExportRenderer(typeof(Button), typeof(LowerCaseButtonRenderer))]
namespace HowLong.Droid.Renderers
{
	public class LowerCaseButtonRenderer : ButtonRenderer
	{
		public LowerCaseButtonRenderer(Context context) : base(context) { }
    }
}