using Orchard.Security;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Utils
{
    public enum AccessType
    {
        Deny,
        AllowView,
        AllowEdit
    }

    public interface IAccessRecord
    {
        SmartWalkUserRecord SmartWalkUserRecord { get; }
    }

    public static class SecurityUtil
    {
        public static AccessType GetAccess(IAuthorizer authorizer)
        {
            var result =
                authorizer.Authorize(Permissions.EditOwnContent)
                || authorizer.Authorize(Permissions.EditAllContent)
                    ? AccessType.AllowEdit
                    : (authorizer.Authorize(Permissions.ViewAllContent)
                           ? AccessType.AllowView
                           : AccessType.Deny);
            return result;
        }

        public static AccessType GetAccess(
            this IAccessRecord record,
            IAuthorizer authorizer,
            SmartWalkUserRecord user)
        {
            if (record == null) return AccessType.AllowView;

            var allowEdit =
                user != null
                && (authorizer.Authorize(Permissions.EditAllContent)
                    || (record.SmartWalkUserRecord != null
                        && record.SmartWalkUserRecord.Id == user.Id
                        && authorizer.Authorize(Permissions.EditOwnContent)));

            var result =
                allowEdit
                    ? AccessType.AllowEdit
                    : (authorizer.Authorize(Permissions.ViewAllContent)
                           ? AccessType.AllowView
                           : AccessType.Deny);
            return result;
        }
    }
}