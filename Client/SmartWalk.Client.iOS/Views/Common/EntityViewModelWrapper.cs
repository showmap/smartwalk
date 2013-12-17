using System.ComponentModel;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common.EntityCell;

namespace SmartWalk.iOS.Views.Common
{
    public class EntityViewModelWrapper : IEntityCellContext
    {
        private readonly EntityViewModel _viewModel;

        public EntityViewModelWrapper(EntityViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += (sender, e) => 
                {
                    if (e.PropertyName == _viewModel.GetPropertyName(p => p.IsDescriptionExpanded) && 
                        PropertyChanged != null)
                    {
                        PropertyChanged(this, e);
                    }
                };

            Entity = viewModel.Entity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Entity Entity { get; private set; }

        public bool IsDescriptionExpanded
        {
            get
            {
                return _viewModel.IsDescriptionExpanded;
            }
        }

        public override bool Equals(object obj)
        {
            var wrapper = obj as EntityViewModelWrapper;
            if (wrapper != null)
            {
                return Equals(Entity, wrapper.Entity);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial.CombineHashCodeOrDefault(Entity);
        }
    }
}