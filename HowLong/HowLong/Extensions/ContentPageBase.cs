using ReactiveUI;
using ReactiveUI.XamForms;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace HowLong.Extensions
{
	public class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : ViewModelBase
	{
		protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (!ViewModel.ShouldInit
				|| ViewModel.InitializationCommand == null) return;
			this.WhenAnyValue(x => x.ViewModel.Initialize)
				.Where(x => x && ViewModel.InitializationCommand!=null)
				.Select(x => Unit.Default)
				.InvokeCommand(ViewModel.InitializationCommand);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			SubscriptionDisposables.Clear();
		}
	}
}