// <copyright file="ICssQueryOptions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Query options supported by CSS
    /// </summary>
    public interface ICssQueryOptions
    {
        /// <summary>
        /// Signal types
        /// </summary>
        IReadOnlySet<SignalType> SignalTypes { get; }

        /// <summary>
        /// Start filter
        /// </summary>
        IDateTimeFilter StartFilter { get; }

        /// <summary>
        /// End filter
        /// </summary>
        IDateTimeFilter EndFilter { get; }

        /// <summary>
        /// Sort by property
        /// </summary>
        SortByProperty SortByProperty { get; }

        /// <summary>
        /// Sort by direction
        /// </summary>
        SortByDirection SortByDirection { get; }

        /// <summary>
        /// Maximum number of signals to return
        /// </summary>
        /// <remarks>
        /// A value of -1 indicates all results will be returned
        /// </remarks>
        int? Top { get; }

        /// <summary>
        /// Validates CssQueryOptions instance
        /// </summary>
        /// <exception cref="ArgumentException">Exception thrown if query options are invalid</exception>
        void Validate();

        /// <summary>
        /// Validates CssQueryOptions instance
        /// </summary>
        /// <param name="allowNoSignalTypeFilter">Allow specifying no SignalType in the filter query</param>
        /// <exception cref="ArgumentException">Exception thrown if query options are invalid</exception>
        void Validate(bool allowNoSignalTypeFilter);
    }
}
