using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public partial class VenueView : ListViewBase
    {
        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenueShowsTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override NSLayoutConstraint GetProgressViewTopConstraint()
        {
            return ProgressViewTopConstraint;
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource)
                .To<VenueViewModel>(vm => vm.Venue)
                .WithConversion(new VenueTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                foreach (var cell in VenueShowsTableView.VisibleCells.OfType<VenueShowCell>())
                {
                    cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                }

                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
        }

        protected override void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
            if (ViewModel.ShowDirectionsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.NavigateInMaps);
            }

            if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.ShowContactInfo);
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyAddress);
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.NavigateInMaps:
                    if (ViewModel.ShowDirectionsCommand.CanExecute(ViewModel.Entity))
                    {
                        ViewModel.ShowDirectionsCommand.Execute(ViewModel.Entity);
                    }
                    break;

                case Localization.ShowContactInfo:
                    if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
                    {
                        ViewModel.ShowHideContactsCommand.Execute(ViewModel.Entity);
                    }
                    break;

                case Localization.CopyAddress:
                    if (ViewModel.CopyAddressCommand.CanExecute(null))
                    {
                        ViewModel.CopyAddressCommand.Execute(null);
                    }
                    break;

                case Localization.CopyLink:
                    if (ViewModel.CopyLinkCommand.CanExecute(null))
                    {
                        ViewModel.CopyLinkCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    if (ViewModel.ShareCommand.CanExecute(null))
                    {
                        ViewModel.ShareCommand.Execute(null);
                    }
                    break;
            }
        }
    }
}