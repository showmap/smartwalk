using SmartWalk.Core.Model;

namespace SmartWalk.Core.Utils
{
    public static class EntityInfoExtensions
    {
        public static bool HasLogo(this EntityInfo info)
        {
            return info != null && info.Logo != null;
        }

        public static bool HasContact(this EntityInfo info)
        {
            return info != null && info.Contact != null && !info.Contact.IsEmpty;
        }

        public static bool HasAddress(this EntityInfo info)
        {
            return info != null && info.Addresses != null && info.Addresses.Length > 0;
        }
    }
}