// <copyright file="Status.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Signal Status
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Unknown = 0,
        Completed,
        InProgress,
        CompletedMissingStart
    }
}
