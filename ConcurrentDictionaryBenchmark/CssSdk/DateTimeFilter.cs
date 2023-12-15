// <copyright file="DateTimeFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using System;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;

    /// <summary>
    /// Defines a filter based on datetime
    /// </summary>
    public record DateTimeFilter : IDateTimeFilter
    {
        /// <inheritdoc/>
        public FilterProperty FilterProperty { get; init; }

        /// <inheritdoc/>
        public DateTimeOffset DateTime { get; init; }

        /// <inheritdoc/>
        public ComparisonOperator ComparisonOperator { get; init; }
    }
}
