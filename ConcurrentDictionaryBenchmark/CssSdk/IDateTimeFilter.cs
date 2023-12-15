// <copyright file="IDateTimeFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts
{
    using System;

    /// <summary>
    /// DateTime filter
    /// </summary>
    public interface IDateTimeFilter
    {
        /// <summary>
        /// The property to filter on
        /// </summary>
        FilterProperty FilterProperty { get; }

        /// <summary>
        /// The datetime value to filter by
        /// </summary>
        DateTimeOffset DateTime { get; }

        /// <summary>
        /// Filter comparison operator
        /// </summary>
        ComparisonOperator ComparisonOperator { get; }
    }

    /// <summary>
    /// Properties that support filtering
    /// </summary>
    public enum FilterProperty
    {
        StartTime,
        EndTime,
    }

    /// <summary>
    /// Operators for comparing properties and values
    /// </summary>
    public enum ComparisonOperator
    {
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    /// <summary>
    /// Properties that support sort by
    /// </summary>
    public enum SortByProperty
    {
        StartTime,
    }

    /// <summary>
    /// Sort by direction
    /// </summary>
    public enum SortByDirection
    {
        Asc,
        Desc
    }
}
