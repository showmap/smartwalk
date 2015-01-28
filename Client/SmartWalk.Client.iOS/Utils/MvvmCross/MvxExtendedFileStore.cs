using System;
using System.IO;
using Cirrious.MvvmCross.Plugins.File.Touch;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxExtendedFileStore : MvxTouchFileStore, IMvxExtendedFileStore
    {
        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }
    }
}