using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
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
        IContactsEntityProvider,
        IShareableViewModel
    {
        private readonly IConfiguration _configuration;
        private readonly IAnalyticsService _analyticsService;
        private readonly IEnvironmentService _environmentService;

        private Entity _entity;
        private bool _isDescriptionExpanded;
        private Entity _currentContactsEntityInfo;
        private string _currentFullscreenImage;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand<string> _showFullscreenImageCommand;
        private MvxCommand<Entity> _showHideContactsCommand;
        private MvxCommand<Contact> _callPhoneCommand;
        private MvxCommand<Contact> _composeEmailCommand;
        private MvxCommand<Entity> _showDirectionsCommand;
        private MvxCommand<Contact> _navigateWebLinkCommand;
        private MvxCommand _navigateAddressesCommand;
        private MvxCommand _copyLinkCommand;
        private MvxCommand _shareCommand;

        protected EntityViewModel(
            IConfiguration configuration,
            IEnvironmentService environmentService,
            IAnalyticsService analyticsService) 
            : base(environmentService.Reachability, analyticsService)
        {
            _configuration = configuration;
            _environmentService = environmentService;
            _analyticsService = analyticsService;
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;

        public virtual string Subtitle
        {
            get { return null; }
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

        public ICommand ShowNextEntityCommand
        {
            get
            {
                if (_showNextEntityCommand == null)
                {
                    _showNextEntityCommand = 
                        new MvxCommand(() => { 
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowNextEntity);

                            OnShowNextEntity();
                        }, 
                        () => CanShowNextEntity);
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
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                image != null
                                    ? Analytics.ActionLabelShowFullscreenImage
                                    : Analytics.ActionLabelHideFullscreenImage);

                            CurrentFullscreenImage = image;
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
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                entity != null 
                                    ? Analytics.ActionLabelShowContacts
                                    : Analytics.ActionLabelHideContacts);

                            CurrentContactsEntityInfo = entity;
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
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCallPhone);

                            _environmentService.MakePhoneCall(contact.Title, contact.ContactText);
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
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelComposeEmail);

                            _environmentService.ComposeEmail(contact.ContactText, null, null, null, true);
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
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelShowDirections);

                                var address = entity.Addresses.FirstOrDefault();
                                _environmentService.ShowDirections(address);
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
                    _navigateAddressesCommand = new MvxCommand(
                        () => ShowViewModel<MapViewModel>(
                            new MapViewModel.Parameters {
                                Title = Entity.Name,
                                Addresses = new Addresses { Items = Entity.Addresses },
                                Location = InitParameters.Location
                            }),
                        () => 
                            Entity != null &&
                            Entity.HasAddresses());
                }

                return _navigateAddressesCommand;
            }
        }

        public ICommand CopyLinkCommand
        {
            get
            {
                if (_copyLinkCommand == null)
                {
                    _copyLinkCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCopyLink);

                            var url = _configuration
                                .GetEntityUrl(Entity.Id, Entity.Type.Value);
                            _environmentService.Copy(url);
                        },
                        () => Entity != null && Entity.Type.HasValue);
                }

                return _copyLinkCommand;
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShare);

                            var url = _configuration
                                .GetEntityUrl(Entity.Id, Entity.Type.Value);
                            if (url != null && Share != null)
                            {
                                Share(this, new MvxValueEventArgs<string>(url));
                            }
                        },
                        () => Entity != null && Entity.Type.HasValue);
                }

                return _shareCommand;
            }
        }

        public virtual bool CanShowNextEntity
        {
            get { return true; }
        }

        public virtual string NextEntityTitle
        {
            get { return null; }
        }

        protected virtual void OnShowNextEntity()
        {
        }
    }
}