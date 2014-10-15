using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using SmartWalk.Server.Records;
using SmartWalk.Server.Resources;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.QueryService
{
    public static class GroupedEventsQueryFactory
    {
        /// <summary>
        /// Generates an SQL query for a query accross grouped by Host 
        /// EventMetadata table's records with a where condition and order by clause.
        /// </summary>
        public static ISQLQuery CreateQuery(
            ISession session,
            RequestSelect select,
            IDictionary<string, object[]> results)
        {
            double[] latLong;

            var eventMetadataTableName =
                QueryContext.Instance.DbPrefix +
                QueryContext.Instance.EventMetadataTable;

            var result =
                session.CreateSQLQuery(
                    string.Format(
                        @"SELECT EMR.*
                        FROM
	                        (SELECT EMR.*, ROW_NUMBER() OVER ({3}) as Row
                            FROM
	                            (SELECT 
		                            EntityRecord_Id AS HostId, MAX(StartTime) AS StartTime
	                            FROM 
		                            {0}
                                {4}
	                            GROUP BY EntityRecord_Id) EMRGroupped
                            INNER JOIN
	                            {0} EMR ON EMRGroupped.HostId = EMR.EntityRecord_Id AND EMRGroupped.StartTime = EMR.StartTime) EMR
                        WHERE EMR.Row > {1} and EMR.Row <= {2}
                        {5}",
                        eventMetadataTableName,
                        select.Offset ?? 0,
                        Math.Min(select.Fetch ?? QueryService.DefaultEventsLimit, 1000), // hard limit for one page size to 1000 events
                        GetRowNumberOrderBy(select, out latLong),
                        GetWhere(select),
                        GetOrderBy(select)))
                       .AddEntity(typeof(EventMetadataRecord));

            if (latLong != null)
            {
                result =
                    (ISQLQuery)result
                        .SetDouble("lat", latLong[0])
                        .SetDouble("long", latLong[1]);
            }

            if (select.Where != null)
            {
                for (var i = 0; i < select.Where.Length; i++)
                {
                    result =
                        (ISQLQuery)result
                            .SetParameter("v" + i, select.Where[i].Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Generates a string for an index order.
        /// </summary>
        private static string GetRowNumberOrderBy(
            RequestSelect select,
            out double[] latLong)
        {
            var result = string.Empty;
            latLong = null;

            // if there is sort by condition then build sort by expression
            if (select.SortBy != null && select.SortBy.Length > 0)
            {
                if (IsLatLongDistanceSorting(select.SortBy, out latLong))
                {
                    result = "ABS(EMR.Latitude - :lat) + ABS(EMR.Longitude - :long) ASC";
                }
            }

            return result != string.Empty
                ? "ORDER BY " + result
                : "ORDER BY EMR.Id";
        }

        /// <summary>
        /// Generates a string for a grouped events query accross table's records with sorting.
        /// </summary>
        private static string GetOrderBy(RequestSelect select)
        {
            var result = string.Empty;

            // if there is sort by condition then build sort by expression
            if (select.SortBy != null)
            {
                foreach (var sortBy in select.SortBy)
                {
                    if (SkipLatLongDistanceSorting(sortBy)) continue;

                    if (!QueryContext.Instance.EventMetadataProperties.ContainsIgnoreCase(sortBy.Field))
                    {
                        throw new InvalidExpressionException(
                            string.Format(
                                Localization.CantFindFieldInRequestedItems,
                                sortBy.Field));
                    }

                    result +=
                        (result != string.Empty ? ", " : string.Empty) +
                        string.Format(
                            "EMR.{0} {1}",
                            sortBy.Field,
                            sortBy.IsDescending.HasValue &&
                            sortBy.IsDescending.Value
                                ? "DESC"
                                : "ASC");
                }
            }

            return result != string.Empty
                ? "ORDER BY " + result
                : string.Empty;
        }

        // TODO: To support the rest of where's value cases
        /// <summary>
        /// Generates a string for a generic query accross table's records with where expression.
        /// </summary>
        private static string GetWhere(RequestSelect select)
        {
            var result = string.Empty;

            if (select.Where != null && select.Where.Length > 0)
            {
                for (var i = 0; i < select.Where.Length; i++)
                {
                    var where = select.Where[i];
                    if (QueryContext.Instance.EventMetadataProperties
                            .Contains(where.Field, StringComparer.OrdinalIgnoreCase))
                    {
                        if (RequestSelectWhereOperators.All
                                .Contains(where.Operator, StringComparer.OrdinalIgnoreCase))
                        {
                            result +=
                                (result != string.Empty ? " AND " : string.Empty) +
                                string.Format(
                                    "({0}{1}:v{2})",
                                    where.Field,
                                    where.Operator,
                                    i);
                        }
                        else
                        {
                            throw new InvalidExpressionException(
                                string.Format("Operator '{0}' is not supported.", where.Operator));
                        }
                    }
                    else
                    {
                        throw new InvalidExpressionException(
                            string.Format("Can't find field '{0}'.", where.Field));
                    }
                }
            }

            return result != string.Empty
                ? "WHERE " + result
                : string.Empty;
        }

        private static bool IsLatLongDistanceSorting(
            RequestSelectSortBy[] sortBy,
            out double[] latLong)
        {
            var latitude = sortBy
                .FirstOrDefault(
                    sb => sb.Field.EqualsIgnoreCase(QueryContext.Instance.EventMetadataLatitude));
            var longitude = sortBy
                .FirstOrDefault(
                    sb => sb.Field.EqualsIgnoreCase(QueryContext.Instance.EventMetadataLongitude));

            if (latitude != null && latitude.OfDistance.HasValue &&
                longitude != null && longitude.OfDistance.HasValue)
            {
                latLong = new[] { latitude.OfDistance.Value, longitude.OfDistance.Value };
                return true;
            }

            latLong = null;
            return false;
        }

        private static bool SkipLatLongDistanceSorting(
            RequestSelectSortBy sortBy)
        {
            return (sortBy.Field == QueryContext.Instance.EventMetadataLatitude ||
                    sortBy.Field == QueryContext.Instance.EventMetadataLongitude) &&
                    sortBy.OfDistance.HasValue;
        }
    }
}