using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class EntityViewBase : ListViewBase
    {
        private ContactsView _contactsView;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (_contactsView != null)
            {
                SetDialogViewFullscreenFrame(_contactsView);
            }
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeContactsView();
            }
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            if (_contactsView != null)
            {
                SetDialogViewFullscreenFrame(_contactsView);
            }
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            var contactsProvider = ViewModel as IContactsEntityProvider;
            if (contactsProvider != null &&
                propertyName == contactsProvider.GetPropertyName(p => p.CurrentContactsEntityInfo))
            {
                ShowHideContactsView(contactsProvider.CurrentContactsEntityInfo);
            }
        }

        private void ShowHideContactsView(Entity entity)
        {
            var contactsProvider = ViewModel as IContactsEntityProvider;
            if (entity != null && contactsProvider != null)
            {
                _contactsView = View.Subviews.OfType<ContactsView>().FirstOrDefault();
                if (_contactsView == null)
                {
                    InitializeContactsView(contactsProvider);

                    View.Add(_contactsView);
                    UIView.Animate(
                        UIConstants.AnimationDuration, 
                        new NSAction(() => _contactsView.Alpha = 1));
                }

                _contactsView.Entity = entity;
            }
            else if (_contactsView != null)
            {
                UIView.Animate(
                    UIConstants.AnimationDuration, 
                    new NSAction(() => _contactsView.Alpha = 0),
                    new NSAction(_contactsView.RemoveFromSuperview));

                DisposeContactsView();
            }
        }

        private void InitializeContactsView(IContactsEntityProvider contactsProvider)
        {
            _contactsView = ContactsView.Create();

            SetDialogViewFullscreenFrame(_contactsView);

            _contactsView.CloseCommand = contactsProvider.ShowHideContactsCommand;
            _contactsView.CallPhoneCommand = contactsProvider.CallPhoneCommand;
            _contactsView.ComposeEmailCommand = contactsProvider.ComposeEmailCommand;
            _contactsView.NavigateWebSiteCommand = contactsProvider.NavigateWebLinkCommand;
        }

        private void DisposeContactsView()
        {
            if (_contactsView != null)
            {
                _contactsView.CloseCommand = null;
                _contactsView.CallPhoneCommand = null;
                _contactsView.ComposeEmailCommand = null;
                _contactsView.NavigateWebSiteCommand = null;
                _contactsView.Dispose();
                _contactsView = null;
            }
        }
    }
}