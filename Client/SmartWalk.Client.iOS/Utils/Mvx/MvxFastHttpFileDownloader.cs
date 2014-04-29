using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS.Utils.Mvx
{
    /// <summary>
    /// As seen on http://www.michaelridland.com/mobile/implementing-modernhttpclient-in-mvvmcross/
    /// </summary>
    public class MvxFastHttpFileDownloader 
        : MvxLockableObject, IMvxHttpFileDownloader
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

            RunSyncOrAsyncWithLock(
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
            RunSyncOrAsyncWithLock(
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

            RunSyncOrAsyncWithLock(
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

    public class MvxFastFileDownloadRequest
    {
        public MvxFastFileDownloadRequest(string url, string downloadPath)
        {
            Url = url;
            DownloadPath = downloadPath;
        }

        public string DownloadPath { get; private set; }
        public string Url { get; private set; }

        public event EventHandler<MvxFileDownloadedEventArgs> DownloadComplete;
        public event EventHandler<MvxValueEventArgs<Exception>> DownloadFailed;

        public void Start()
        {
            var client = HttpService.CreateHttpClient();
            client
                .GetAsync(Url)
                .ContinueWith(
                response =>
                {
                    var httpResult = response.Result;
                    httpResult.EnsureSuccessStatusCode(); 
                    httpResult.Content
                            .ReadAsStreamAsync()
                            .ContinueWith(
                                HandleSuccess, 
                                TaskContinuationOptions.NotOnFaulted)
                            .ContinueWith(
                                ae => FireDownloadFailed(ae.Exception), 
                                TaskContinuationOptions.OnlyOnFaulted);

                })
                .ContinueWith(
                    ae => FireDownloadFailed(ae.Exception), 
                    TaskContinuationOptions.OnlyOnFaulted);
        }

        private void HandleSuccess(Task<Stream> result)
        {
            try
            {
                var fileService = MvxFileStoreHelper.SafeGetFileStore();
                var tempFilePath = DownloadPath + ".tmp";

                using (result.Result)
                {
                    fileService.WriteFile(tempFilePath, result.Result.CopyTo);
                }

                fileService.TryMove(tempFilePath, DownloadPath, true);
            }
            catch (Exception exception)
            {
                FireDownloadFailed(exception);
                return;
            }

            FireDownloadComplete();
        }

        private void FireDownloadFailed(Exception exception)
        {
            var handler = DownloadFailed;
            if (handler != null)
            {
                handler(this, new MvxValueEventArgs<Exception>(exception));
            }
        }

        private void FireDownloadComplete()
        {
            var handler = DownloadComplete;
            if (handler != null)
            {
                handler(this, new MvxFileDownloadedEventArgs(Url, DownloadPath));
            }
        }
    }
}