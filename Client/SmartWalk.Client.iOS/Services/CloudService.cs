using System.IO;
using System.Threading;
using Foundation;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils.iCloud;
using UIKit;

namespace SmartWalk.Client.iOS.Services
{
    public class CloudService : ICloudService
    {
        private readonly IFavoritesService _favoritesService;

        private NSUrl _cloudUrl;
        private FavoritesDocument _document;

        public CloudService(IFavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
            _favoritesService.FavoritesUpdated += 
                (s, e) => SaveFavoritesDocument();

            new Thread(
                new ThreadStart(() =>
                    {
                        var url = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null);
                        _cloudUrl = url;
                    })).Start();

            LoadFavorites();
        }

        private void LoadFavorites()
        {
            var query = new NSMetadataQuery();
            query.SearchScopes = new NSObject[] { NSMetadataQuery.UbiquitousDocumentsScope };

            var pred = 
                NSPredicate.FromFormat(
                    "%K == %@",
                    new NSObject[] {
                        NSMetadataQuery.ItemFSNameKey,
                        new NSString(FavoritesService.FavoritesFileName)
                    });
            query.Predicate = pred;

            NSNotificationCenter.DefaultCenter.AddObserver(
                NSMetadataQuery.DidFinishGatheringNotification,
                OnDidFinishGathering,
                query);
            
            query.StartQuery();
        }

        private void OnDidFinishGathering(NSNotification notification) 
        {
            var query = (NSMetadataQuery)notification.Object;
            query.DisableUpdates();
            query.StopQuery();
            
            LoadFavoritesDocument(query);
        }

        private void LoadFavoritesDocument(NSMetadataQuery query) 
        {
            if (query.ResultCount == 1)
            {
                var item = (NSMetadataItem)query.ResultAtIndex(0);
                var url = (NSUrl)item.ValueForAttribute(NSMetadataQuery.ItemURLKey);
                _document = new FavoritesDocument(url);
                _document.Open(success =>
                    {
                        if (success && _document.Data != null)
                        {
                            var favorites = _favoritesService.LoadFavorites();
                            if (favorites.LastUpdated < _document.Data.LastUpdated)
                            {
                                _favoritesService.SaveFavorites(_document.Data);
                            }
                        }
                    });
            }
            else if (query.ResultCount == 0)
            {
                SaveFavoritesDocument();
            }
        }

        private void SaveFavoritesDocument()
        {
            if (_cloudUrl == null) return;

            var favorites = _favoritesService.LoadFavorites();
            if (favorites == null) return;

            if (_document == null)
            {
                var docsFolder = Path.Combine(_cloudUrl.Path, "Documents");
                var docPath = Path.Combine(docsFolder, FavoritesService.FavoritesFileName);
                var ubiq = new NSUrl(docPath, false);

                var document = new FavoritesDocument(ubiq);
                document.Save(
                    document.FileUrl, 
                    UIDocumentSaveOperation.ForCreating, 
                    success =>
                    {
                        if (success)
                        {
                            _document = document;
                            _document.Data = favorites;
                            _document.UpdateChangeCount(UIDocumentChangeKind.Done);
                        }
                    });
            }
            else
            {
                _document.Data = favorites;
                _document.UpdateChangeCount(UIDocumentChangeKind.Done);
            }
        }
    }
}