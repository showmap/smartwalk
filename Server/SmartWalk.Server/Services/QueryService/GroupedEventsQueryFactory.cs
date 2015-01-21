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
                QueryContext.Instance.EventMetadataView;
            var where = GetWhere(select);

            var result =
                session.CreateSQLQuery(
                    string.Format(
                        @"SELECT *
                        FROM
	                        (
		                        SELECT 
			                        EME.*, 
			                        ROW_NUMBER() OVER ({1}) as Row
		                        FROM
                                (
			                        SELECT HostGroups.HostId as HostId, MIN(EME.DaysTo) as DaysTo
			                        FROM
				                        (
					                        SELECT EntityRecord_Id AS HostId, MIN(ABS(DaysTo)) AS DaysTo
					                        FROM {0}
					                        {2}
					                        GROUP BY EntityRecord_Id
				                        ) HostGroups 
				                        INNER JOIN {0} EME
				                        ON 
					                        HostGroups.HostId = EME.EntityRecord_Id 
					                        AND HostGroups.DaysTo = ABS(EME.DaysTo)
			                        GROUP BY HostGroups.HostId
		                        ) EMRGroupped
		                        INNER JOIN {0} EME 
		                        ON 
			                        EMRGroupped.HostId = EME.EntityRecord_Id 
			                        {3}
			                        AND ((EMRGroupped.DaysTo = EME.DaysTo) 
				                        OR (EME.StartTime IS NOT NULL
					                        AND GETUTCDATE() - EMRGroupped.DaysTo - :range <= EME.StartTime AND EME.StartTime <= GETUTCDATE() - EMRGroupped.DaysTo + :range)
				                        OR (EME.EndTime IS NOT NULL
					                        AND GETUTCDATE() - EMRGroupped.DaysTo - :range <= EME.EndTime AND EME.EndTime <= GETUTCDATE() - EMRGroupped.DaysTo + :range)
				                        OR (EME.StartTime IS NOT NULL AND EME.EndTime IS NOT NULL
					                        AND EME.StartTime <= GETUTCDATE() - EMRGroupped.DaysTo - :range AND  GETUTCDATE() - EMRGroupped.DaysTo + :range <= EME.EndTime))
	                        ) EMROut
                        WHERE EMROut.Row > :offset and EMROut.Row <= :fetch
                        ORDER BY
	                        CASE -- Is Going On Now
		                        WHEN ISNULL(EMROut.StartTime, GETUTCDATE()) - 1 <= GETUTCDATE() 
			                        AND GETUTCDATE() <= ISNULL(EMROut.EndTime, ISNULL(EMROut.StartTime, GETUTCDATE())) + 1 
		                        THEN -1 ELSE 1
	                        END,
	                        ABS(ISNULL(DATEDIFF(d, EMROut.StartTime, GETUTCDATE()), 0))",
                        eventMetadataTableName,
                        GetRowNumberOrderBy(select, out latLong),
                        where != string.Empty ? "WHERE " + where : string.Empty,
                        where != string.Empty ? "AND " + where : string.Empty))
                    .AddEntity(typeof(EventMetadataRecord));

            result = 
                (ISQLQuery)result
                    .SetInt32("offset", select.Offset ?? 0)
                    .SetInt32("fetch", Math.Min(select.Fetch ?? QueryService.DefaultEventsLimit, 1000)) // hard limit for one page size to 1000 events
                    .SetInt32("range", 7);

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
                    result = "ABS(EME.Latitude - :lat) + ABS(EME.Longitude - :long) ASC";
                }
            }

            return result != string.Empty
                ? "ORDER BY " + result
                : "ORDER BY EME.Id";
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
                            "EMROut.{0} {1}",
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

            return result;
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