using EventKit;
using EventKitUI;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public class OrgEventCalEditViewDelegate : EKEventEditViewDelegate
    {
        private readonly OrgEventInfoViewModel _viewModel;

        public OrgEventCalEditViewDelegate(OrgEventInfoViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void Completed(EKEventEditViewController controller, EKEventEditViewAction action)
        {
            switch (action)
            {
                case EKEventEditViewAction.Canceled:
                    if (_viewModel.CancelEventCommand.CanExecute(null))
                    {
                        _viewModel.CancelEventCommand.Execute(null);
                    }
                    break;

                case EKEventEditViewAction.Saved:
                    if (_viewModel.SaveEventCommand.CanExecute(null))
                    {
                        _viewModel.SaveEventCommand.Execute(null);
                    }
                    break;
            }
        }
    }
}