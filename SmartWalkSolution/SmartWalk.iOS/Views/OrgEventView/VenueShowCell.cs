using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Converters;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Utils;
using System.Drawing;
using System.Windows.Input;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() => {
                var set = this.CreateBindingSet<VenueShowCell, VenueShow>();
                set.Bind(StartTimeLabel).To(vs => vs.Start).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(EndTimeLabel).To(vs => vs.End).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(DescriptionLabel).To(vs => vs.Description);
                set.Apply();
            });
        }

        public ICommand ExpandCollapseShowCommand { get; set; }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(bool isExpanded, VenueShow show)
        {
            if (isExpanded)
            {
                var textHeight = default(float);

                if (show.Description != null)
                {
                    var cellHeight = 8 + 8;
                    var cellWidth = 8 + 60 + 3 + 60 + 8;
                    var frameSize = new SizeF(ScreenUtil.CurrentScreenWidth - cellWidth, float.MaxValue); 
                    var textSize = new NSString(show.Description).StringSize(
                        UIFont.FromName("Helvetica", 15),
                        frameSize,
                        UILineBreakMode.TailTruncation);
                    textHeight = cellHeight + textSize.Height;
                }

                return textHeight;
            }

            return 35;
        }

        protected override bool Initialize()
        {
            var result = InitializeGestures();
            return result;
        }

        private bool InitializeGestures()
        {
            if (DescriptionLabel != null)
            {
                if (DescriptionLabel.GestureRecognizers == null ||
                    DescriptionLabel.GestureRecognizers.Length == 0)
                {
                    var tap = new UITapGestureRecognizer(() => {
                        if (ExpandCollapseShowCommand != null &&
                            ExpandCollapseShowCommand.CanExecute(DataContext))
                        {
                            ExpandCollapseShowCommand.Execute(DataContext);
                        }
                    });

                    tap.NumberOfTouchesRequired = (uint)1;
                    tap.NumberOfTapsRequired = (uint)1;

                    DescriptionLabel.AddGestureRecognizer(tap);
                }

                return true;
            }

            return false;
        }
    }
}