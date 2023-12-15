// <copyright file="SigsQueryResponse.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Microsoft.Substrate.CompactSignalStore.Item.DataModels;

    /// <summary>
    /// Sigs OData response
    /// </summary>
    internal class SigsQueryResponse
    {
        /// <summary>
        /// The values
        /// </summary>
        [JsonPropertyName("value")]
        public IEnumerable<Signal> Value { get; set; }

        /// <summary>
        /// the OData next link
        /// </summary>
        [JsonPropertyName("@odata.nextLink")]
        public string NextLink { get; set; }
    }
}
