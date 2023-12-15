// <copyright file="ICssSdkRequestRouter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    /// <summary>
    /// Interface of CSS SDK request router
    /// </summary>
    public interface ICssSdkRequestRouter
    {
        /// <summary>
        /// Route the query request to the right client
        /// </summary>
        /// <param name="filter">OData filter string</param>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        /// 
        RoutingResult Route(string filter, ICssSdkRequestContext requestContext);
    }
}
