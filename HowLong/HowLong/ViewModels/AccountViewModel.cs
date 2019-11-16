using HowLong.Data;
using HowLong.DependencyServices;
using HowLong.Extensions;
using HowLong.Models;
using HowLong.Navigation;
using HowLong.Services;
using HowLong.Views;
using Microsoft.EntityFrameworkCore;
using Plugin.Toast;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly TimeAccountingContext _timeAccountingContext;
        private TimeSpan _totalDbOverWork;
        private readonly MainPage _mainPage;
        private readonly HistoryViewModel _historyViewModel;
        private readonly INavigationService _navigationService;
        private double _workedTime;
        private TimeAccount _currentAccounting;
        [Reactive]
        public bool FromHistory { get; set; }
        private bool _isTimerStarted;
        [Reactive]
        public bool IsStarted { get; set; }
        [Reactive]
        public Break SelectedBreak { get; set; }
        [Reactive]
        public ObservableCollection<Break> Breaks { get; set; }
        [Reactive]
        public TimeSpan StartWorkTime { get; set; }
        [Reactive]
        public TimeSpan EndWorkTime { get; set; }
        [Reactive]
        public DateTime WorkDate { get; set; }
        [Reactive]
        public TimeSpan CurrentOverWork { get; set; }
        [Reactive]
        public TimeSpan TotalOverWork { get; set; }
        public ReactiveCommand<Unit, TimeSpan> StartWorkCommand { get; internal set; }
        public ReactiveCommand<Unit, TimeSpan> EndWorkCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> AddBreakCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; internal set; }
        public ReactiveCommand<Break, Unit> DeleteBreakCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> AllCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> CurrentCommand { get; internal set; }

        public AccountViewModel
        (
            TimeAccount currentAccounting,
            double workedTime,
            bool fromHistory,
            INavigationService navigationService,
            HistoryViewModel historyViewModel,
            MainPage mainPage,
            TimeAccountingContext timeAccountingContext
        )
        {
            FromHistory = fromHistory;
            _timeAccountingContext = timeAccountingContext;
            _mainPage = mainPage;
            _historyViewModel = historyViewModel;
            _navigationService = navigationService;
            _workedTime = workedTime;
            _currentAccounting = currentAccounting;
            _isTimerStarted = !currentAccounting.IsClosed;
            IsStarted = currentAccounting.IsStarted;
            StartWorkTime = currentAccounting.StartWorkTime;
            if (!_currentAccounting.IsClosed) Breaks = currentAccounting.Breaks;
            else
                Breaks = new ObservableCollection<Break>(
                    currentAccounting.Breaks.OrderBy(x => x.StartBreakTime)
                        .Select(@break => new Break
                        {
                            EndBreakTime = @break.EndBreakTime,
                            StartBreakTime = @break.StartBreakTime,
                            TimeAccount = @break.TimeAccount
                        }));

            EndWorkTime = currentAccounting.EndWorkTime;
            WorkDate = currentAccounting.WorkDate;

            if (!FromHistory) InitializeAsync();

            StartWorkCommand = ReactiveCommand.Create(() => StartWorkTime = DateTime.Now.TimeOfDay);
            EndWorkCommand = ReactiveCommand.Create(() => EndWorkTime = DateTime.Now.TimeOfDay);
            AddBreakCommand = ReactiveCommand.CreateFromTask(AddBreakExecuteAsync);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveExecuteAsync);
            AllCommand = ReactiveCommand.CreateFromTask(AllExecuteAsync);
            CurrentCommand = ReactiveCommand.CreateFromTask(CurrentExecuteAsync);
            DeleteBreakCommand = ReactiveCommand.CreateFromTask<Break, Unit>(async _ =>
             {
                 await DeleteBreakExecuteAsync(_);
                 return Unit.Default;
             });

            this.WhenAnyValue(x => x.StartWorkTime)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(async _ => await UpdateStartWorkExecuteAsync());

            this.WhenAnyValue(x => x.EndWorkTime)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(async _ => await UpdateEndWorkExecuteAsync());

            this.WhenAnyValue(x => x.SelectedBreak)
                .Skip(1)
                .Where(x => x != null)
                .InvokeCommand(DeleteBreakCommand);
        }

        private static async Task CurrentExecuteAsync() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("RemainedCurrentTitle"),
            TranslationCodeExtension.GetTranslation("RemainedCurrentText"),
            TranslationCodeExtension.GetTranslation("OkText"));

        private static async Task AllExecuteAsync() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("RemainedAllTitle"),
            TranslationCodeExtension.GetTranslation("RemainedAllText"),
            TranslationCodeExtension.GetTranslation("OkText"));

        private async Task AddBreakExecuteAsync()
        {
            var newBreak = new Break
            {
                StartBreakTime = DateTime.Now.TimeOfDay.TotalMinutes,
                EndBreakTime = DateTime.Now.TimeOfDay.TotalMinutes,
                TimeAccount = _currentAccounting
            };
            if (!_currentAccounting.IsClosed)
            {
                _timeAccountingContext.Breaks.Add(newBreak);
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
            else
                Breaks.Add(newBreak);
        }

        private async Task DeleteBreakExecuteAsync(Break @break)
        {
            IsEnable = false;
            await Task.Delay(100);
            var result = await Application.Current.MainPage.DisplayAlert(
                 TranslationCodeExtension.GetTranslation("DeleteBreakTitle"),
                  TranslationCodeExtension.GetTranslation("DeleteBreakText"),
                  TranslationCodeExtension.GetTranslation("YesDeleteBreakText"),
                   TranslationCodeExtension.GetTranslation("NoText"));
            if (result)
            {
                await Task.Delay(50);
                if (!_currentAccounting.IsClosed)
                {
                    _timeAccountingContext.Breaks.Remove(@break);
                    await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                Breaks.Remove(@break);
            }
            IsEnable = true;
        }

        private async Task UpdateEndWorkExecuteAsync()
        {
            if (StartWorkTime > EndWorkTime) StartWorkTime = EndWorkTime;

            if (EndWorkTime == _currentAccounting.EndWorkTime || _currentAccounting.IsClosed) return;

            _currentAccounting.EndWorkTime = EndWorkTime;
            _timeAccountingContext.Entry(_currentAccounting).State = EntityState.Modified;
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);
        }

        private async Task UpdateStartWorkExecuteAsync()
        {
            if (DateTime.Today > WorkDate && !FromHistory) return;

            if (StartWorkTime > EndWorkTime) EndWorkTime = StartWorkTime;

            if (StartWorkTime == _currentAccounting.StartWorkTime || _currentAccounting.IsClosed) return;

            if (!_currentAccounting.IsStarted) IsStarted = true;

            _currentAccounting.IsStarted = true;
            _currentAccounting.StartWorkTime = StartWorkTime;
            _timeAccountingContext.Entry(_currentAccounting).State = EntityState.Modified;
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);
        }
        private bool OnTimerTick()
        {
            if (DateTime.Today > WorkDate)
            {
                if (_isTimerStarted) CloseEndedDayAsync();
                return _isTimerStarted = false;
            }

            if (!_currentAccounting.IsStarted) return _isTimerStarted;
            var currentWorkTime = Breaks == null
                ? (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes + _workedTime
                : (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes
                  - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) + _workedTime;

            CurrentOverWork = _currentAccounting.IsWorking
                ? TimeSpan.FromMinutes(currentWorkTime - DateService.WorkingTime(DateTime.Now.DayOfWeek))
                : TimeSpan.FromMinutes(currentWorkTime);
            var halfTime = TimeSpan.FromMinutes(currentWorkTime
                                                - DateService.WorkingTime(DateTime.Now.DayOfWeek) / 2);

            if (CurrentOverWork > default(TimeSpan)
                && CurrentOverWork <= TimeSpan.FromSeconds(1)
                && _currentAccounting.IsWorking
                && _isTimerStarted)
                ShowWorkingEndAsync();


            if (halfTime > default(TimeSpan)
                && halfTime <= TimeSpan.FromSeconds(1)
                && _currentAccounting.IsWorking
                && _isTimerStarted)
                ShowWorkingHalfAsync();

            TotalOverWork = CurrentOverWork + _totalDbOverWork;
            return _isTimerStarted;
        }

        private async void CloseEndedDayAsync()
        {
            if (WorkDate == DateTime.Today) return;
            IsEnable = false;
            await Task.Delay(100);

            if (!_currentAccounting.IsStarted)
                _timeAccountingContext.TimeAccounts.Remove(_currentAccounting);
            else
            {
                _currentAccounting.IsClosed = true;
                _currentAccounting.EndWorkTime = new TimeSpan(23, 59, 59);

                if (_currentAccounting.StartWorkTime > _currentAccounting.EndWorkTime) _currentAccounting.StartWorkTime = _currentAccounting.EndWorkTime;

                var workTime = Breaks == null
                                    ? (_currentAccounting.EndWorkTime - _currentAccounting.StartWorkTime).TotalMinutes
                                    : (_currentAccounting.EndWorkTime - _currentAccounting.StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                _currentAccounting.OverWork = _currentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(_currentAccounting.WorkDate.DayOfWeek)
                        : workTime;
                _timeAccountingContext.Entry(_currentAccounting).State = EntityState.Modified;
                DependencyService.Get<IShowNotify>()
                    .CancelAll();
            }
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);

            var isWorkDay = DateService.IsWorking(DateTime.Today.DayOfWeek);
            var currentAccounting = new TimeAccount
            {
                WorkDate = DateTime.Today,
                IsWorking = isWorkDay,
                IsStarted = IsStarted,
                StartWorkTime = default,
                EndWorkTime = default
            };
            _timeAccountingContext.TimeAccounts.Add(currentAccounting);
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);
            UpdateAccount(currentAccounting);

            await Task.Delay(100);
            IsEnable = true;
        }

        private void UpdateAccount(TimeAccount currentAccounting)
        {
            _currentAccounting = currentAccounting;
            _workedTime = 0;
            WorkDate = currentAccounting.WorkDate;
            _isTimerStarted = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
            IsStarted = currentAccounting.IsStarted;
            StartWorkTime = currentAccounting.StartWorkTime;
            Breaks = currentAccounting.Breaks;

            EndWorkTime = currentAccounting.EndWorkTime;
            if (!FromHistory) InitializeAsync();
            _mainPage.UpdateWorkingDay();
        }

        private static async void ShowWorkingHalfAsync() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("HalfWorkTitle"),
            TranslationCodeExtension.GetTranslation("HalfWorkText"),
            TranslationCodeExtension.GetTranslation("YesHalfWorkText"));

        private static async void ShowWorkingEndAsync() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("DayIsOverTitle"),
            TranslationCodeExtension.GetTranslation("DayIsOverText"),
            TranslationCodeExtension.GetTranslation("YesDayIsOverText"));

        private async Task SaveExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);

            if (!FromHistory)
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                TranslationCodeExtension.GetTranslation("EndDayTitle"),
                 TranslationCodeExtension.GetTranslation("EndDayText"),
                 TranslationCodeExtension.GetTranslation("YesEndDayText"),
                  TranslationCodeExtension.GetTranslation("NoText"));
                if (!result)
                {
                    IsEnable = true;
                    return;
                }
                if (_currentAccounting.IsWorking && _currentAccounting.IsStarted)
                    DependencyService.Get<IShowNotify>()
                        .CancelAll();
                _currentAccounting.EndWorkTime = DateTime.Now.TimeOfDay;
            }
            if (StartWorkTime > EndWorkTime) StartWorkTime = EndWorkTime;


            if (_currentAccounting.IsClosed)
            {
                var workTime = Breaks == null
                                    ? (EndWorkTime - StartWorkTime).TotalMinutes
                                    : (EndWorkTime - StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                _currentAccounting.OverWork = _currentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(_currentAccounting.WorkDate.DayOfWeek)
                        : workTime;

                _currentAccounting.StartWorkTime = StartWorkTime;
                _currentAccounting.EndWorkTime = EndWorkTime;
                _timeAccountingContext.Entry(_currentAccounting).State = EntityState.Modified;
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);

                if (_currentAccounting.Breaks != null) _timeAccountingContext.Breaks.RemoveRange(_currentAccounting.Breaks);

                if (Breaks != null) _timeAccountingContext.Breaks.AddRange(Breaks);

                await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);

                _historyViewModel.UpdateElement(_currentAccounting);
            }
            else
            {
                var workTime = Breaks == null
                                    ? (_currentAccounting.EndWorkTime - _currentAccounting.StartWorkTime).TotalMinutes
                                    : (_currentAccounting.EndWorkTime - _currentAccounting.StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                _currentAccounting.OverWork = _currentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(_currentAccounting.WorkDate.DayOfWeek)
                        : workTime;
                _currentAccounting.IsClosed = true;
                _timeAccountingContext.Entry(_currentAccounting).State = EntityState.Modified;
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
                if (FromHistory) await _historyViewModel.UpdateHistoryAsync();
            }

            CrossToastPopUp.Current.ShowCustomToast(
                !FromHistory
                    ? TranslationCodeExtension.GetTranslation("DaySuccessEndedText")
                    : TranslationCodeExtension.GetTranslation("DaySuccessUpdatedText"), "#00AB00", "#FFFFFF");

            await _navigationService.GoBackAsync();
            IsEnable = true;
        }

        private async void InitializeAsync()
        {
            _totalDbOverWork = TimeSpan.FromMinutes(await _timeAccountingContext.TimeAccounts.Where(x => x.WorkDate < WorkDate && x.IsClosed)
                .Include(x => x.Breaks)
                .GroupBy(x => x.WorkDate)
                .Select(x =>
                    x.Select(y => new HistoryAccount
                    {
                        DayOverWork = x.Sum(v => v.Breaks == null
                                          ? (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                          : (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                            - v.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                                      - (y.Breaks == null
                                          ? (y.EndWorkTime - y.StartWorkTime).TotalMinutes - y.OverWork
                                          : (y.EndWorkTime - y.StartWorkTime).TotalMinutes
                                            - y.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) - y.OverWork)
                    }))
                .SumAsync(g => g.First().DayOverWork)
                .ConfigureAwait(false));

            CurrentOverWork = _currentAccounting.IsWorking
                            ? TimeSpan.FromMinutes(_workedTime - DateService.WorkingTime(DateTime.Now.DayOfWeek))
                            : TimeSpan.FromMinutes(_workedTime);

            TotalOverWork = CurrentOverWork + _totalDbOverWork;
        }

        public void Subscribe()
        {
            if (FromHistory) return;
            DependencyService.Get<IShowNotify>()
                .CancelAll();
            _isTimerStarted = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }
        public void Unsubscribe()
        {
            _isTimerStarted = false;
            if (FromHistory || !_currentAccounting.IsWorking || !_currentAccounting.IsStarted ||
                DateTime.Today != WorkDate) return;
            var currentWorkTime = Breaks == null
                ? (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes + _workedTime
                : (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes + _workedTime
                  - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);

            var halfTime = (DateService.WorkingTime(DateTime.Now.DayOfWeek) / 2)
                           - currentWorkTime;
            currentWorkTime = DateService.WorkingTime(DateTime.Now.DayOfWeek)
                              - currentWorkTime;
            if (halfTime > 0) DependencyService.Get<IShowNotify>()
                .SetHalf(halfTime);

            if (currentWorkTime > 0) DependencyService.Get<IShowNotify>()
                .SetEnd(currentWorkTime);
        }
    }
}
