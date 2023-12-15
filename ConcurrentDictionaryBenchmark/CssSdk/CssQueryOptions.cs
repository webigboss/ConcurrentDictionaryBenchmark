// <copyright file="CssQueryOptions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Query options supported by CSS
    /// </summary>
    public record CssQueryOptions : ICssQueryOptions
    {
        /// <inheritdoc/>
        public IReadOnlySet<SignalType> SignalTypes { get; init; }

        /// <inheritdoc/>
        public IDateTimeFilter StartFilter { get; init; }

        /// <inheritdoc/>
        public IDateTimeFilter EndFilter { get; init; }

        /// <inheritdoc/>
        public SortByProperty SortByProperty { get; init; }

        /// <inheritdoc/>
        public SortByDirection SortByDirection { get; init; }

        /// <inheritdoc/>
        public int? Top { get; init; } = -1;

        /// <summary>
        /// Create default query options using common filters
        /// </summary>
        /// <param name="signalTypes">Signal types to include</param>
        /// <param name="startDateTime">Get signals with StartTime greater than or equal to this value</param>
        /// <param name="endDateTime">Get signals with EndTime less than or equal to this value</param>
        /// <returns>CSS query options instance</returns>
        public static CssQueryOptions CreateDefault(HashSet<SignalType> signalTypes, DateTimeOffset startDateTime, DateTimeOffset? endDateTime) =>
            new CssQueryOptions
            {
                SignalTypes = signalTypes,
                StartFilter = new DateTimeFilter
                {
                    FilterProperty = FilterProperty.StartTime,
                    ComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                    DateTime = startDateTime,
                },
                EndFilter = endDateTime == null ?
                            null :
                            new DateTimeFilter
                            {
                                FilterProperty = FilterProperty.EndTime,
                                ComparisonOperator = ComparisonOperator.LessThanOrEqual,
                                DateTime = (DateTimeOffset)endDateTime,
                            },
                Top = -1,
                SortByDirection = SortByDirection.Asc,
                SortByProperty = SortByProperty.StartTime
            };

        /// <inheritdoc/>
        public void Validate() => Validate(false);

        /// <inheritdoc/>
        public void Validate(bool allowNoSignalTypeFilter)
        {
            if (!allowNoSignalTypeFilter && (this.SignalTypes == null || this.SignalTypes.Count == 0))
            {
                throw new ArgumentException("At least one signal type must be specified");
            }

            if (this.StartFilter == null)
            {
                throw new ArgumentException($"{nameof(ICssQueryOptions.StartFilter)} must not be null");
            }

            if (this.StartFilter.FilterProperty != FilterProperty.StartTime)
            {
                throw new ArgumentException($"{nameof(ICssQueryOptions.StartFilter)} only supports the filter property {nameof(FilterProperty.StartTime)}");
            }

            if (this.StartFilter.ComparisonOperator != ComparisonOperator.GreaterThanOrEqual)
            {
                throw new ArgumentException($"{nameof(ICssQueryOptions.StartFilter)} only supports the comparison operator {nameof(ComparisonOperator.GreaterThanOrEqual)}");
            }

            // EndFilter is optional
            if (this.EndFilter != null)
            {
                if (this.EndFilter.FilterProperty != FilterProperty.EndTime)
                {
                    throw new ArgumentException($"{nameof(ICssQueryOptions.EndFilter)} only supports the filter property {nameof(FilterProperty.EndTime)}");
                }

                if (this.EndFilter.DateTime < this.StartFilter.DateTime)
                {
                    throw new ArgumentException("End time must be greater than start time");
                }

                if (this.EndFilter.ComparisonOperator != ComparisonOperator.LessThanOrEqual)
                {
                    throw new ArgumentException($"{nameof(ICssQueryOptions.EndFilter)} only supports the comparison operator {nameof(ComparisonOperator.LessThanOrEqual)}");
                }
            }
        }
    }
}
