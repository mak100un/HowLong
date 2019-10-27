using HowLong.Extensions;
using HowLong.Models;
using HowLong.Services;
using ReactiveUI;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCard
    {
        public AccountCard()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => ViewModel != null)
                .DistinctUntilChanged()
                .Do(PopulateFromViewModel)
                .Subscribe();
        }

        private void PopulateFromViewModel(HistoryAccount vm)
        {
            DayLbl.Text = Settings.Language.IsNullOrEmptyOrWhiteSpace()
                ? $"{vm.TimeAccount.WorkDate:M}, {DateService.DayShortName(vm.TimeAccount.WorkDate.DayOfWeek)}"
                : vm.TimeAccount.WorkDate.ToString("M", CultureInfo.GetCultureInfo(Settings.Language)) 
                + $", {DateService.DayShortName(vm.TimeAccount.WorkDate.DayOfWeek)}";

            StartWorkSpn.Text = vm.TimeAccount.StartWorkTime.ToString(@"hh\:mm");
            EndWorkSpn.Text = vm.TimeAccount.EndWorkTime.ToString(@"hh\:mm");
            if (vm.TimeAccount.Breaks==null || vm.TimeAccount.Breaks.Count == 0)
                DinnerStck.IsVisible = false;
            else
            {
                var orderedBreaks = vm.TimeAccount.Breaks
                    .OrderBy(x => x.StartBreakTime)
                    .ToArray();
                for (var i=0; i < orderedBreaks.Length; i++)
                {
                    if (i == 0) DinnerLbl.Text = TimeSpan.FromMinutes(orderedBreaks[i].StartBreakTime).ToString(@"hh\:mm")
                               + " - " + TimeSpan.FromMinutes(orderedBreaks[i].EndBreakTime).ToString(@"hh\:mm");
                    else DinnerLbl.Text += ", " + TimeSpan.FromMinutes(orderedBreaks[i].StartBreakTime).ToString(@"hh\:mm")
                               + " - " + TimeSpan.FromMinutes(orderedBreaks[i].EndBreakTime).ToString(@"hh\:mm");
                }
                BreakLbl.Text = orderedBreaks.Length > 1
                        ? TranslationCodeExtension.GetTranslation("BreaksText")
                        : TranslationCodeExtension.GetTranslation("BreakText");
            }
            OverWorkTitle.Text = vm.DayOverWork >= 0
                ? TranslationCodeExtension.GetTranslation("OverworkText") + " "
                : TranslationCodeExtension.GetTranslation("WeaknessesText") + " ";
            OverWorkLbl.TextColor = vm.DayOverWork >= 0
               ? Color.ForestGreen
               : (Color)Application.Current.Resources["AccentColor"];
            OverWorkLbl.Text = TimeSpan.FromMinutes(vm.DayOverWork).ToString(@"hh\:mm");
        }
    }
}