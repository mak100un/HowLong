using HowLong.Containers;
using HowLong.Data;
using HowLong.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Xamarin.Forms.Xaml;

namespace HowLong.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BreakCard
    {
        private readonly TimeAccountingContext _timeAccountingContext;
        public BreakCard()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Do(PopulateFromViewModel)
                .Subscribe();
            _timeAccountingContext = CompositionRoot.Resolve<TimeAccountingContext>();
        }

        private void PopulateFromViewModel(Break vm)
        {
            StartDinnerTmPck.Time = TimeSpan.FromMinutes(vm.StartBreakTime);
            EndDinnerTmPck.Time = TimeSpan.FromMinutes(vm.EndBreakTime);

            Observable.FromEventPattern<EventHandler, EventArgs>(
                h => EndDinnerBtn.Clicked += h,
                h => EndDinnerBtn.Clicked -= h)
                 .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                 .Subscribe(_ =>
                 {
                     EndDinnerTmPck.Time = DateTime.Now.TimeOfDay;
                     if (StartDinnerTmPck.Time > EndDinnerTmPck.Time) StartDinnerTmPck.Time = EndDinnerTmPck.Time;
                 });

            Observable.FromEventPattern<EventHandler, EventArgs>(
                h => StartDinnerBtn.Clicked += h,
                h => StartDinnerBtn.Clicked -= h)
                 .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                 .Subscribe(_ =>
                 {
                     StartDinnerTmPck.Time = DateTime.Now.TimeOfDay;
                     if (StartDinnerTmPck.Time > EndDinnerTmPck.Time) EndDinnerTmPck.Time = StartDinnerTmPck.Time;
                 });

            this.WhenAnyValue(x => x.StartDinnerTmPck.Time)
                .Skip(1)
                .Where(x => Math.Abs(x.TotalMinutes - ViewModel.StartBreakTime) > 0.017)
                .Subscribe(async _ =>
                {
                    if (StartDinnerTmPck.Time > EndDinnerTmPck.Time) EndDinnerTmPck.Time = StartDinnerTmPck.Time;

                    ViewModel.StartBreakTime = StartDinnerTmPck.Time.TotalMinutes;

                    if (ViewModel.TimeAccount.IsClosed) return;
                    _timeAccountingContext.Entry(ViewModel).State = EntityState.Modified;
                    await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);
                });

            this.WhenAnyValue(x => x.EndDinnerTmPck.Time)
                .Skip(1)
                .Where(x => Math.Abs(x.TotalMinutes - ViewModel.EndBreakTime) > 0.017)
                .Subscribe(async _ =>
                {
                    if (StartDinnerTmPck.Time > EndDinnerTmPck.Time) StartDinnerTmPck.Time = EndDinnerTmPck.Time;

                    ViewModel.EndBreakTime = EndDinnerTmPck.Time.TotalMinutes;

                    if (ViewModel.TimeAccount.IsClosed) return;
                    _timeAccountingContext.Entry(ViewModel).State = EntityState.Modified;
                    await _timeAccountingContext.SaveChangesAsync()
                        .ConfigureAwait(false);
                });
        }
    }
}