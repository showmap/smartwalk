using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    /// <summary>
    /// As seen on http://www.michaelridland.com/mobile/implementing-modernhttpclient-in-mvvmcross/
    /// </summary>
    public class MvxFastHttpFileDownloader : MvxLockableObject, IMvxHttpFileDownloader
    {
        private const int DefaultMaxConcurrentDownloads = 30;

        private readonly Dictionary<MvxFastFileDownloadRequest, bool> _currentRequests =
            new Dictionary<MvxFastFileDownloadRequest, bool>();
        private readonly Queue<MvxFastFileDownloadRequest> _queuedRequests = 
            new Queue<MvxFastFileDownloadRequest>();

        private readonly int _maxConcurrentDownloads;

        public MvxFastHttpFileDownloader(
            int maxConcurrentDownloads = DefaultMaxConcurrentDownloads)
        {
            _maxConcurrentDownloads = maxConcurrentDownloads;
        }

        public void RequestDownload(
            string url, 
            string downloadPath, 
            Action success, 
            Action<Exception> error)
        {
            var request = new MvxFastFileDownloadRequest(url, downloadPath);

            request.DownloadComplete += 
                (sender, args) =>
                {
                    OnRequestFinished(request);
                    success();
                };

            request.DownloadFailed += 
                (sender, args) =>
                {
                    OnRequestFinished(request);
                    error(args.Value);
                };

            RunAsyncWithLock(
                () =>
                {
                    _queuedRequests.Enqueue(request);

                    if (_currentRequests.Count < _maxConcurrentDownloads)
                    {
                        StartNextQueuedItem();
                    }
                });
        }

        private void OnRequestFinished(MvxFastFileDownloadRequest request)
        {
            RunAsyncWithLock(
                () =>
                {
                    _currentRequests.Remove(request);

                    if (_queuedRequests.Any())
                    {
                        StartNextQueuedItem();
                    }
                });
        }

        private void StartNextQueuedItem()
        {
            if (_currentRequests.Count >= _maxConcurrentDownloads) return;

            RunAsyncWithLock(
                () =>
                {
                    if (_currentRequests.Count >= _maxConcurrentDownloads) return;
                    if (!_queuedRequests.Any()) return;

                    var request = _queuedRequests.Dequeue();
                    _currentRequests.Add(request, true);
                    request.Start();
                });
        }
    }
}