using System.Threading.Tasks;

namespace SmartWalk.Client.Core.Utils
{
    public static class TaskExtensions
    {
        public static async void ContinueWithThrow(this Task task)
        {
            var previous = task;
            await previous;

            if (previous.IsFaulted)
            {
                UISynchronizationContext.Current.Post(state =>
                {
                    throw previous.Exception;
                }, null);
            }
        }
    }
}