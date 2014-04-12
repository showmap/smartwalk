using System.ComponentModel;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public interface IEntityCellContext : INotifyPropertyChanged
    {
        Entity Entity { get; }

        bool IsDescriptionExpanded { get; }
    }
}