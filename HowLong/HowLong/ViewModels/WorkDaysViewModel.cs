using HowLong.Extensions;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class WorkDaysViewModel : ViewModelBase
    {
        public bool IsMonday
        {
            get => Settings.IsMonday;
            set
            {
                Settings.IsMonday = value;
                this.RaisePropertyChanged(nameof(IsMonday));
            }
        }
        public double MondayStart
        {
            get => Settings.MondayStart;
            set
            {
                Settings.MondayStart = value;
                this.RaisePropertyChanged(nameof(MondayStart));
            }
        }
        public double MondayEnd
        {
            get => Settings.MondayEnd;
            set
            {
                Settings.MondayEnd = value;
                this.RaisePropertyChanged(nameof(MondayEnd));
            }
        }
        public bool IsTuesday
        {
            get => Settings.IsTuesday;
            set
            {
                Settings.IsTuesday = value;
                this.RaisePropertyChanged(nameof(IsTuesday));
            }
        }
        public double TuesdayStart
        {
            get => Settings.TuesdayStart;
            set
            {
                Settings.TuesdayStart = value;
                this.RaisePropertyChanged(nameof(TuesdayStart));
            }
        }
        public double TuesdayEnd
        {
            get => Settings.TuesdayEnd;
            set
            {
                Settings.TuesdayEnd = value;
                this.RaisePropertyChanged(nameof(TuesdayEnd));
            }
        }
        public bool IsWednesday
        {
            get => Settings.IsWednesday;
            set
            {
                Settings.IsWednesday = value;
                this.RaisePropertyChanged(nameof(IsWednesday));
            }
        }
        public double WednesdayStart
        {
            get => Settings.WednesdayStart;
            set
            {
                Settings.WednesdayStart = value;
                this.RaisePropertyChanged(nameof(WednesdayStart));
            }
        }
        public double WednesdayEnd
        {
            get => Settings.WednesdayEnd;
            set
            {
                Settings.WednesdayEnd = value;
                this.RaisePropertyChanged(nameof(WednesdayEnd));
            }
        }
        public double ThursdayStart
        {
            get => Settings.ThursdayStart;
            set
            {
                Settings.ThursdayStart = value;
                this.RaisePropertyChanged(nameof(ThursdayStart));
            }
        }
        public bool IsThursday
        {
            get => Settings.IsThursday;
            set
            {
                Settings.IsThursday = value;
                this.RaisePropertyChanged(nameof(IsThursday));
            }
        }
        public double ThursdayEnd
        {
            get => Settings.ThursdayEnd;
            set
            {
                Settings.ThursdayEnd = value;
                this.RaisePropertyChanged(nameof(ThursdayEnd));
            }
        }
        public double FridayStart
        {
            get => Settings.FridayStart;
            set
            {
                Settings.FridayStart = value;
                this.RaisePropertyChanged(nameof(FridayStart));
            }
        }
        public double FridayEnd
        {
            get => Settings.FridayEnd;
            set
            {
                Settings.FridayEnd = value;
                this.RaisePropertyChanged(nameof(FridayEnd));
            }
        }
        public bool IsFriday
        {
            get => Settings.IsFriday;
            set
            {
                Settings.IsFriday = value;
                this.RaisePropertyChanged(nameof(IsFriday));
            }
        }
        public bool IsSaturday
        {
            get => Settings.IsSaturday;
            set
            {
                Settings.IsSaturday = value;
                this.RaisePropertyChanged(nameof(IsSaturday));
            }
        }
        public double SaturdayStart
        {
            get => Settings.SaturdayStart;
            set
            {
                Settings.SaturdayStart = value;
                this.RaisePropertyChanged(nameof(SaturdayStart));
            }
        }
        public double SaturdayEnd
        {
            get => Settings.SaturdayEnd;
            set
            {
                Settings.SaturdayEnd = value;
                this.RaisePropertyChanged(nameof(SaturdayEnd));
            }
        }
        public bool IsSunday
        {
            get => Settings.IsSunday;
            set
            {
                Settings.IsSunday = value;
                this.RaisePropertyChanged(nameof(IsSunday));
            }
        }
        public double SundayStart
        {
            get => Settings.SundayStart;
            set
            {
                Settings.SundayStart = value;
                this.RaisePropertyChanged(nameof(SundayStart));
            }
        }
        public double SundayEnd
        {
            get => Settings.SundayEnd;
            set
            {
                Settings.SundayEnd = value;
                this.RaisePropertyChanged(nameof(SundayEnd));
            }
        }

        public ReactiveCommand<Unit, Unit> InfoCommand { get; internal set; }

        public WorkDaysViewModel()
        {
            InfoCommand = ReactiveCommand.CreateFromTask(async() => await Application.Current.MainPage.DisplayAlert(
                    string.Empty,
                    TranslationCodeExtension.GetTranslation("TimePickerText"),
                    TranslationCodeExtension.GetTranslation("OkText")));
            this.WhenAnyValue(x => x.MondayStart)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (MondayStart > MondayEnd) MondayEnd = MondayStart;
                });
            this.WhenAnyValue(x => x.MondayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (MondayStart > MondayEnd) MondayStart = MondayEnd;
                });

            this.WhenAnyValue(x => x.TuesdayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (TuesdayStart > TuesdayEnd) TuesdayEnd = TuesdayStart;
               });
            this.WhenAnyValue(x => x.TuesdayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (TuesdayStart > TuesdayEnd) TuesdayStart = TuesdayEnd;
                });

            this.WhenAnyValue(x => x.WednesdayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (WednesdayStart > WednesdayEnd) WednesdayEnd = WednesdayStart;
               });
            this.WhenAnyValue(x => x.WednesdayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (WednesdayStart > WednesdayEnd) WednesdayStart = WednesdayEnd;
                });

            this.WhenAnyValue(x => x.ThursdayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (ThursdayStart > ThursdayEnd) ThursdayEnd = ThursdayStart;
               });
            this.WhenAnyValue(x => x.ThursdayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (ThursdayStart > ThursdayEnd) ThursdayStart = ThursdayEnd;
                });

            this.WhenAnyValue(x => x.FridayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (FridayStart > FridayEnd) FridayEnd = FridayStart;
               });
            this.WhenAnyValue(x => x.FridayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (FridayStart > FridayEnd) FridayStart = FridayEnd;
                });

            this.WhenAnyValue(x => x.SaturdayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (SaturdayStart > SaturdayEnd) SaturdayEnd = SaturdayStart;
               });
            this.WhenAnyValue(x => x.SaturdayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (SaturdayStart > SaturdayEnd) SaturdayStart = SaturdayEnd;
                });

            this.WhenAnyValue(x => x.SundayStart)
               .Select(x => x)
               .Subscribe(_ =>
               {
                   if (SundayStart > SundayEnd) SundayEnd = SundayStart;
               });
            this.WhenAnyValue(x => x.SundayEnd)
                .Select(x => x)
                .Subscribe(_ =>
                {
                    if (SundayStart > SundayEnd) SundayStart = SundayEnd;
                });
        }
    }
}
