using HowLong.Data;
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
using HowLong.DependencyServices;
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
        [Reactive]
        public TimeAccount CurrentAccounting { get; set; }
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
            INavigationService navigationService,
            HistoryViewModel historyViewModel,
            MainPage mainPage,
            TimeAccountingContext timeAccountingContext
        )
        {
            _timeAccountingContext = timeAccountingContext;
            _mainPage = mainPage;
            _historyViewModel = historyViewModel;
            _navigationService = navigationService;
            _workedTime = workedTime;
            CurrentAccounting = currentAccounting;
            _isTimerStarted = !currentAccounting.IsClosed;
            IsStarted = currentAccounting.IsStarted;
            StartWorkTime = currentAccounting.StartWorkTime;
            if (!CurrentAccounting.IsClosed) Breaks = currentAccounting.Breaks;
            else
                Breaks = new ObservableCollection<Break>(
                    currentAccounting.Breaks.OrderBy(x=>x.StartBreakTime)
                        .Select(@break => new Break
                        {
                            EndBreakTime = @break.EndBreakTime,
                            StartBreakTime = @break.StartBreakTime,
                            TimeAccount = @break.TimeAccount
                        }));

            EndWorkTime = currentAccounting.EndWorkTime;
            WorkDate = currentAccounting.WorkDate;

            if (!CurrentAccounting.IsClosed) Initialize();

            StartWorkCommand = ReactiveCommand.Create(() => StartWorkTime = DateTime.Now.TimeOfDay);
            EndWorkCommand = ReactiveCommand.Create(() => EndWorkTime = DateTime.Now.TimeOfDay);
            AddBreakCommand = ReactiveCommand.CreateFromTask(AddBreakExecute);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveExecute);
            AllCommand = ReactiveCommand.CreateFromTask(AllExecute);
            CurrentCommand = ReactiveCommand.CreateFromTask(CurrentExecute);
            DeleteBreakCommand = ReactiveCommand.CreateFromTask<Break, Unit>(async _ =>
             {
                 await DeleteBreakExecute(_);
                 return Unit.Default;
             });

            this.WhenAnyValue(x => x.StartWorkTime)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Select(x => x)
                .Subscribe(async _ => await UpdateStartWorkExecute());

            this.WhenAnyValue(x => x.EndWorkTime)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Select(x => x)
                .Subscribe(async _ => await UpdateEndWorkExecute());

            this.WhenAnyValue(x => x.SelectedBreak)
                .Skip(1)
                .Where(x => x != null)
                .Select(x => x)
                .InvokeCommand(DeleteBreakCommand);
        }

        private static async Task CurrentExecute() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("RemainedCurrentTitle"),
            TranslationCodeExtension.GetTranslation("RemainedCurrentText"),
            TranslationCodeExtension.GetTranslation("OkText"));

        private static async Task AllExecute() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("RemainedAllTitle"),
            TranslationCodeExtension.GetTranslation("RemainedAllText"),
            TranslationCodeExtension.GetTranslation("OkText"));

        private async Task AddBreakExecute()
        {
            var newBreak = new Break
            {
                StartBreakTime = DateTime.Now.TimeOfDay.TotalMinutes,
                EndBreakTime = DateTime.Now.TimeOfDay.TotalMinutes,
                TimeAccount = CurrentAccounting
            };
            if (!CurrentAccounting.IsClosed)
            {
                _timeAccountingContext.Breaks.Add(newBreak);
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
            else
                Breaks.Add(newBreak);
        }

        private async Task DeleteBreakExecute(Break @break)
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
                if (!CurrentAccounting.IsClosed)
                {
                    _timeAccountingContext.Breaks.Remove(@break);
                    await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                Breaks.Remove(@break);
            }
            IsEnable = true;
        }

        private async Task UpdateEndWorkExecute()
        {
            if (StartWorkTime > EndWorkTime) StartWorkTime = EndWorkTime;

            if (EndWorkTime == CurrentAccounting.EndWorkTime || CurrentAccounting.IsClosed) return;

            CurrentAccounting.EndWorkTime = EndWorkTime;
            _timeAccountingContext.Entry(CurrentAccounting).State = EntityState.Modified;
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);
        }

        private async Task UpdateStartWorkExecute()
        {
            if (DateTime.Today > WorkDate && !CurrentAccounting.IsClosed) return;

            if (StartWorkTime > EndWorkTime) EndWorkTime = StartWorkTime;

            if (StartWorkTime == CurrentAccounting.StartWorkTime || CurrentAccounting.IsClosed) return;

            if (!CurrentAccounting.IsStarted) IsStarted = true;

            CurrentAccounting.IsStarted = true;
            CurrentAccounting.StartWorkTime = StartWorkTime;
            _timeAccountingContext.Entry(CurrentAccounting).State = EntityState.Modified;
            await _timeAccountingContext.SaveChangesAsync()
                .ConfigureAwait(false);
        }
        private bool OnTimerTick()
        {
            if (DateTime.Today > WorkDate)
            {
                if (_isTimerStarted) CloseEndedDay();
                return _isTimerStarted = false;
            }

            if (!CurrentAccounting.IsStarted) return _isTimerStarted;
            var currentWorkTime = Breaks == null
                ? (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes + _workedTime
                : (DateTime.Now.TimeOfDay - StartWorkTime).TotalMinutes
                  - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) + _workedTime;
                
            CurrentOverWork = CurrentAccounting.IsWorking
                ? TimeSpan.FromMinutes(currentWorkTime - DateService.WorkingTime(DateTime.Now.DayOfWeek))
                : TimeSpan.FromMinutes(currentWorkTime);
            var halfTime = TimeSpan.FromMinutes(currentWorkTime
                                                - DateService.WorkingTime(DateTime.Now.DayOfWeek) / 2);
                
            if (CurrentOverWork > default(TimeSpan)
                && CurrentOverWork <= TimeSpan.FromSeconds(1)
                && CurrentAccounting.IsWorking
                && _isTimerStarted)
                ShowWorkingEnd();
                

            if (halfTime > default(TimeSpan)
                && halfTime <= TimeSpan.FromSeconds(1)
                && CurrentAccounting.IsWorking
                && _isTimerStarted)
                ShowWorkingHalf();
                
            TotalOverWork = CurrentOverWork + _totalDbOverWork;
            return _isTimerStarted;
        }

        private async void CloseEndedDay()
        {
            if (WorkDate == DateTime.Today) return;
            IsEnable = false;
            await Task.Delay(100);
            
            if (!CurrentAccounting.IsStarted)
                _timeAccountingContext.TimeAccounts.Remove(CurrentAccounting);
            else
            {
                CurrentAccounting.IsClosed = true;
                CurrentAccounting.EndWorkTime = new TimeSpan(23, 59, 59);

                if (CurrentAccounting.StartWorkTime > CurrentAccounting.EndWorkTime) CurrentAccounting.StartWorkTime = CurrentAccounting.EndWorkTime;

                var workTime = Breaks == null
                                    ? (CurrentAccounting.EndWorkTime - CurrentAccounting.StartWorkTime).TotalMinutes
                                    : (CurrentAccounting.EndWorkTime - CurrentAccounting.StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                CurrentAccounting.OverWork = CurrentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(CurrentAccounting.WorkDate.DayOfWeek)
                        : workTime;
                _timeAccountingContext.Entry(CurrentAccounting).State = EntityState.Modified;
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
            CurrentAccounting = currentAccounting;
            _workedTime = 0;
            WorkDate = currentAccounting.WorkDate;
            _isTimerStarted = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
            IsStarted = currentAccounting.IsStarted;
            StartWorkTime = currentAccounting.StartWorkTime;
            Breaks = currentAccounting.Breaks;

            EndWorkTime = currentAccounting.EndWorkTime;
            if (!CurrentAccounting.IsClosed) Initialize();
            _mainPage.UpdateWorkingDay();
        }

        private static async void ShowWorkingHalf() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("HalfWorkTitle"),
            TranslationCodeExtension.GetTranslation("HalfWorkText"),
            TranslationCodeExtension.GetTranslation("YesHalfWorkText"));

        private static async void ShowWorkingEnd() => await Application.Current.MainPage.DisplayAlert(
            TranslationCodeExtension.GetTranslation("DayIsOverTitle"),
            TranslationCodeExtension.GetTranslation("DayIsOverText"),
            TranslationCodeExtension.GetTranslation("YesDayIsOverText"));

        private async Task SaveExecute()
        {
            IsEnable = false;
            await Task.Delay(100);

            var isClosed = CurrentAccounting.IsClosed;

            if (!isClosed)
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
                if (CurrentAccounting.IsWorking && CurrentAccounting.IsStarted)
                    DependencyService.Get<IShowNotify>()
                        .CancelAll();
                CurrentAccounting.EndWorkTime = DateTime.Now.TimeOfDay;
            }
            if (StartWorkTime > EndWorkTime) StartWorkTime = EndWorkTime;

            
            if (isClosed)
            {
                var workTime = Breaks == null
                                    ? (EndWorkTime - StartWorkTime).TotalMinutes
                                    : (EndWorkTime - StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                CurrentAccounting.OverWork = CurrentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(CurrentAccounting.WorkDate.DayOfWeek)
                        : workTime;

                CurrentAccounting.StartWorkTime = StartWorkTime;
                CurrentAccounting.EndWorkTime = EndWorkTime;
                _timeAccountingContext.Entry(CurrentAccounting).State = EntityState.Modified;
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);

                if (CurrentAccounting.Breaks != null) _timeAccountingContext.Breaks.RemoveRange(CurrentAccounting.Breaks);

                if (Breaks != null) _timeAccountingContext.Breaks.AddRange(Breaks);

                await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);

                _historyViewModel.UpdateElement(CurrentAccounting);
            }
            else
            {
                var workTime = Breaks == null
                                    ? (CurrentAccounting.EndWorkTime - CurrentAccounting.StartWorkTime).TotalMinutes
                                    : (CurrentAccounting.EndWorkTime - CurrentAccounting.StartWorkTime).TotalMinutes
                                    - Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime);
                CurrentAccounting.OverWork = CurrentAccounting.IsWorking
                        ? workTime - DateService.WorkingTime(CurrentAccounting.WorkDate.DayOfWeek)
                        : workTime;
                CurrentAccounting.IsClosed = true;
                _timeAccountingContext.Entry(CurrentAccounting).State = EntityState.Modified;
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }

            CrossToastPopUp.Current.ShowCustomToast(
                !isClosed
                    ? TranslationCodeExtension.GetTranslation("DaySuccessEndedText")
                    : TranslationCodeExtension.GetTranslation("DaySuccessUpdatedText"), "#00AB00", "#FFFFFF");

            await _navigationService.GoBackAsync();
            IsEnable = true;
        }

        private async void Initialize()
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
            
            CurrentOverWork = CurrentAccounting.IsWorking 
                            ? TimeSpan.FromMinutes(_workedTime - DateService.WorkingTime(DateTime.Now.DayOfWeek))
                            : TimeSpan.FromMinutes(_workedTime);   
          
            TotalOverWork = CurrentOverWork + _totalDbOverWork;
        }

        public virtual void Subscribe()
        {
            if (CurrentAccounting.IsClosed) return;
            DependencyService.Get<IShowNotify>()
                .CancelAll();
            _isTimerStarted = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }
        public virtual void Unsubscribe()
        {
            _isTimerStarted = false;
            if (CurrentAccounting.IsClosed || !CurrentAccounting.IsWorking || !CurrentAccounting.IsStarted ||
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
