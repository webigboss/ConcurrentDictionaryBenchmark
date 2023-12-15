// <copyright file="ICssItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System;

    /// <summary>
    /// Interface that defines a Css Item
    /// </summary>
    public interface ICssItem
    {
        /// <summary>
        /// Core property - StartTime
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        /// Core property - EndTime
        /// </summary>
        DateTimeOffset EndTime { get; }

        /// <summary>
        /// Core property - CreationTime
        /// </summary>
        DateTimeOffset CreationTime { get; }

        /// <summary>
        /// Core property - Signal Type
        /// </summary>
        SignalType SignalType { get; }

        /// <summary>
        /// Core property - Shard Key
        /// </summary>
        ShardKey ShardTag { get; }

        /// <summary>
        /// Core property - Id
        /// </summary>
        string Id { get; }
    }
}
