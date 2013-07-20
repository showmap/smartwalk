using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class OrgEventView : TableViewBase
    {
        private UIBarButtonItem _modeButton;
        private OrgEvent _currenmMapViewtOrgEvent;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return VenuesAndShowsTableView; } }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            VenuesMapView.Delegate = new OrgEventMapDelegate(ViewModel);

            InitializeToolBar();

            UpdateViewState();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            // to fix the bug: http://stackoverflow.com/questions/14307037/bug-in-uitableview-layout-after-orientation-change
            TableView.BeginUpdates();
            TableView.EndUpdates();
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.Info != null)
            {
                NavigationItem.Title = ViewModel.OrgEvent.Info.Date.ToShortDateString();
            }
        }

        protected override MvxTableViewSource CreateTableViewSource()
        {
            var tableSource = new OrgEventTableSource(VenuesAndShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgEventViewModel vm) => vm.OrgEvent.Venues).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
            {
                InitializeMapView();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewState();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
            }
        }

        private void InitializeToolBar()
        {
            _modeButton = new UIBarButtonItem();

            _modeButton.Clicked += (sender, e) => 
                {
                    if (ViewModel.SwitchModeCommand.CanExecute(null))
                    {
                        ViewModel.SwitchModeCommand.Execute(null);
                    }
                };

            NavigationItem.SetRightBarButtonItem(_modeButton, true);
        }

        private void InitializeMapView()
        {
            if (ViewModel.OrgEvent != null &&
                ViewModel.OrgEvent.Venues != null)
            {
                if (_currenmMapViewtOrgEvent != ViewModel.OrgEvent)
                {
                    var annotations = ViewModel.OrgEvent.Venues
                    .SelectMany(v => v.Info.Addresses
                        .Select(a => new VenueAnnotation(v, a))).ToArray();
                    var coordinates = annotations
                    .Select(va => va.Coordinate)
                    .Where(c => c.Latitude != 0 && c.Longitude != 0).ToArray();

                    VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), false);
                    VenuesMapView.AddAnnotations(annotations);

                    _currenmMapViewtOrgEvent = ViewModel.OrgEvent;
                }
            }
            else
            {
                VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);
            }
        }

        private void SelectVenueMapAnnotation(Venue venue)
        {
            if (venue != null)
            {
                var annotation = VenuesMapView.Annotations
                .OfType<VenueAnnotation>()
                    .FirstOrDefault(an => an.Venue == venue);

                if (annotation != null)
                {
                    VenuesMapView.SelectAnnotation(annotation, true);
                }
            }
            else if (VenuesMapView.SelectedAnnotations.Any())
            {
                foreach (var annotation in VenuesMapView.SelectedAnnotations)
                {
                    VenuesMapView.DeselectAnnotation(annotation, false);
                }
            }
        }
       
        private void UpdateViewState()
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                InitializeMapView();
                TablePanel.Hidden = true;
                MapPanel.Hidden = false;
                _modeButton.Title = "List";
            }
            else
            {
                TablePanel.Hidden = false;
                MapPanel.Hidden = true;
                _modeButton.Title = "Map";
            }
        }
    }
}