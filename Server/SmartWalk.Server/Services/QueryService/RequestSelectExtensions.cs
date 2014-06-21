using System.Collections.Generic;
using System.Linq;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services.QueryService
{
    public static class RequestSelectExtensions
    {
        /// <summary>
        /// Appends default where conditions to present only non-deleted publicly visible rows.
        /// </summary>
        public static RequestSelect AppendDefaultWhere(this RequestSelect select)
        {
            var wheres = new List<RequestSelectWhere>(select.Where ?? Enumerable.Empty<RequestSelectWhere>())
                {
                    new RequestSelectWhere
                        {
                            Field = QueryContext.Instance.EventMetadataIsDeleted,
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = false
                        },
                    new RequestSelectWhere
                        {
                            Field = QueryContext.Instance.EventMetadataIsPublic,
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = true
                        }
                };
            select.Where = wheres.ToArray();
            return select;
        }
    }
}