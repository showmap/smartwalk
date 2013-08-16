using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.ViewModels
{
    public abstract class EntityViewModel : MvxViewModel
    {
        private Entity _entity;
        private bool _isDescriptionExpanded;

        private MvxCommand _expandCollapseCommand;
        private MvxCommand _showPreviousEntityCommand;
        private MvxCommand _showNextEntityCommand;
        private MvxCommand<WebSiteInfo> _navigateWebLinkCommand;

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