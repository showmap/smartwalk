using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Utils
{
    public class ViewsFactory<T> where T : UIView
    {
        private readonly Func<T> _createHandler;
        private readonly List<T> _viewsQueue = new List<T>();

        public ViewsFactory(Func<T> createHandler)
        {
            _createHandler = createHandler;
        }

        public ViewsFactory(Func<T> createHandler, int initialCount)
        {
            _createHandler = createHandler;

            if (initialCount > 0)
            {
                for (var i = 0; i < initialCount; i++)
                {
                    _viewsQueue.Add(_createHandler());
                }
            }
        }

        public T DequeueReusableView()
        {
            var recycledView = _viewsQueue.FirstOrDefault(v => v.Superview == null);
            if (recycledView == null)
            {
                recycledView = _createHandler();
                _viewsQueue.Add(recycledView);
            }

            return recycledView;
        }
    }
}