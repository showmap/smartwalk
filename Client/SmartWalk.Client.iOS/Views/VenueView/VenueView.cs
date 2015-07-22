using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.OrgEventView;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public partial class VenueView : EntityViewBase
    {
        private ScrollToHideUIManager _scrollToHideManager;

        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(VenueShowsTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override IListViewSource CreateListViewSource()
        {
            _scrollToHideManager = new ScrollToHideUIManager(VenueShowsTableView);

            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel, _scrollToHideManager);

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
                VenueShowsTableView.UpdateLayout();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                VenueShowsTableView.ExpandShowCell(ViewModel.ExpandedShow);
                VenueShowsTableView.UpdateLayout();
            }
        }

        protected override void OnInitializingActionSheet(List<string> titles)
        {
            if (ViewModel.ShowDirectionsCommand.CanExecute(null))
            {
                titles.Add(Localization.NavigateInMaps);
            }

            if (ViewModel.ShowContactsCommand.CanExecute(null))
            {
                titles.Add(Localization.ContactInfo);
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                titles.Add(Localization.CopyAddress);
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                titles.Add(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                titles.Add(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.NavigateInMaps:
                    if (ViewModel.ShowDirectionsCommand.CanExecute(null))
                    {
                        ViewModel.ShowDirectionsCommand.Execute(null);
                    }
                    break;

                case Localization.ContactInfo:
                    if (ViewModel.ShowContactsCommand.CanExecute(null))
                    {
                        ViewModel.ShowContactsCommand.Execute(null);
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

        protected override void OnViewModelRefreshed(bool hasData, bool pullToRefresh)
        {
            // forcing to scroll page to top when next venue is requested
            base.OnViewModelRefreshed(hasData, true);

            if (pullToRefresh)
            {
                _scrollToHideManager.Reset();
            }
        }
    }
}