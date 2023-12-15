// <copyright file="RoutingResult.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    using System.Collections.Generic;

    /// <summary>
    /// Routing result returned by the router
    /// </summary>
    public class RoutingResult
    {
        /// <summary>
        /// The client type to route to
        /// </summary>
        public ClientType To { get; init; }

        /// <summary>
        /// When the ClientType is CssAndSigs, this dictionary contains the filters to be routed to each client.
        /// </summary>
        public IReadOnlyDictionary<ClientType, string> SplitFilters { get; init; }

        /// <summary>
        /// The reason of why the router makes the routing decision.
        /// mostly important for checking the reason of routing to SIGS instead of CSS.
        /// </summary>
        public string Reason { get; init; }

        /// <!inheritdoc/>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Reason))
            {
                return $"Routing to {To}.";
            }
            else
            {
                return $"Routing to {To} with reason: {Reason}.";
            }
        }
    }
}
