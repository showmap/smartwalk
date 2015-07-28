using System;
using Foundation;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Model;
using UIKit;

namespace SmartWalk.Client.iOS.Utils.iCloud
{
    public class FavoritesDocument : UIDocument
    {
        public FavoritesDocument(NSUrl url) : base (url)
        {
        }

        public Favorites Data { get; set; }

        public override bool LoadFromContents(NSObject contents, string typeName, out NSError outError)
        {
            outError = null;

            if (contents != null)
            {
                var str = NSString.FromData((NSData)contents, NSStringEncoding.UTF8);

                try
                {
                    Data = JsonConvert.DeserializeObject<Favorites>(str);
                }
                catch (Exception ex)
                {
                    outError = new NSError(new NSString(ex.Message), 0);
                    return false;
                }
            }

            return true;
        }

        public override NSObject ContentsForType(string typeName, out NSError outError)
        {
            outError = null;
            NSObject result;

            try
            {
                var str = Data != null ? JsonConvert.SerializeObject(Data) : null;
                result = new NSString(str ?? string.Empty).Encode(NSStringEncoding.UTF8);
            }
            catch (Exception ex)
            {
                outError = new NSError(new NSString(ex.Message), 0);
                return null;
            }

            return result;
        }
    }
}