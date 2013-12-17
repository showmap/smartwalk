using SmartWalk.Core.Model;
using System.ComponentModel;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public interface IEntityCellContext : INotifyPropertyChanged
    {
        Entity Entity { get; }

        bool IsDescriptionExpanded { get; }
    }
}