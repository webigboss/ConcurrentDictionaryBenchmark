// <copyright file="ISignalContainer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System;

    /// <summary>
    /// Data Model for User Identify Tagged SIGS signal
    /// </summary>
    public interface ISignalContainer
    {
        /// <summary>
        /// The Shard Tag to be used for tagging the Signal
        /// </summary>
        ShardKey ShardKey { get; }

        /// <summary>
        /// Supported Signal
        /// </summary>
        SignalType SignalType { get; }

        /// <summary>
        /// Signal StartTime
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        /// Signal EndTime
        /// </summary>
        DateTimeOffset EndTime { get; }

        /// <summary>
        /// The SIGS signal will be stored in this payload
        /// </summary>
        ISignal Signal { get; }
    }
}
