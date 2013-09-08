using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels.Interfaces;
using SmartWalk.Core.ViewModels.Common;
using SmartWalk.Core.ViewModels;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.Core.ViewModels
{
    public abstract class EntityViewModel : RefreshableViewModel, IFullscreenImageProvider
    {
        private Entity _entity;
        private bool _isDescriptionExpanded;
        private string _currentFullscreenImage;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showPreviousEntityCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand<string> _showFullscreenImageCommand;
        private MvxCommand<WebSiteInfo> _navigateWebLinkCommand;
        private MvxCommand<Entity> _navigateAddressesCommand;

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
            private set
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

        public ICommand ExpandCollapseCommand
        {
            get 
            {
                if (_expandCollapseCommand == null)
                {
                    _expandCollapseCommand = 
                        new MvxCommand(() => IsDescriptionExpanded = !IsDescriptionExpanded);
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
                        new MvxCommand(OnShowPreviousEntity, () => CanShowNextEntity);
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
                        new MvxCommand(OnShowNextEntity, () => CanShowPreviousEntity);
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
                        image => CurrentFullscreenImage = image);
                }

                return _showFullscreenImageCommand;
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