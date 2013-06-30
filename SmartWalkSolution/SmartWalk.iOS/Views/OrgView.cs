using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Cells;
using Cirrious.CrossCore.Converters;

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

            var set = this.CreateBindingSet<OrgView, OrgViewModel>();

            set.Bind(NameLabel).To(p => p.Org.Info.Name);
            set.Bind(DescriptionLabel).To(p => p.Org.Description);
            set.Bind(OrgImageView).To(p => p.Org.Info.Logo).WithConversion(new LogoImageConverter(), null);

            set.Apply();

            
            var tableSource = new OrgEventTableSource(OrgEventsTableView);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm.Org.EventInfos).Apply();

            OrgEventsTableView.Source = tableSource;
            OrgEventsTableView.ReloadData();
        }
    }

    public class LogoImageConverter : IMvxValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return UIImage.FromFile((string)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
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

            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(OrgEventCell.Key, indexPath);
        }
    }
}