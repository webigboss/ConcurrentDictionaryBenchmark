// <copyright file="SdkConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Constants for SDK
    /// </summary>
    internal class SdkConstants
    {
        /// <summary>
        /// The header name for specifying the target mailbox
        /// </summary>
        internal const string AnchorMailboxHeaderKey = "X-AnchorMailbox";

        /// <summary>
        /// The header name for specifying the Client Application Id
        /// </summary>
        internal const string CallingServiceAppIdHeaderKey = "callingServiceAppId";

        /// <summary>
        /// The header name for specifying the Routing session key
        /// </summary>
        internal const string RoutingSessionHeaderKey = "X-RoutingParameter-SessionKey";

        /// <summary>
        /// The header name for specifying the Accept header key
        /// </summary>
        internal const string AcceptHeaderKey = "Accept";

        /// <summary>
        /// The header name for specifying the Accept header value with content type as json
        /// </summary>
        internal const string AcceptHeaderJsonValue = "application/json";

        /// <summary>
        /// The header name for specifying the Authorization header key
        /// </summary>
        internal const string AuthorizationHeaderKey = "Authorization";

        /// <summary>
        /// The header name for specifying the Prefer header key
        /// </summary>
        internal const string PreferHeaderKey = "Prefer";

        /// <summary>
        /// The header name for specifying the Prefer header value.
        /// </summary>
        internal const string PreferHeaderValue = "substrate.signals-add-as-compound,exchange.behavior=\"SignalAccessV2,OpenComplexTypeExtensions,ApplicationData\",outlook.open-as-system";

        /// <summary>
        /// Cut off date for routing requests to SIGS.
        /// it means queries before (or partially overlapping) this date will be routed to SIGS.
        /// </summary>
        internal static readonly DateTimeOffset DefaultRouteToSigsCutOffDate = new (2023, 10, 6, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Route to sigs cut off date per signal type per ring, if the signal type is not in the dictionary, then default to the <see cref="DefaultRouteToSigsCutOffDate"/>
        /// </summary>
        internal static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<SignalType, DateTimeOffset>> RouteToSigsCutOffDateByRingBySignalType =
            new Dictionary<string, IReadOnlyDictionary<SignalType, DateTimeOffset>>(comparer: StringComparer.OrdinalIgnoreCase) 
            {
                {
                    // this is just a sample also used by unit tests
                    "Dev", new Dictionary<SignalType,  DateTimeOffset>()
                    {
                        { SignalType.MessageSent, new DateTimeOffset(2023, 6, 13, 0, 0, 0, TimeSpan.Zero) },
                        { SignalType.InstantMessage, new DateTimeOffset(2023, 6, 14, 0, 0, 0, TimeSpan.Zero) },
                    } 
                },
            };
    }
}
