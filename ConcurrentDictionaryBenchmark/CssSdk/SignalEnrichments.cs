// <copyright file="SignalEnrichments.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using MessagePack;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Store.DsApi;

    /// <summary>
    /// Enrichments to the signal class
    /// </summary>
    [MessagePackObject]
    public class SignalEnrichments
    {
        /// <summary>
        /// Resolved identities object containing mapping from json path in CustomProperties to resolved CssUser
        /// </summary>
        [IgnoreMember]
#pragma warning disable CA2227 // Collection properties should be read only
        public IDictionary<string, ICollection<CssUser>> ResolvedIdentities { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Resolved identites as string
        /// </summary>
        [Key(0)]
        [JsonIgnore]
        public string ResolvedIdentitiesAsString
        {
            get
            {
                return JsonSerializer.Serialize(this.ResolvedIdentities);
            }

            set
            {
                this.ResolvedIdentities = JsonSerializer.Deserialize<IDictionary<string, ICollection<CssUser>>>(value);
            }
        }
    }
}
