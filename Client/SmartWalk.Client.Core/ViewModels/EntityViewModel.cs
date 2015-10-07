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
        IContactsEntityProvider, IShareableViewModel
    {
        private readonly IConfiguration _configuration;
        private readonly IAnalyticsService _analyticsService;
        private readonly IEnvironmentService _environmentService;

        private Entity _entity;
        private bool _isDescriptionExpanded;
        private Entity _currentContactsEntityInfo;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand _showContactsCommand;
        private MvxCommand _hideContactsCommand;
        private MvxCommand<Contact> _callPhoneCommand;
        private MvxCommand<Contact> _composeEmailCommand;
        private MvxCommand _navigateAddressesCommand;
        private MvxCommand _showDirectionsCommand;
        private MvxCommand _copyLinkCommand;
        private MvxCommand _shareCommand;

        protected EntityViewModel(
            IConfiguration configuration,
            IEnvironmentService environmentService,
            IAnalyticsService analyticsService,
            IPostponeService postponeService) 
            : base(environmentService.Reachability, analyticsService, postponeService)
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

        public ICommand ShowContactsCommand
        {
            get
            {
                if (_showContactsCommand == null)
                {
                    _showContactsCommand = new MvxCommand(
                        () =>
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowContacts);

                            CurrentContactsEntityInfo = Entity;
                        },
                        () => Entity != null && Entity.HasContacts());
                }

                return _showContactsCommand;
            }
        }

        public ICommand HideContactsCommand
        {
            get
            {
                if (_hideContactsCommand == null)
                {
                    _hideContactsCommand = new MvxCommand(
                        () =>
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelHideContacts);

                            CurrentContactsEntityInfo = null;
                        },
                        () => CurrentContactsEntityInfo != null);
                }

                return _hideContactsCommand;
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

        public ICommand ShowDirectionsCommand
        {
            get
            {
                if (_showDirectionsCommand == null)
                {
                    _showDirectionsCommand = new MvxCommand(
                        () =>
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelShowDirections);

                                var address = Entity.Addresses.FirstOrDefault();
                                _environmentService.ShowDirections(address);
                            },
                        () => Entity != null && Entity.HasAddresses());
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