using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Cirrious.CrossCore.Converters;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public partial class OrgView : MvxViewController
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            /*var set = this.CreateBindingSet<OrgView, OrgViewModel>();

            set.Bind(NameLabel).To(p => p.Org.Info.Name);
            set.Bind(DescriptionLabel).To(p => p.Org.Description);
            set.Bind(OrgImageView).To(p => p.Org.Info.Logo).WithConversion(new LogoImageConverter(), null);

            set.Apply();*/

            
            var tableSource = new OrgEventTableSource(OrgEventsTableView);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm.Org)
                .WithConversion(new OrgTableSourceConverter(), null).Apply();

            OrgEventsTableView.Source = tableSource;
            OrgEventsTableView.ReloadData();
        }
    }

    public class OrgEventTableSource : MvxTableViewSource
    {
        public OrgEventTableSource(UITableView tableView)
            : base(tableView)
        {
            UseAnimations = true;
            AddAnimation = UITableViewRowAnimation.Top;
            RemoveAnimation = UITableViewRowAnimation.Middle;

            tableView.RegisterNibForCellReuse(OrgCell.Nib, OrgCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            if (item is Org)
            {
                return 240.0f;
            }

            if (item is OrgEventInfo)
            {
                return 50.0f;
            }

            throw new Exception("There is an unsupported type in the list.");
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {
            var key = default(NSString);

            if (item is Org)
            {
                key = OrgCell.Key;
            }

            if (item is OrgEventInfo)
            {
                key = OrgEventCell.Key;
            }

            return tableView.DequeueReusableCell(key, indexPath);
        }
    }

    public class LogoImageConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UIImage.FromFile((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class OrgTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var org = value as Org;
            if (org != null)
            {
                var result = new List<object>();

                result.Add(org);
                result.AddRange(org.EventInfos.ToArray());

                return result.ToArray();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}