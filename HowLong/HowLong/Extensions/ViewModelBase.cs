using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace HowLong.Extensions
{
    public class ViewModelBase : ReactiveObject
    {
        [Reactive]
        public bool IsEnable { get; set; } = true;
        [Reactive]
        public bool ShouldInit { get; set; }
        [Reactive]
        public bool Initialize { get; set; }
        public ReactiveCommand<Unit, Unit> InitializationCommand { get; set; }
    }
}
