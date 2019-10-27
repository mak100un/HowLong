using Plugin.Multilingual;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong.Extensions
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private const string ResourceId = "HowLong.Resources.Resource";

        private static readonly Lazy<ResourceManager> ResManager = new Lazy<ResourceManager>(() =>
        new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null) throw new NotImplementedException();
            
            var ci = Settings.Language.IsNullOrEmptyOrWhiteSpace()
                ? CrossMultilingual.Current.CurrentCultureInfo
                : new CultureInfo(Settings.Language);
            var translation = ResManager.Value.GetString(Text, ci);

            return translation ?? throw new NotImplementedException();
        }
    }
}