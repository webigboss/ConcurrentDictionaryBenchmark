// <copyright file="ISignal.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using Microsoft.M365.Substrate.Sdk.SubstrateItem.DataModel.Signals;

    /// <summary>
    /// Signal payload
    /// </summary>
    public interface ISignal : ICssItem
    {
        /// <summary>
        /// Core property - IsPrivate
        /// </summary>
        bool IsPrivate { get; }

        /// <summary>
        /// Core property - DurationInSeconds
        /// </summary>
        long? DurationInSeconds { get; }

        /// <summary>
        /// Core property - Status
        /// </summary>
        Status Status { get; }

        /// <summary>
        /// Signal Locale
        /// </summary>
        string Locale { get; }

        /// <summary>
        /// Core property - Application
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Core property - Actor
        /// </summary>
        Actor Actor { get; }

        /// <summary>
        /// Core property - Compliance setting
        /// </summary>
        Compliance Compliance { get; }

        /// <summary>
        /// Device Type for a signal
        /// </summary>
        Device? Device { get; }

        /// <summary>
        /// The Item a Signal is about
        /// </summary>
        Item? Item { get; }
        
        /// <summary>
        /// The correlation vector of the event
        /// </summary>
        string CV { get; }
        
        /// <summary>
        /// The immutable Id of the Signal
        /// </summary>
        string ImmutableId { get; }

        /// <summary>
        /// Custom properties present in the Signal
        /// </summary>
        string CustomPropertiesAsString { get; }

        /// <summary>
        /// Enrichments to the signal
        /// </summary>
        SignalEnrichments Enrichments { get; }
    }
}
