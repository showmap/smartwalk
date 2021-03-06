using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.DownloadCache;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedDynamicImageHelper<T> : IMvxResizedImageHelper<T> where T : class
    {
        private MvxResizedImageRequest<T> _currentImageRequest;

        private ImageState _currentImageState = ImageState.DefaultShown;

        private string _defaultImagePath;

        private string _errorImagePath;

        private string _imageUrl;

        public ImageState CurrentImageState
        {
            get { return _currentImageState; }
        }

        public string DefaultImagePath
        {
            get { return _defaultImagePath; }
            set
            {
                if (_defaultImagePath == value)
                    return;
                _defaultImagePath = value;
                OnImagePathChanged();

                if (string.IsNullOrEmpty(_errorImagePath))
                    ErrorImagePath = value;
            }
        }

        public string ErrorImagePath
        {
            get { return _errorImagePath; }
            set
            {
                if (_errorImagePath == value)
                    return;
                _errorImagePath = value;
                OnImagePathChanged();
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (_imageUrl == value)
                    return;
                _imageUrl = value;
                RequestImage(_imageUrl);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        ~MvxResizedDynamicImageHelper()
        {
            Dispose(false);
        }

        public event EventHandler<MvxValueEventArgs<T>> ImageChanged;

        private void FireImageChanged(T image)
        {
            var handler = ImageChanged;
            if (handler != null)
                handler(this, new MvxValueEventArgs<T>(image));
        }

        private void RequestImage(string imageSource)
        {
            ClearCurrentHttpImageRequest();

            if (string.IsNullOrEmpty(imageSource))
            {
                ShowDefaultImage();
                return;
            }

            if (imageSource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                NewHttpImageRequested();

                _currentImageRequest = new MvxResizedImageRequest<T>(imageSource);
                _currentImageRequest.Complete += CurrentImageRequestOnComplete;
                _currentImageRequest.Error += CurrentImageRequestOnError;
                _currentImageRequest.Start();
            }
            else
            {
                var image = ImageFromLocalFile(imageSource);
                if (image == null)
                {
                    ShowErrorImage();
                }
                else
                {
                    NewImageAvailable(image);
                }
            }
        }

        private void ClearCurrentHttpImageRequest()
        {
            if (_currentImageRequest == null)
                return;

            _currentImageRequest.Complete -= CurrentImageRequestOnComplete;
            _currentImageRequest.Error -= CurrentImageRequestOnError;

            _currentImageRequest = null;
        }

        private void CurrentImageRequestOnError(object sender, MvxValueEventArgs<Exception> mvxExceptionEventArgs)
        {
            if (sender != _currentImageRequest)
                return;

            HttpImageErrorSeen();
            ClearCurrentHttpImageRequest();
        }

        private void CurrentImageRequestOnComplete(object sender, MvxValueEventArgs<T> mvxValueEventArgs)
        {
            if (sender != _currentImageRequest)
                return;

            var image = mvxValueEventArgs.Value;
            NewImageAvailable(image);
            ClearCurrentHttpImageRequest();
        }

        private void OnImagePathChanged()
        {
            switch (_currentImageState)
            {
                case ImageState.ErrorShown:
                    ShowErrorImage();
                    break;
                case ImageState.DefaultShown:
                    ShowDefaultImage();
                    break;
            }
        }

        private void ShowDefaultImage()
        {
            ShowLocalFile(_defaultImagePath);
        }

        private void ShowErrorImage()
        {
            ShowLocalFile(_errorImagePath);
        }

        private void ShowLocalFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                FireImageChanged(null);
            }
            else
            {
                var localImage = ImageFromLocalFile(filePath);
                if (localImage == null)
                {
                    MvxTrace.Warning( "Failed to load local image for filePath {0}", filePath);
                }
                FireImageChanged(ImageFromLocalFile(filePath));
            }
        }

        private T ImageFromLocalFile(string path)
        {
            var loader = Mvx.Resolve<IMvxLocalFileImageLoader<T>>();
            var wrapped = loader.Load(path, true);
            return wrapped.RawImage;
        }

        private void NewHttpImageRequested()
        {
            _currentImageState = ImageState.DefaultShown;
            ShowDefaultImage();
        }

        private void HttpImageErrorSeen()
        {
            _currentImageState = ImageState.ErrorShown;
            ShowErrorImage();
        }

        private void NewImageAvailable(T image)
        {
            _currentImageState = ImageState.HttpImageShown;
            FireImageChanged(image);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                ClearCurrentHttpImageRequest();
            }
        }
    }
}