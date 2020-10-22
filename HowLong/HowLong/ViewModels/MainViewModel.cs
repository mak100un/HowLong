using HowLong.Data;
using HowLong.Extensions;
using HowLong.Models;
using HowLong.Navigation;
using HowLong.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using HowLong.Values;
using Plugin.LatestVersion;
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly Func<HistoryViewModel> _historyFactory;
        private readonly Func<TimeAccount, double, bool, AccountViewModel> _accountFactory;
        private readonly Func<SettingsViewModel> _settingsFactory;
        private readonly TimeAccountingContext _timeAccountingContext;
        public ReactiveCommand<Unit, Unit> HistoryCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> CurrentCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> SettingsCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> RateCommand { get; internal set; }

        public MainViewModel
        (
            INavigationService navigationService,
            TimeAccountingContext timeAccountingContext,
            Func<SettingsViewModel> settingsFactory,
            Func<TimeAccount, double, bool, AccountViewModel> accountFactory,
            Func<HistoryViewModel> historyFactory
        )
        {
            _historyFactory = historyFactory;
            _accountFactory = accountFactory;
            _settingsFactory = settingsFactory;
            _timeAccountingContext = timeAccountingContext;
            _navigationService = navigationService;
            HistoryCommand = ReactiveCommand.CreateFromTask(HistoryExecuteAsync);
            CurrentCommand = ReactiveCommand.CreateFromTask(CurrentExecuteAsync);
            SettingsCommand = ReactiveCommand.CreateFromTask(SettingsExecuteAsync);
            RateCommand = ReactiveCommand.CreateFromTask(RateExecuteAsync);
        }

        private static async Task RateExecuteAsync() => 
            await CrossLatestVersion.Current.OpenAppInStore(BaseValue.PackageName);
          

        private async Task SettingsExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);
            await _navigationService.NavigateToAsync
                    (
                _settingsFactory()
                    );
            await Task.Delay(150);
            IsEnable = true;
        }

        private async Task CurrentExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);
            var currentDate = DateTime.Today;
            var currentAccounting = await _timeAccountingContext.TimeAccounts
                .Include(x => x.Breaks)
                .SingleOrDefaultAsync(x => x.WorkDate == currentDate && !x.IsClosed)
                .ConfigureAwait(false);
            var todayWorks = await _timeAccountingContext.TimeAccounts.Where(x => x.WorkDate == currentDate && x.IsClosed).ToArrayAsync();
            var workedTime = todayWorks?.Sum(v => v.Breaks == null
                                    ? (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                    : (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                    - v.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                    ?? 0;
            if (currentAccounting != null)
            {
                currentAccounting.Breaks = new ObservableCollection<Break>
                    (
                        currentAccounting.Breaks.OrderBy(y => y.StartBreakTime)
                    );
                await _navigationService.NavigateToAsync
                (
                    _accountFactory(currentAccounting, workedTime, false)
                );
                await Task.Delay(150);
                IsEnable = true;
                return;
            }
            var isWorkDay = DateService.IsWorking(currentDate.DayOfWeek);
            if (!isWorkDay)
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                    TranslationCodeExtension.GetTranslation("WeekendAlertTitle"),
                    TranslationCodeExtension.GetTranslation("WeekendAlertText"),
                    TranslationCodeExtension.GetTranslation("YesWeekendAlertText"),
                    TranslationCodeExtension.GetTranslation("NoText"));
                if (!result)
                {
                    IsEnable = true;
                    return;
                }
                await Task.Delay(50);
            }
            var previousAccount = await _timeAccountingContext.TimeAccounts
                .Include(x => x.Breaks)
                .OrderByDescending(x=>x.WorkDate)
                .FirstOrDefaultAsync(x => x.WorkDate < currentDate && !x.IsClosed)
                .ConfigureAwait(false);
            if (previousAccount != null && workedTime <= default(double))
            {
                if (previousAccount.IsStarted)
                {
                    previousAccount.IsClosed = true;
                    previousAccount.EndWorkTime = new TimeSpan(23, 59, 59);

                    if (previousAccount.StartWorkTime > previousAccount.EndWorkTime) previousAccount.StartWorkTime = previousAccount.EndWorkTime;

                    var workTime = previousAccount.Breaks == null
                        ? (previousAccount.EndWorkTime - previousAccount.StartWorkTime).TotalMinutes
                        : (previousAccount.EndWorkTime - previousAccount.StartWorkTime).TotalMinutes
                            - previousAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                    previousAccount.OverWork = previousAccount.IsWorking
                        ? workTime - DateService.WorkingTime(previousAccount.WorkDate.DayOfWeek)
                        : workTime;
                    _timeAccountingContext.Entry(previousAccount).State = EntityState.Modified;
                }
                else _timeAccountingContext.TimeAccounts.Remove(previousAccount);
                
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }

            var notClosedAccounts = await _timeAccountingContext.TimeAccounts
                .Include(x => x.Breaks)
                .Where(x => !x.IsClosed)
                .ToArrayAsync()
                .ConfigureAwait(false); // Can be if user change date on the device
            if (notClosedAccounts.Length != 0)
            {
                foreach (var notClosedAccount in notClosedAccounts)
                {
                    if (notClosedAccount.Breaks.Count != 0) _timeAccountingContext.Breaks.RemoveRange(notClosedAccount.Breaks);
                    _timeAccountingContext.TimeAccounts.Remove(notClosedAccount);
                }
            }

            currentAccounting = new TimeAccount { WorkDate = currentDate, IsWorking = isWorkDay};
            _timeAccountingContext.TimeAccounts.Add(currentAccounting);
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);

            await _navigationService.NavigateToAsync
            (
                _accountFactory(currentAccounting, workedTime, false)
            );

            await Task.Delay(150);
            IsEnable = true;
        }

        private async Task HistoryExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);
            var history = _historyFactory();
            await Task.Delay(300);
            await _navigationService.NavigateToAsync
                    (
                       history
                    );
            await Task.Delay(150);
            IsEnable = true;
        }
    }
}
