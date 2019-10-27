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
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly Func<HistoryViewModel> _historyFactory;
        private readonly Func<TimeAccount, double, AccountViewModel> _accountFactory;
        private readonly Func<SettingsViewModel> _settingsFactory;
        private readonly TimeAccountingContext _timeAccountingContext;
        public ReactiveCommand<Unit, Unit> HistoryCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> CurrentCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> SettingsCommand { get; internal set; }

        public MainViewModel
        (
            INavigationService navigationService,
            TimeAccountingContext timeAccountingContext,
            Func<SettingsViewModel> settingsFactory,
            Func<TimeAccount, double, AccountViewModel> accountFactory,
            Func<HistoryViewModel> historyFactory
        )
        {
            _historyFactory = historyFactory;
            _accountFactory = accountFactory;
            _settingsFactory = settingsFactory;
            _timeAccountingContext = timeAccountingContext;
            _navigationService = navigationService;
            HistoryCommand = ReactiveCommand.CreateFromTask(HistoryExecute);
            CurrentCommand = ReactiveCommand.CreateFromTask(CurrentExecute);
            SettingsCommand = ReactiveCommand.CreateFromTask(SettingsExecute);
        }

        private async Task SettingsExecute()
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

        private async Task CurrentExecute()
        {
            IsEnable = false;
            await Task.Delay(100);
            var currentDate = DateTime.Today;
            var currentAccounting = await _timeAccountingContext.TimeAccounts
                .Include(x => x.Breaks)
                .SingleOrDefaultAsync(x => x.WorkDate == currentDate && !x.IsClosed)
                .ConfigureAwait(false);
            var workedTime = await _timeAccountingContext.TimeAccounts.Where(x => x.WorkDate == currentDate && x.IsClosed)
                    .SumAsync(v => v.Breaks == null
                                    ? (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                    : (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                    - v.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                    .ConfigureAwait(false);
            if (currentAccounting != null)
            {
                currentAccounting.Breaks = new ObservableCollection<Break>
                    (
                        currentAccounting.Breaks.OrderBy(y => y.StartBreakTime)
                    );
                await _navigationService.NavigateToAsync
                (
                    _accountFactory(currentAccounting, workedTime)
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
                .OrderByDescending(x=>x.WorkDate)
                .FirstOrDefaultAsync(x => x.WorkDate < currentDate)
                .ConfigureAwait(false);
            if (previousAccount != null && workedTime <= default(double))
            {
                for (var i = previousAccount.WorkDate.AddDays(1); i < currentDate; i = i.AddDays(1))
                {
                    if (DateService.IsWorking(i.DayOfWeek))
                        _timeAccountingContext.TimeAccounts.Add(
                            new TimeAccount
                            {
                                WorkDate = i,
                                IsWorking = true,
                                StartWorkTime = DateService.StartWorkTime(i.DayOfWeek),
                                EndWorkTime = DateService.StartWorkTime(i.DayOfWeek),
                                IsClosed = true,
                                IsStarted = true,
                                OverWork = - DateService.WorkingTime(i.DayOfWeek)
                            });
                }

                var lastAccount = await _timeAccountingContext.TimeAccounts
                    .Include(x => x.Breaks)
                    .FirstOrDefaultAsync(x=>x.WorkDate == previousAccount.WorkDate && !x.IsClosed)
                    .ConfigureAwait(false);

                if (lastAccount != null)
                {
                    if (lastAccount.IsStarted)
                    {
                        lastAccount.IsClosed = true;
                        lastAccount.EndWorkTime = new TimeSpan(23, 59, 59);

                        if (lastAccount.StartWorkTime > lastAccount.EndWorkTime) lastAccount.StartWorkTime = lastAccount.EndWorkTime;

                        var workTime = lastAccount.Breaks == null
                            ? (lastAccount.EndWorkTime - lastAccount.StartWorkTime).TotalMinutes
                            : (lastAccount.EndWorkTime - lastAccount.StartWorkTime).TotalMinutes
                              - lastAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                        lastAccount.OverWork = lastAccount.IsWorking
                            ? workTime - DateService.WorkingTime(lastAccount.WorkDate.DayOfWeek)
                            : workTime;
                        _timeAccountingContext.Entry(lastAccount).State = EntityState.Modified;
                    }
                    else _timeAccountingContext.TimeAccounts.Remove(lastAccount);
                }

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
                _accountFactory(currentAccounting, workedTime)
                );

            await Task.Delay(150);
            IsEnable = true;
        }

        private async Task HistoryExecute()
        {
            IsEnable = false;
            await Task.Delay(100);
            var history = _historyFactory();
            await Task.Delay(300);
            await history.UpdateHistory();
            await _navigationService.NavigateToAsync
                    (
                       history
                    );
            await Task.Delay(150);
            IsEnable = true;
        }
    }
}
