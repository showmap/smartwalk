using System.ComponentModel;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public interface IEntityCellContext : INotifyPropertyChanged
    {
        Entity Entity { get; }
        string Title { get; }
        string Subtitle { get; }
        bool IsDescriptionExpanded { get; }
        EntityViewModelWrapper.ModelMode Mode { get; }
    }
}