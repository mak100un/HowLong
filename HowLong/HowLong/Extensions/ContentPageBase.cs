using ReactiveUI.XamForms;
using System.Reactive.Disposables;

namespace HowLong.Extensions
{
	public class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : class
	{
		protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			SubscriptionDisposables.Clear();
		}
	}
}