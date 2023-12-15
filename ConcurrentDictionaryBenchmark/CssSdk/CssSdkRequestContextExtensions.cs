// <copyright file="CssSdkRequestContextExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk
{
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// Extension methods for ICssSdkRequestContext
    /// </summary>
    internal static class CssSdkRequestContextExtensions
    {
        /// <summary>
        /// Get the route key for the request context
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public static string GetRouteKey(this ICssSdkRequestContext requestContext)
            => $"MBX:{requestContext.ShardKey.MailboxGuid}@{requestContext.ShardKey.TenantId}"; 
    }
}
