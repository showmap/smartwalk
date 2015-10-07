using System.Windows.Input;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IModalProviderViewModel
    {
        ModalViewContext ModalViewContext { get; }
        ICommand ShowHideModalViewCommand { get; }
    }

    public class ModalViewContext 
    {
        public ModalViewContext(ModalViewKind kind, object dataContext)
        {
            ViewKind = kind;
            DataContext = dataContext;
        }

        public ModalViewKind ViewKind { get; private set; }
        public object DataContext { get; private set; }

        public override bool Equals(object obj)
        {
            var context = obj as ModalViewContext;
            if (context != null)
            {
                return Equals(DataContext, context.DataContext) &&
                    ViewKind == context.ViewKind;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(ViewKind)
                .CombineHashCodeOrDefault(DataContext);
        }
    }

    public enum ModalViewKind
    {
        FullscreenImage = 1,
        Browser = 2
    }
}