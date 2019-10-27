using HowLong.Extensions;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace HowLong.Converters
{
    public class TimeAccountHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var (item1, item2) = ((int, int))value;
            string headerTitle;
            switch (item1)
            {
                case 1: headerTitle = TranslationCodeExtension.GetTranslation("JanuaryText");
                    break;
                case 2:
                    headerTitle = TranslationCodeExtension.GetTranslation("FebruaryText");
                    break;
                case 3:
                    headerTitle = TranslationCodeExtension.GetTranslation("MarchText");
                    break;
                case 4:
                    headerTitle = TranslationCodeExtension.GetTranslation("AprilText");
                    break;
                case 5:
                    headerTitle = TranslationCodeExtension.GetTranslation("MayText");
                    break;
                case 6:
                    headerTitle = TranslationCodeExtension.GetTranslation("JuneText");
                    break;
                case 7:
                    headerTitle = TranslationCodeExtension.GetTranslation("JulyText");
                    break;
                case 8:
                    headerTitle = TranslationCodeExtension.GetTranslation("AugustText");
                    break;
                case 9:
                    headerTitle = TranslationCodeExtension.GetTranslation("SeptemberText");
                    break;
                case 10:
                    headerTitle = TranslationCodeExtension.GetTranslation("OctoberText");
                    break;
                case 11:
                    headerTitle = TranslationCodeExtension.GetTranslation("NovemberText");
                    break;
                default:
                    headerTitle = TranslationCodeExtension.GetTranslation("DecemberText");
                    break;
            }

            return headerTitle + $", {item2}";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DateTime.Now;
    }
}