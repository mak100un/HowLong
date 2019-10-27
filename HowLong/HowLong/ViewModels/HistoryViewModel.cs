using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
using HowLong.Data;
using HowLong.Extensions;
using HowLong.Models;
using HowLong.Navigation;
using Microsoft.EntityFrameworkCore;
using Plugin.Toast;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private readonly Func<TimeAccount, double, AccountViewModel> _accountFactory;
        private readonly TimeAccountingContext _timeAccountingContext;
        private readonly INavigationService _navigationService;

        [Reactive]
        public bool IsRefreshing { get; set; }
        [Reactive]
        public HistoryAccount SelectedAccount { get; set; }
        [Reactive]
        public ObservableCollectionExtended<Grouping<(int, int), HistoryAccount>> AllAccounts { get; set; }
        [Reactive]
        public TimeSpan TotalOverWork { get; set; }
        [Reactive]
        public bool NoElements { get; set; }

        public ReactiveCommand<HistoryAccount, Unit> NavigateToAccountCommand { get; }
        public ICommand RefreshCommand =>
            new Command(async () =>
            {
                IsRefreshing = true;
                await UpdateHistory();
                IsRefreshing = false;
            });

        public HistoryViewModel
        (
            INavigationService navigationService,
            TimeAccountingContext timeAccountingContext,
            Func<TimeAccount, double, AccountViewModel> accountFactory
        )
        {
            _accountFactory = accountFactory;
            _timeAccountingContext = timeAccountingContext;
            _navigationService = navigationService;
            NavigateToAccountCommand = ReactiveCommand.CreateFromTask<HistoryAccount, Unit>(async _ =>
            {
                await NavigateToAccountExecute(_);
                return Unit.Default;
            });
            this.WhenAnyValue(x => x.SelectedAccount)
                .Skip(1)
                .Where(x => x != null)
                .Select(x => x)
                .InvokeCommand(NavigateToAccountCommand);

            this.WhenAnyValue(x => x.AllAccounts)
                .Skip(1)
                .Where(x => x != null)
                .Select(x => x)
                .Subscribe(UpdateTotalOverWork);
        }


        private void UpdateTotalOverWork(ObservableCollection<Grouping<(int, int), HistoryAccount>> accounts) =>
            TotalOverWork = TimeSpan.FromMinutes(accounts.SelectMany(x => x)
                .GroupBy(x => x.TimeAccount.WorkDate)
                .Sum(g => g.First().DayOverWork));

        private async Task NavigateToAccountExecute(HistoryAccount arg)
        {
            IsEnable = false;
            await Task.Delay(100);
            var action = await Application.Current.MainPage.DisplayActionSheet(
                TranslationCodeExtension.GetTranslation("SelectAction"),
                TranslationCodeExtension.GetTranslation("CancelAction"),
                null,
                TranslationCodeExtension.GetTranslation("EditAction"),
                TranslationCodeExtension.GetTranslation("DeleteAction"));
            if (action == TranslationCodeExtension.GetTranslation("EditAction")) await _navigationService.NavigateToAsync
                    (
                _accountFactory(arg.TimeAccount, 0)
                    );
            if (action == TranslationCodeExtension.GetTranslation("DeleteAction"))
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                    TranslationCodeExtension.GetTranslation("DeleteDayTitle"),
                       TranslationCodeExtension.GetTranslation("DeleteDayText"),
                       TranslationCodeExtension.GetTranslation("YesDeleteDayText"),
                       TranslationCodeExtension.GetTranslation("NoText"));
                if (!result)
                {
                    IsEnable = true;
                    return;
                }
                DeleteElement(arg);
                _timeAccountingContext.Breaks.RemoveRange(arg.TimeAccount.Breaks);
                _timeAccountingContext.TimeAccounts.Remove(arg.TimeAccount);
                await _timeAccountingContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
            await Task.Delay(50);
            IsEnable = true;
        }

        private void DeleteElement(HistoryAccount arg)
        {
            var (indexOfGroup, indexOfElement) = SearchIndex(arg.TimeAccount);
            if (AllAccounts[indexOfGroup].Count == 1) AllAccounts.RemoveAt(indexOfGroup);
            else
            {
                AllAccounts[indexOfGroup].RemoveAt(indexOfElement);
                var todayAccounts = AllAccounts.SelectMany(x => x)
                    .Where(x => x.TimeAccount.WorkDate == arg.TimeAccount.WorkDate)
                    .GroupBy(x => x.TimeAccount.WorkDate)
                    .Select(x =>
                        x.Select(y => new HistoryAccount
                        {
                            TimeAccount = y.TimeAccount,
                            DayOverWork = x.Sum(v => v.TimeAccount.Breaks == null
                                                ? (v.TimeAccount.EndWorkTime - v.TimeAccount.StartWorkTime).TotalMinutes
                                                : (v.TimeAccount.EndWorkTime - v.TimeAccount.StartWorkTime).TotalMinutes
                                                - v.TimeAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                                       - (y.TimeAccount.Breaks == null
                                            ? (y.TimeAccount.EndWorkTime - y.TimeAccount.StartWorkTime).TotalMinutes - y.TimeAccount.OverWork
                                            : (y.TimeAccount.EndWorkTime - y.TimeAccount.StartWorkTime).TotalMinutes
                                            - y.TimeAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) - y.TimeAccount.OverWork)
                        }))
                    .SelectMany(x => x)
                    .OrderByDescending(x => x.TimeAccount.WorkDate)
                    .ToList();
                foreach (var todayAcc in todayAccounts)
                {
                    var indexes = SearchIndex(todayAcc.TimeAccount);
                    AllAccounts[indexes.indexOfGroup][indexes.indexOfElement] = todayAcc;
                }
            }
            NoElements = AllAccounts.Count == 0;
            CrossToastPopUp.Current.ShowCustomToast(TranslationCodeExtension.GetTranslation("DaySuccessDeleteText"), "#00AB00", "#FFFFFF");
            UpdateTotalOverWork(AllAccounts);
        }

        public void UpdateElement(TimeAccount currentAccounting)
        {
            var (indexOfGroup, indexOfElement) = SearchIndex(currentAccounting);
            AllAccounts[indexOfGroup][indexOfElement].TimeAccount = currentAccounting;
            var todayAccounts = AllAccounts.SelectMany(x => x)
                .Where(x => x.TimeAccount.WorkDate == currentAccounting.WorkDate)
                .GroupBy(x => x.TimeAccount.WorkDate)
                .Select(x =>
                        x.Select(y => new HistoryAccount
                        {
                            TimeAccount = y.TimeAccount,
                            DayOverWork = x.Sum(v => v.TimeAccount.Breaks == null
                                                ? (v.TimeAccount.EndWorkTime - v.TimeAccount.StartWorkTime).TotalMinutes
                                                : (v.TimeAccount.EndWorkTime - v.TimeAccount.StartWorkTime).TotalMinutes
                                                - v.TimeAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                                       - (y.TimeAccount.Breaks == null
                                            ? (y.TimeAccount.EndWorkTime - y.TimeAccount.StartWorkTime).TotalMinutes - y.TimeAccount.OverWork
                                            : (y.TimeAccount.EndWorkTime - y.TimeAccount.StartWorkTime).TotalMinutes
                                            - y.TimeAccount.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) - y.TimeAccount.OverWork)
                        }))
                .SelectMany(x => x)
                .OrderByDescending(x => x.TimeAccount.WorkDate)
                .ToList();
            foreach (var todayAcc in todayAccounts)
            {
                var indexes = SearchIndex(todayAcc.TimeAccount);
                AllAccounts[indexes.indexOfGroup][indexes.indexOfElement] = todayAcc;
            }
            UpdateTotalOverWork(AllAccounts);
        }
        public async Task UpdateHistory()
        {
            var allAccounts = await _timeAccountingContext.TimeAccounts.Where(x => x.IsClosed)
                .Include(x => x.Breaks)
                .ToArrayAsync()
                .ConfigureAwait(false);
            var allHistory = allAccounts.GroupBy(x => x.WorkDate)
                .Select(x =>
                    x.Select(y => new HistoryAccount
                    {
                        TimeAccount = y,
                        DayOverWork = x.Sum(v => v.Breaks == null
                                            ? (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                            : (v.EndWorkTime - v.StartWorkTime).TotalMinutes
                                            - v.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime))
                                    - (y.Breaks == null
                                        ? (y.EndWorkTime - y.StartWorkTime).TotalMinutes - y.OverWork
                                        : (y.EndWorkTime - y.StartWorkTime).TotalMinutes
                                        - y.Breaks.Sum(d => d.EndBreakTime - d.StartBreakTime) - y.OverWork)
                    }))
                .SelectMany(x => x)
                .OrderByDescending(x => x.TimeAccount.WorkDate)
                .ToList();
            var groups = allHistory.GroupBy(p => (p.TimeAccount.WorkDate.Month, p.TimeAccount.WorkDate.Year)).Select(g => new Grouping<(int, int), HistoryAccount>(g.Key, g));
            AllAccounts = new ObservableCollectionExtended<Grouping<(int, int), HistoryAccount>>(groups);
            NoElements = AllAccounts.Count == 0;
        }

        public (int indexOfGroup, int indexOfElement) SearchIndex(TimeAccount currentAccounting)
        {
            for (var i = 0; i < AllAccounts.Count; i++)
                if (AllAccounts[i].Key == (currentAccounting.WorkDate.Month, currentAccounting.WorkDate.Year))
                    for (var y = 0; y < AllAccounts[i].Count; y++)
                        if (AllAccounts[i][y].TimeAccount.AccountId == currentAccounting.AccountId) return (i, y);
            return (-1, -1);
        }
    }
}
