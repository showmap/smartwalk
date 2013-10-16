using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Constants;
using SmartWalk.Core.Model;
using SmartWalk.Core.Model.Interfaces;
using SmartWalk.Core.Services;
using SmartWalk.Core.ViewModels;
using SmartWalk.Core.ViewModels.Common;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.Core.ViewModels
{
    public abstract class EntityViewModel : RefreshableViewModel, 
        IFullscreenImageProvider,
        IContactsEntityProvider
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IMvxPhoneCallTask _phoneCallTask;
        private readonly IMvxComposeEmailTask _composeEmailTask;

        private Entity _entity;
        private bool _isDescriptionExpanded;
        private EntityInfo _currentContactsEntityInfo;
        private string _currentFullscreenImage;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showPreviousEntityCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand<string> _showFullscreenImageCommand;
        private MvxCommand<EntityInfo> _showHideContactsCommand;
        private MvxCommand<PhoneInfo> _callPhoneCommand;
        private MvxCommand<EmailInfo> _composeEmailCommand;
        private MvxCommand<WebSiteInfo> _navigateWebLinkCommand;
        private MvxCommand<Entity> _navigateAddressesCommand;

        protected EntityViewModel(
            IAnalyticsService analyticsService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask) : 
            base(analyticsService)
        {
            _analyticsService = analyticsService;
            _phoneCallTask = phoneCallTask;
            _composeEmailTask = composeEmailTask;
        }

        public Entity Entity
        {
            get
            {
                return _entity;
            }
            protected set
            {
                if (!Equals(_entity, value))
                {
                    _entity = value;
                    RaisePropertyChanged(() => Entity);
                    IsDescriptionExpanded = false;
                }
            }
        }

        public bool IsDescriptionExpanded
        {
            get
            {
                return _isDescriptionExpanded;
            }
            protected set
            {
                if (_isDescriptionExpanded != value)
                {
                    _isDescriptionExpanded = value;
                    RaisePropertyChanged(() => IsDescriptionExpanded);
                }
            }
        }

        public string CurrentFullscreenImage
        {
            get
            {
                return _currentFullscreenImage;
            }
            private set
            {
                if (_currentFullscreenImage != value)
                {
                    _currentFullscreenImage = value;
                    RaisePropertyChanged(() => CurrentFullscreenImage);
                }
            }
        }

        public EntityInfo CurrentContactsEntityInfo
        {
            get
            {
                return _currentContactsEntityInfo;
            }
            private set
            {
                if (!Equals(_currentContactsEntityInfo, value))
                {
                    _currentContactsEntityInfo = value;
                    RaisePropertyChanged(() => CurrentContactsEntityInfo);
                }
            }
        }

        public ICommand ExpandCollapseCommand
        {
            get 
            {
                if (_expandCollapseCommand == null)
                {
                    _expandCollapseCommand = 
                        new MvxCommand(() => {
                            IsDescriptionExpanded = !IsDescriptionExpanded;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                IsDescriptionExpanded 
                                    ? Analytics.ActionLabelExpandDescription 
                                    : Analytics.ActionLabelCollapseDescription);
                        });
                }

                return _expandCollapseCommand;
            }
        }

        public ICommand ShowPreviousEntityCommand
        {
            get
            {
                if (_showPreviousEntityCommand == null)
                {
                    _showPreviousEntityCommand = 
                        new MvxCommand(() => {
                            OnShowPreviousEntity();

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowPreviousEntity);
                        },
                        () => CanShowNextEntity);
                }

                return _showPreviousEntityCommand;
            }
        }

        public ICommand ShowNextEntityCommand
        {
            get
            {
                if (_showNextEntityCommand == null)
                {
                    _showNextEntityCommand = 
                        new MvxCommand(() => { 
                            OnShowNextEntity();

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowNextEntity);
                        }, 
                        () => CanShowPreviousEntity);
                }

                return _showNextEntityCommand;
            }
        }

        public ICommand ShowHideFullscreenImageCommand
        {
            get
            {
                if (_showFullscreenImageCommand == null)
                {
                    _showFullscreenImageCommand = new MvxCommand<string>(
                        image => {
                            CurrentFullscreenImage = image;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentFullscreenImage != null
                                    ? Analytics.ActionLabelShowFullscreenImage
                                    : Analytics.ActionLabelHideFullscreenImage);
                        });
                }

                return _showFullscreenImageCommand;
            }
        }

        public ICommand ShowHideContactsCommand
        {
            get
            {
                if (_showHideContactsCommand == null)
                {
                    _showHideContactsCommand = new MvxCommand<EntityInfo>(
                        entityInfo => {
                            CurrentContactsEntityInfo = entityInfo;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentContactsEntityInfo != null 
                                    ? Analytics.ActionLabelShowContacts
                                    : Analytics.ActionLabelHideContacts);
                        });
                }

                return _showHideContactsCommand;
            }
        }

        public ICommand CallPhoneCommand
        {
            get
            {
                if (_callPhoneCommand == null)
                {
                    _callPhoneCommand = new MvxCommand<PhoneInfo>(
                        info => {
                            _phoneCallTask.MakePhoneCall(info.Name, info.Phone);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCallPhone);
                        },
                        info => info != null && info.Phone != null);
                }

                return _callPhoneCommand;
            }
        }

        public ICommand ComposeEmailCommand
        {
            get
            {
                if (_composeEmailCommand == null)
                {
                    _composeEmailCommand = new MvxCommand<EmailInfo>(
                        info => {
                            _composeEmailTask.ComposeEmail(info.Email, null, null, null, true);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelComposeEmail);
                        },
                    info => info != null && info.Email != null);
                }

                return _composeEmailCommand;
            }
        }

        public ICommand NavigateWebLinkCommand
        {
            get
            {
                if (_navigateWebLinkCommand == null)
                {
                    _navigateWebLinkCommand = new MvxCommand<WebSiteInfo>(
                        info => ShowViewModel<BrowserViewModel>(
                            new BrowserViewModel.Parameters {  
                                URL = info.URL
                            }),
                        info => info != null);
                }

                return _navigateWebLinkCommand;
            }
        }

        public ICommand NavigateAddressesCommand
        {
            get
            {
                if (_navigateAddressesCommand == null)
                {
                    _navigateAddressesCommand = new MvxCommand<Entity>(
                        entity => ShowViewModel<MapViewModel>(
                            new MapViewModel.Parameters {
                                Title = entity.Info.Name,
                                Number = entity is INumberEntity ? ((INumberEntity)entity).Number : 0,
                                Addresses = new Addresses { Items = entity.Info.Addresses }
                            }),
                        entity => 
                            entity != null && 
                            entity.Info != null && 
                            entity.Info.Addresses != null &&
                            entity.Info.Addresses.Length > 0);
                }

                return _navigateAddressesCommand;
            }
        }

        public virtual bool CanShowNextEntity
        {
            get { return true; }
        }

        public virtual bool CanShowPreviousEntity
        {
            get { return true; }
        }

        protected virtual void OnShowPreviousEntity()
        {
        }

        protected virtual void OnShowNextEntity()
        {
        }
    }
}