using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong.Extensions
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShadowView
	{
		public ShadowView() => InitializeComponent();
		private void NavShadow_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			var info = args.Info;
			var surface = args.Surface;
			var canvas = surface.Canvas;

			canvas.Clear(SKColor.Empty);

			using (var path = new SKPath())
			{
				path.MoveTo(0, -6);
				path.LineTo(info.Width, -6);
				var shadowPaint = new SKPaint
				{
					Style = SKPaintStyle.Stroke,
					StrokeWidth = 6,
					Color = SKColor.Parse("#FFFFFF"),
					ImageFilter = SKImageFilter.CreateDropShadow(
						0,
						6,
						0,
						6,
						SKColor.Parse("#444444"),
						SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
				};
				canvas.DrawPath(path, shadowPaint);
			}
		}
	}
}