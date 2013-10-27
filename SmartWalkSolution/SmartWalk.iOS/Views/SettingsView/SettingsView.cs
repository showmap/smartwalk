using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.BindingContext;
using CrossUI.Touch.Dialog.Elements;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.SettingsView
{
    public partial class SettingsView : ActiveAwareDialogController
    {
        public new SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ButtonBarUtil.OverrideNavigatorBackButton(
                NavigationItem,
                () => NavigationController.PopViewControllerAnimated(true));

            var target = this.CreateInlineBindingTarget<SettingsViewModel>();

            // TODO: To Localize
            Root = new RootElement("Settings".ToUpper())
                {
                    new Section("Privacy")
                        {
                            new BooleanElement("Send Usage Stats", true)
                                .Bind(
                                    target,
                                    vm => vm.IsAnonymousStatsEnabled,
                                    (string)null,
                                    null,
                                    null,
                                    MvxBindingMode.TwoWay),
                        },
                };

            // TODO: Add About Privacy section
        }
    }
}