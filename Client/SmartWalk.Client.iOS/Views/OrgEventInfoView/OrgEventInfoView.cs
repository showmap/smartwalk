using Cirrious.MvvmCross.Binding.BindingContext;
using EventKit;
using EventKitUI;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public partial class OrgEventInfoView : EntityViewBase
    {
        private EKEventEditViewController _editCalEventController;

        public new OrgEventInfoViewModel ViewModel
        {
            get { return (OrgEventInfoViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(OrgEventInfoTableView);  
        }

        protected override ProgressView GetProgressView()
        { 
            return ProgressView;
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new OrgEventInfoTableSource(OrgEventInfoTableView, ViewModel);

            this.CreateBinding(tableSource)
                .To<OrgEventInfoViewModel>(vm => vm.OrgEvent)
                .WithConversion(new OrgEventInfoTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventInfoTableView.UpdateLayout();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.CurrentCalendarEvent))
            {
                if (ViewModel.CurrentCalendarEvent != null)
                {
                    if (_editCalEventController == null)
                    {
                        InitializeCalEventViewController();
                    }
                }
                else
                {
                    DisposeCalEventViewController();
                }
            }
        }

        protected override void OnInitializingActionSheet(System.Collections.Generic.List<string> titles)
        {
            if (ViewModel.CreateEventCommand.CanExecute(null))
            {
                titles.Add(Localization.AddToCalendar);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.AddToCalendar:
                    if (ViewModel.CreateEventCommand.CanExecute(null))
                    {
                        ViewModel.CreateEventCommand.Execute(null);
                    }
                    break;
            }
        }

        private void InitializeCalEventViewController()
        {
            _editCalEventController = new OrgEventEditViewController();
            _editCalEventController.EventStore = (EKEventStore)ViewModel.CurrentCalendarEvent.EventStore;
            _editCalEventController.Event = (EKEvent)ViewModel.CurrentCalendarEvent.EventObj;

            var viewDelegate = new OrgEventCalEditViewDelegate(ViewModel);
            _editCalEventController.EditViewDelegate = viewDelegate;

            PresentViewController(_editCalEventController, true, null);
        }

        private void DisposeCalEventViewController()
        {
            if (_editCalEventController != null)
            {
                _editCalEventController.DismissViewController(true, null);
                _editCalEventController.EditViewDelegate = null;
                _editCalEventController.Dispose();
                _editCalEventController = null;
            }
        }
    }
}