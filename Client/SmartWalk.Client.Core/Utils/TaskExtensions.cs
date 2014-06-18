using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWalk.Client.Core.Utils
{
    public static class TaskExtensions
    {
        private static TaskScheduler _uiTaskScheduler;

        public static TaskScheduler UITaskScheduler
        {
            get
            {
                if (_uiTaskScheduler == null)
                {
                    if (UISynchronizationContext.Current == SynchronizationContext.Current)
                    {
                        _uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<TaskScheduler>();
                        UISynchronizationContext.Current.Post(
                            state => tcs.SetResult(TaskScheduler.FromCurrentSynchronizationContext()), 
                            null);
                        _uiTaskScheduler = tcs.Task.Result;
                    }
                }

                return _uiTaskScheduler;
            }
        }

        public static async void ContinueWithThrow(this Task task)
        {
            var previous = task;
            await previous;

            if (previous.IsFaulted)
            {
                UISynchronizationContext.Current.Post(
                    state => { throw previous.Exception; }, 
                    null);
            }
        }

        public static Task ContinueWithUIThread<TInput>(
            this Task<TInput> task, 
            Action<Task<TInput>> continuationAction)
        {
            return task.ContinueWith(continuationAction, UITaskScheduler);
        }

        public static Task<TResult> ContinueWithUIThread<TResult>(
            this Task task, 
            Func<Task, TResult> continuationFunction)
        {
            return task.ContinueWith(continuationFunction, UITaskScheduler);
        }

        public static Task<TResult> ContinueWithUIThread<TInput, TResult>(
            this Task<TInput> task, 
            Func<Task<TInput>, TResult> continuationFunction)
        {
            return task.ContinueWith(continuationFunction, UITaskScheduler);
        }

        public static Task ContinueWithUIThread(
            this Task task, 
            Action<Task> continuationAction)
        {
            return task.ContinueWith(continuationAction, UITaskScheduler);
        }
    }
}