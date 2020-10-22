using System;
using Xamarin.Forms;

namespace HowLong.Controls
{
    public class SelectableDatePicker : DatePicker
    {
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
           propertyName: nameof(SelectedDate),
           returnType: typeof(DateTime?),
           declaringType: typeof(SelectableDatePicker),
           defaultValue: default);
    }
}
