using System;
using Cirrious.MvvmCross.Plugins.File;

namespace SmartWalk.Client.Core.Services
{
    public interface IMvxExtendedFileStore : IMvxFileStore
    {
        DateTime GetLastWriteTime(string path);
    }
}