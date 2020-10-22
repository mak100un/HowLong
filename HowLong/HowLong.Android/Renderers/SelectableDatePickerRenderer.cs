using Android.App;
using Android.Content;
using HowLong.Controls;
using HowLong.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DatePicker = Xamarin.Forms.DatePicker;

[assembly: ExportRenderer(typeof(SelectableDatePicker), typeof(SelectableDatePickerRenderer))]
namespace HowLong.Droid.Renderers
{
    public class SelectableDatePickerRenderer : DatePickerRenderer
    {
        internal SelectableDatePicker Picker => Element as SelectableDatePicker;
        public SelectableDatePickerRenderer(Context context) : base(context) { }

        protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
        {
            DatePicker view = Element;
            var dialog = new DatePickerDialog(Context, (o, e) =>
            {
                view.Date = e.Date;
                Picker.SelectedDate = null;
                Picker.SelectedDate = e.Date;
                ((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            }, year, month, day);

            return dialog;
        }
    }
}