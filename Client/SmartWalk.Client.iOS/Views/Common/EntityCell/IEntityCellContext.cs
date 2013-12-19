using SmartWalk.Client.Core.Model;
using System.ComponentModel;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public interface IEntityCellContext : INotifyPropertyChanged
    {
        Entity Entity { get; }

        bool IsDescriptionExpanded { get; }
    }
}