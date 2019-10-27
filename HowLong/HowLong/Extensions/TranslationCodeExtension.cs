using Plugin.Multilingual;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace HowLong.Extensions
{
    public class TranslationCodeExtension
    {
        private const string ResourceId = "HowLong.Resources.Resource";

        private static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(() => 
        new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));

        public static string GetTranslation(string text)
        {
            if (text == null) throw new NotImplementedException();
            var ci = Settings.Language.IsNullOrEmptyOrWhiteSpace()
                    ? CrossMultilingual.Current.CurrentCultureInfo
                    : new CultureInfo(Settings.Language);
                var translation = ResMgr.Value.GetString(text, ci);

                return translation ?? throw new NotImplementedException();
        }
    }
}
