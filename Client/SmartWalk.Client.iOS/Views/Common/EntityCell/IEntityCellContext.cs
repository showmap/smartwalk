using System;
using System.ComponentModel;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public interface IEntityCellContext : INotifyPropertyChanged
    {
        Entity Entity { get; }
        string Title { get; }
        string Subtitle { get; }
        string Description { get; }
        bool IsDescriptionExpanded { get; }
    }

    public static class EntityCellContextExtensions
    {
        public static string FullDescription(this IEntityCellContext context)
        {
            if (context.Description == null)
            {
                return context.Entity.Description;
            }

            if (context.Entity.Description == null)
            {
                return context.Description;
            }

            var result = string.Format(
                "{0}{1}{1}{2}", 
                context.Description, 
                Environment.NewLine,
                context.Entity.Description);
            return result;
        }
    }
}