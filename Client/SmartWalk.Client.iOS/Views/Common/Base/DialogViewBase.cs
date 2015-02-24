using System;
using System.Windows.Input;
using Foundation;
using SmartWalk.Client.iOS.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class DialogViewBase : UIView
    {
        private bool _isInitialized;
        private ImmediateTouchRecognizer _outsideTouchGesture;

        protected DialogViewBase(IntPtr handle) : base(handle)
        {
        }

        public ICommand CloseCommand { get; set; }

        protected abstract UIView OutsideAreaView { get; }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                CloseCommand = null;

                DisposeGestures();
            }
        }

        public void Initialize()
        {
            if (!_isInitialized)
            {
                OnInitialize();
                _isInitialized = true;
            }
        }

        protected virtual void OnInitialize()
        {
            InitializeGestures();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected void CloseView()
        {
            if (CloseCommand != null &&
                CloseCommand.CanExecute(null))
            {
                CloseCommand.Execute(null);
            }
        }

        private void InitializeGestures()
        {
            if (_outsideTouchGesture == null)
            {
                _outsideTouchGesture = new ImmediateTouchRecognizer(CloseView);
                OutsideAreaView.AddGestureRecognizer(_outsideTouchGesture);
            }
        }

        private void DisposeGestures()
        {
            if (_outsideTouchGesture != null)
            {
                OutsideAreaView.RemoveGestureRecognizer(_outsideTouchGesture);
                _outsideTouchGesture.Dispose();
                _outsideTouchGesture = null;
            }
        }
    }

    public class ImmediateTouchRecognizer : UIGestureRecognizer
    {
        public ImmediateTouchRecognizer(Action handler) : base(handler)
        {
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            State = UIGestureRecognizerState.Recognized;
        }
    }
}