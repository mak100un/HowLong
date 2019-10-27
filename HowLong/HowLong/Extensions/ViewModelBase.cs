using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace HowLong.Extensions
{
    public class ViewModelBase : ReactiveObject
    {
        [Reactive]
        public bool IsEnable { get; set; } = true;
    }
}
