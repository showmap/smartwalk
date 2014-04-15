using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.ViewModels
{
    public abstract class EntityViewModel : RefreshableViewModel, 
        IFullscreenImageProvider,
        IContactsEntityProvider
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IMvxPhoneCallTask _phoneCallTask;
        private readonly IMvxComposeEmailTask _composeEmailTask;
        private readonly IShowDirectionsTask _showDirectionsTask;

        private Entity _entity;
        private bool _isDescriptionExpanded;
        private Entity _currentContactsEntityInfo;
        private string _currentFullscreenImage;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showPreviousEntityCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand<string> _showFullscreenImageCommand;
        private MvxCommand<Entity> _showHideContactsCommand;
        private MvxCommand<Contact> _callPhoneCommand;
        private MvxCommand<Contact> _composeEmailCommand;
        private MvxCommand<Entity> _showDirectionsCommand;
        private MvxCommand<Contact> _navigateWebLinkCommand;
        private MvxCommand<Entity> _navigateAddressesCommand;

        protected EntityViewModel(
            IAnalyticsService analyticsService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IShowDirectionsTask showDirectionsTask) : 
            base(analyticsService)
        {
            _analyticsService = analyticsService;
            _phoneCallTask = phoneCallTask;
            _composeEmailTask = composeEmailTask;
            _showDirectionsTask = showDirectionsTask;
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

                    if (CurrentContactsEntityInfo != null)
                    {
                        CurrentContactsEntityInfo = Entity;
                    }
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

        public Entity CurrentContactsEntityInfo
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
                    _showHideContactsCommand = new MvxCommand<Entity>(
                        entity => 
                        {
                            CurrentContactsEntityInfo = entity;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentContactsEntityInfo != null 
                                    ? Analytics.ActionLabelShowContacts
                                    : Analytics.ActionLabelHideContacts);
                        },
                        entity => entity == null || entity.HasContacts());
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
                    _callPhoneCommand = new MvxCommand<Contact>(
                        contact =>
                        {
                            _phoneCallTask.MakePhoneCall(contact.Title, contact.ContactText);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCallPhone);
                        },
                        contact => 
                            contact != null && 
                            contact.Type == SmartWalk.Shared.DataContracts.ContactType.Phone && 
                            contact.ContactText != null);
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
                    _composeEmailCommand = new MvxCommand<Contact>(
                        contact =>
                        {
                            _composeEmailTask.ComposeEmail(contact.ContactText, null, null, null, true);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelComposeEmail);
                        },
                        contact => 
                            contact != null &&
                            contact.Type == SmartWalk.Shared.DataContracts.ContactType.Email &&
                            contact.ContactText != null);
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
                    _navigateWebLinkCommand = new MvxCommand<Contact>(
                        contact => ShowViewModel<BrowserViewModel>(
                            new BrowserViewModel.Parameters {  
                                URL = contact.ContactText,
                                Location = InitParameters.Location
                            }),
                        contact => 
                            contact != null &&
                            contact.Type == SmartWalk.Shared.DataContracts.ContactType.Url &&
                            contact.ContactText != null);
                }

                return _navigateWebLinkCommand;
            }
        }

        public ICommand ShowDirectionsCommand
        {
            get
            {
                if (_showDirectionsCommand == null)
                {
                    _showDirectionsCommand = new MvxCommand<Entity>(
                        entity =>
                            {
                                var address = entity.Addresses.FirstOrDefault();

                                _showDirectionsTask.ShowDirections(address);

                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelShowDirections);
                            },
                        entity => 
                            entity != null &&
                            entity.HasAddresses());
                }

                return _showDirectionsCommand;
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
                                Title = entity.Name,
                                Number = 0, // TODO: To support letters
                                Addresses = new Addresses { Items = entity.Addresses },
                                Location = InitParameters.Location
                            }),
                        entity => 
                            entity != null &&
                            entity.HasAddresses());
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