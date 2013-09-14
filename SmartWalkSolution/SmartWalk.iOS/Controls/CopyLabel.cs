using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace SmartWalk.iOS.Controls
{
    [Register("CopyLabel")]
    public class CopyLabel : UILabel
    {
        private UILongPressGestureRecognizer _longPressGestureRecognizer;
        private bool _isCopyingEnabled;

        public CopyLabel()
        {
            Initialize();
        }

        public CopyLabel(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public override bool CanBecomeFirstResponder
        {
            get { return IsCopyingEnabled; }
        }

        public bool IsCopyingEnabled
        {
            get
            {
                return _isCopyingEnabled;
            }
            set
            {
                if (_isCopyingEnabled != value)
                {
                    _isCopyingEnabled = value;

                    UserInteractionEnabled = _isCopyingEnabled;
                    _longPressGestureRecognizer.Enabled = _isCopyingEnabled;
                }
            }
        }

        public override bool CanPerform(Selector action, NSObject withSender)
        {
            if (action == new Selector("copy:"))
            {
                return IsCopyingEnabled;
            }

            return base.CanPerform(action, withSender);
        }

        public override void Copy(NSObject sender)
        {
            if (IsCopyingEnabled)
            {
                var pasteboard = UIPasteboard.General;
                var stringToCopy = Text;

                pasteboard.String = stringToCopy;
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                if (_longPressGestureRecognizer != null)
                {
                    RemoveGestureRecognizer(_longPressGestureRecognizer);
                    _longPressGestureRecognizer.Dispose();
                    _longPressGestureRecognizer = null;
                }
            }
        }

        private void Initialize()
        {
            _longPressGestureRecognizer =  new UILongPressGestureRecognizer(OnLongPress);
            AddGestureRecognizer(_longPressGestureRecognizer);

            IsCopyingEnabled = true;
        }

        private void OnLongPress(UIGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                BecomeFirstResponder();

                var copyMenu = UIMenuController.SharedMenuController;
                copyMenu.SetTargetRect(Bounds, this);
                copyMenu.ArrowDirection = UIMenuControllerArrowDirection.Default;
                copyMenu.SetMenuVisible(true, true);
            }
        }
    }
}