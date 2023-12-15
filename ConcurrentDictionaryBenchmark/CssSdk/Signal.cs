// <copyright file="Signal.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using MessagePack;
    using Microsoft.AspNet.OData.Query;
    using Microsoft.M365.Substrate.Sdk.SubstrateItem.DataModel.Signals;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using CssContractsSignals = Contracts.Signals;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    [MessagePackObject]
    [Filter("SignalType", "StartTime", "EndTime")]
    [OrderBy("SignalType", "StartTime", "EndTime")]
    public sealed class Signal : ISignal
    {
        /// <inheritdoc />
        [Key(0)]
        public DateTimeOffset StartTime { get; set; }

        /// <inheritdoc />
        [Key(1)]
        public DateTimeOffset EndTime { get; set; }

        /// <inheritdoc />
        [Key(2)]
        public CssContractsSignals.SignalType SignalType { get; set; }

        /// <inheritdoc />
        [Key(3)]
        public DateTimeOffset CreationTime { get; set; }

        /// <inheritdoc />
        [Key(4)]
        public string Id { get; set; }

        /// <inheritdoc />
        [Key(5)]
        public long? DurationInSeconds { get; set; }

        /// <inheritdoc />
        [Key(6)]
        public CssContractsSignals.Status Status { get; set; }

        /// <inheritdoc />
        [Key(7)]
        public string Locale { get; set; }

        /// <inheritdoc />
        [Key(8)]
        public CssContractsSignals.Application Application { get; set; }

        /// <inheritdoc />
        [Key(9)]
        public CssContractsSignals.Actor Actor { get; set; }

        /// <inheritdoc />
        [Key(10)]
        public Compliance Compliance { get; set; }

        /// <inheritdoc />
        [Key(11)]
        public CssContractsSignals.Device? Device { get; set; }

        /// <inheritdoc />
        [Key(12)]
        public CssContractsSignals.Item? Item { get; set; }

        /// <inheritdoc />
        [Key(13)]
        [JsonIgnore]
        public string CustomPropertiesAsString
        {
            get
            {
                return JsonSerializer.Serialize(CustomPropertiesJson);
            }

            set
            {
                CustomPropertiesJson = JsonSerializer.Deserialize<object>(value);
            }
        }

        /// <inheritdoc />
        [Key(14)]
        public string CV { get; set; }

        /// <inheritdoc />
        [Key(15)]
        public string ImmutableId { get; set; }

        /// <summary>
        /// CustomProperties from JSON
        /// </summary>
        [JsonPropertyName("CustomProperties")]
        [IgnoreMember]
        public object CustomPropertiesJson { get; set; }

        /// <inheritdoc/>
        [Key(17)]
        public ShardKey ShardTag { get; set; }

        /// <inheritdoc />
        [Key(18)]
        public bool IsPrivate { get; set; }

        /// <inheritdoc />
        [Key(19)]
        public SignalEnrichments Enrichments { get; set; }
    }
}
