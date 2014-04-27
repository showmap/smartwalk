using System.ComponentModel;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
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