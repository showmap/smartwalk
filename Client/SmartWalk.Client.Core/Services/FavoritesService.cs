using System;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public class FavoritesService : IFavoritesService
    {
        private const string FavoritesDocument = "Favorites.json";
        
        private readonly IConfiguration _configuration;
        private readonly IMvxExtendedFileStore _fileStore;
        private readonly IFileService _fileService;
        private readonly string _docFolderPath;

        public FavoritesService(
            IConfiguration configuration, 
            IMvxExtendedFileStore fileStore,
            IFileService fileService)
        {
            _configuration = configuration;
            _fileStore = fileStore;
            _fileService = fileService;
            _docFolderPath = _fileStore.NativePath(_configuration.DocumentsPath);
        }

        public event EventHandler FavoritesUpdated;

        public Favorites LoadFavorites()
        {
            var result = _fileService.GetFileObject<Favorites>(_docFolderPath, FavoritesDocument);

            if (result == null)
            {
                result = new Favorites();
            }

            return result;
        }

        public void SaveFavorites(Favorites favorites)
        {
            favorites.LastUpdated = DateTime.UtcNow;
            _fileService.SetFileObject(_docFolderPath, FavoritesDocument, favorites);

            if (FavoritesUpdated != null)
            {
                FavoritesUpdated(this, EventArgs.Empty);
            }
        }
    }
}