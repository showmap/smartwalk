using EventKit;
using EventKitUI;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventCalEditViewDelegate : EKEventEditViewDelegate
    {
        private readonly OrgEventViewModel _viewModel;

        public OrgEventCalEditViewDelegate(OrgEventViewModel viewModel)
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